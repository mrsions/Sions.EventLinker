#nullable enable

using UnityEngine;

namespace Sions.EventLinker
{
    public class GameObjectSolver : ISolver
    {
        private SerializeSolver m_Solver;
        public EventGenerator Generator { get; }

        public GameObjectSolver(EventGenerator generator)
        {
            Generator = generator;
            m_Solver = Generator.GetSolver<SerializeSolver>();
        }

        public void Solve(GameObject go, string? path = null)
        {
            if (!Generator.AddInspection(go)) return;

            path = path != null ? $"{path}/{go.name}" : go.name;

            var compoennts = go.GetComponents<Component>();
            for (var i = 0; i < compoennts.Length; i++)
            {
                var comp = compoennts[i];

                if (!comp)
                {
                    Debug.LogWarning($"Maybe Missing Component (path:'{Generator.CurrentFileName}/{path}', compoennt:{i}(included transform.))");
                    continue;
                }

                if (Generator.IgnoreComponentType.Contains(comp.GetType()))
                {
                    continue;
                }

                Generator.CurrentInternalPath = $"{path}[{i}]";

                m_Solver.Solve(compoennts[i]);
            }

            foreach (Transform child in go.transform)
            {
                Solve(child.gameObject, path);
            }
        }
    }
}