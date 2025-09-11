#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    public class ScriptableObjectFindProcessor : FindProcessorBase
    {
        private SerializeSolver solver;

        public ScriptableObjectFindProcessor(EventGenerator generator) : base(generator)
        {
            solver = generator.GetSolver<SerializeSolver>();
        }

        public override void Process()
        {
            if (!Store.Settings.IncludeScriptableObject) return;
            base.Process();
        }

        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:ScriptableObject").ToList();
        }

        protected override void Open()
        {
            Store.CurrentFileName = path;
            Store.CurrentInternalPath = "";

            var so = AssetDatabase.LoadAssetAtPath<ScriptableObject>(path);
            solver.Solve(so);
        }
    }
}