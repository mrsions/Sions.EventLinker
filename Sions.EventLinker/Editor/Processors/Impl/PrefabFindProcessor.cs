#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    /// <summary>
    /// Prefab 파일을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class PrefabFindProcessor : FindProcessorBase
    {
        private GameObjectSolver m_Solver;  // GameObject 솔버

        /// <summary>
        /// Prefab 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public PrefabFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<GameObjectSolver>();
        }

        /// <summary>
        /// Prefab 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludePrefab) return;
            base.Process();
        }

        /// <summary>
        /// Prefab 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>Prefab GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:prefab").ToList();
        }

        /// <summary>
        /// Prefab 파일을 열어 GameObject를 처리합니다.
        /// </summary>
        protected override void Open()
        {
            Store.CurrentFileName = path;
            Store.CurrentInternalPath = "";

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            m_Solver.Solve(go);
        }
    }
}
