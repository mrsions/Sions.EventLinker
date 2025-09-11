#nullable enable

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

using UnityEditor;

namespace Sions.EventLinker
{
    public class SendMessageFindProcessor : FindProcessorBase
    {
        public SendMessageFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<SendMessageSolver>();
            m_Search = new Regex(@"\b(SendMessage(Upwards)?|BroadcastMessage)\s*\(\s*""(?<methodName>[^""]+)""", RegexOptions.Compiled);
            m_GetAssemblyNameMethod = typeof(MonoScript).GetMethod("GetAssemblyName", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
            m_GetNamespaceMethod = typeof(MonoScript).GetMethod("GetNamespace", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
        }

        private Regex m_Search;
        private MethodInfo m_GetAssemblyNameMethod;
        private MethodInfo m_GetNamespaceMethod;
        private SendMessageSolver m_Solver;


        public override void Process()
        {
            if (!Store.Settings.IncludeSendMessage) return;
            base.Process();
        }
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:MonoScript").ToList();
        }

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