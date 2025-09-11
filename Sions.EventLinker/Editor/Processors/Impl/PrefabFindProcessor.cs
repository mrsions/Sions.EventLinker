#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    public class PrefabFindProcessor : FindProcessorBase
    {
        private GameObjectSolver m_Solver;

        public PrefabFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<GameObjectSolver>();
        }

        public override void Process()
        {
            if (!Store.Settings.IncludePrefab) return;
            base.Process();
        }

        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:prefab").ToList();
        }

        protected override void Open()
        {
            Store.CurrentFileName = path;
            Store.CurrentInternalPath = "";

            var go = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            m_Solver.Solve(go);
        }
    }
}