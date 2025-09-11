#nullable enable

using System.Collections.Generic;
using System.Linq;

using UnityEditor;

using UnityEngine;

namespace Sions.EventLinker
{
    public class AnimationFindProcessor : FindProcessorBase
    {
        public AnimationFindProcessor(EventGenerator generator) : base(generator)
        {
        }

        public override void Process()
        {
            if (!Store.Settings.IncludeAnimation) return;
            base.Process();
        }

        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:Animation").ToList();
        }

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
                if (obj is not AnimationClip anim) continue;
                if (anim.ToString().Contains(" (UnityEngine.PreviewAnimationClip)")) continue;

                if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(anim, out string guid, out long localId))
                {
                    Store.CurrentInternalPath = $"{anim.name}({localId})";
                }
                else
                {
                    Store.CurrentInternalPath = anim.name;
                }

                foreach (var ev in anim.events)
                {
                    solver.Solve(ev);
                }
            }
        }
    }
}