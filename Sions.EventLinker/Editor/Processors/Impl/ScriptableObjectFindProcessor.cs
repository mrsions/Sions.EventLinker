#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    /// <summary>
    /// ScriptableObject 파일을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class ScriptableObjectFindProcessor : FindProcessorBase
    {
        private SerializeSolver solver;  // Serialize 솔버

        /// <summary>
        /// ScriptableObject 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public ScriptableObjectFindProcessor(EventGenerator generator) : base(generator)
        {
            solver = generator.GetSolver<SerializeSolver>();
        }

        /// <summary>
        /// ScriptableObject 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludeScriptableObject) return;
            base.Process();
        }

        /// <summary>
        /// ScriptableObject 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>ScriptableObject GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:ScriptableObject").ToList();
        }

        /// <summary>
        /// ScriptableObject 파일을 열어 처리합니다.
        /// </summary>
        protected override void Open()
        {
            Store.CurrentFileName = path;
            Store.CurrentInternalPath = "";

            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            solver.Solve(so);
        }
    }
}
