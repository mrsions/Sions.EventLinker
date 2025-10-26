#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    /// <summary>
    /// Animation 파일을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class AnimationFindProcessor : FindProcessorBase
    {
        /// <summary>
        /// Animation 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public AnimationFindProcessor(EventGenerator generator) : base(generator)
        {
        }

        /// <summary>
        /// Animation 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludeAnimation) return;
            base.Process();
        }

        /// <summary>
        /// Animation 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>Animation GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:Animation").ToList();
        }

        /// <summary>
        /// Animation 파일을 열어 이벤트를 처리합니다.
        /// </summary>
        protected override void Open()
        {
            Store.CurrentFileName = path;
            if (Store.CurrentFileName.Contains("/TestA/testanim.anim"))
            {

            }

            var solver = Store.GetSolver<AnimationEventSolver>();

            var objs = AssetDatabase.LoadAllAssetsAtPath(path);
            foreach (var obj in objs)
            {
                // AnimationClip만 처리
                if (obj is not AnimationClip anim) continue;
                if (anim.ToString().Contains(" (UnityEngine.PreviewAnimationClip)")) continue;

                // GUID와 로컬 ID로 내부 경로 설정
                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(anim, out string guid, out long localId))
                {
                    Store.CurrentInternalPath = $"{anim.name}({localId})";
                }
                else
                {
                    Store.CurrentInternalPath = anim.name;
                }

                // 각 이벤트를 솔버로 처리
                foreach (var ev in anim.events)
                {
                    solver.Solve(ev);
                }
            }
        }
    }
}
