#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;

namespace Sions.EventLinker
{
    public class AnimatorFindProcessor : FindProcessorBase
    {
        private SerializeSolver m_Solver;

        public AnimatorFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<SerializeSolver>();
        }

        public override void Process()
        {
            if (!Store.Settings.IncludeAnimator) return;
            base.Process();
        }

        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:AnimatorController").ToList();
        }

        protected override void Open()
        {
            Store.CurrentFileName = path;

            var animCon = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            for (var li = 0; li < animCon.layers.Length; li++)
            {
                var layer = animCon.layers[li];

                var layerBehaviours = layer.stateMachine.behaviours;
                for (var behaviourIndex = 0; behaviourIndex < layerBehaviours.Length; behaviourIndex++)
                {
                    var behaviour = layerBehaviours[behaviourIndex];

                    Store.CurrentInternalPath = $"Layers/[{li}]{layer.name}/Behaviours/[{behaviourIndex}]{behaviour.GetType().FullName}";
                    m_Solver.Solve(behaviour);
                }

                // TODO : 더 하위 스테이트가 존재하는지 확인필요

                var layerStates = layer.stateMachine.states;
                for (var si = 0; si < layerStates.Length; si++)
                {
                    var state = layerStates[si];
                    var stateBehaviours = state.state.behaviours;
                    for (var behaviourIndex = 0; behaviourIndex < stateBehaviours.Length; behaviourIndex++)
                    {
                        var behaviour = stateBehaviours[behaviourIndex];

                        Store.CurrentInternalPath = $"Layers/[{li}]{layer.name}/States/[{si}]{state.state.name}/[{behaviourIndex}]{behaviour.GetType().FullName}";
                        m_Solver.Solve(behaviour);
                    }
                }
            }
        }
    }
}