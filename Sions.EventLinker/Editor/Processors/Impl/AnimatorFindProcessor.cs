#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;
using UnityEditor.Animations;

namespace Sions.EventLinker
{
    /// <summary>
    /// AnimatorController 파일을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class AnimatorFindProcessor : FindProcessorBase
    {
        private SerializeSolver m_Solver;  // Serialize 솔버

        /// <summary>
        /// Animator 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public AnimatorFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<SerializeSolver>();
        }

        /// <summary>
        /// Animator 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludeAnimator) return;
            base.Process();
        }

        /// <summary>
        /// AnimatorController 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>AnimatorController GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:AnimatorController").ToList();
        }

        /// <summary>
        /// AnimatorController 파일을 열어 이벤트를 처리합니다.
        /// </summary>
        protected override void Open()
        {
            Store.CurrentFileName = path;

            var animCon = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

            // 각 레이어 순회
            for (var li = 0; li < animCon.layers.Length; li++)
            {
                var layer = animCon.layers[li];

                // 레이어 비헤이비어 처리
                var layerBehaviours = layer.stateMachine.behaviours;
                for (var behaviourIndex = 0; behaviourIndex < layerBehaviours.Length; behaviourIndex++)
                {
                    var behaviour = layerBehaviours[behaviourIndex];

                    Store.CurrentInternalPath = $"Layers/[{li}]{layer.name}/Behaviours/[{behaviourIndex}]{behaviour.GetType().FullName}";
                    m_Solver.Solve(behaviour);
                }

                // TODO : 더 하위 스테이트가 존재하는지 확인필요

                // 스테이트 비헤이비어 처리
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
