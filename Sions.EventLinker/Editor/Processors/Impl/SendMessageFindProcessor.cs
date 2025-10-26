#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using UnityEditor;

namespace Sions.EventLinker
{
    /// <summary>
    /// SendMessage 호출을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class SendMessageFindProcessor : FindProcessorBase
    {
        private Regex m_Search;  // SendMessage 패턴 검색 정규식
        private MethodInfo m_GetAssemblyNameMethod;  // 어셈블리 이름 가져오기 메서드
        private MethodInfo m_GetNamespaceMethod;  // 네임스페이스 가져오기 메서드
        private SendMessageSolver m_Solver;  // SendMessage 솔버

        /// <summary>
        /// SendMessage 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public SendMessageFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<SendMessageSolver>();
            m_Search = new Regex(@"\b(SendMessage(Upwards)?|BroadcastMessage)\s*\(\s*""(?<methodName>[^""]+)""", RegexOptions.Compiled);
            m_GetAssemblyNameMethod = typeof(MonoScript).GetMethod("GetAssemblyName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            m_GetNamespaceMethod = typeof(MonoScript).GetMethod("GetNamespace", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }


        /// <summary>
        /// SendMessage 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludeSendMessage) return;
            base.Process();
        }

        /// <summary>
        /// MonoScript 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>MonoScript GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:MonoScript").ToList();
        }

        /// <summary>
        /// MonoScript 파일을 열어 SendMessage 호출을 검색합니다.
        /// </summary>
        protected override void Open()
        {
            Store.CurrentFileName = path;

            var solver = Store.GetSolver<AnimationEventSolver>();

            var script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
            var code = script.text;

            var cls = script.GetClass();
            var asmName = (string)m_GetAssemblyNameMethod.Invoke(script, null);
            var namespaceName = (string)m_GetNamespaceMethod.Invoke(script, null);
            Print($"MonoScript: {path} / {asmName} / {namespaceName}");

            // SendMessage 패턴 검색
            var matches = m_Search.Matches(code);
            foreach (Match match in matches)
            {
                if (!match.Success) continue;

                var group = match.Groups["methodName"];
                GetPosition(group.Index, code, out int line, out int offset);

                Store.CurrentInternalPath = $"line:{line},offset:{offset}";

                // TODO : 파라미터를 알수있는 구문분석 필요
                m_Solver.Solve(group.Value, null);
            }
        }

        /// <summary>
        /// 코드 내 인덱스 위치를 라인과 오프셋으로 변환합니다.
        /// </summary>
        /// <param name="index">문자 인덱스</param>
        /// <param name="code">전체 코드</param>
        /// <param name="line">라인 번호 (출력)</param>
        /// <param name="offset">라인 내 오프셋 (출력)</param>
        private void GetPosition(int index, string code, out int line, out int offset)
        {
            line = code.Take(index).Count(v => v == '\n') + 1;

            int lastLineStart = code.LastIndexOf('\n', index);
            if (lastLineStart == -1)
            {
                offset = index;
            }
            else
            {
                offset = index - (lastLineStart + 1);
            }
        }
    }
}
