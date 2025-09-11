#nullable enable

using System.Linq;
using System.Reflection;

using UnityEditor;

namespace Sions.EventLinker
{
    public class AnimationEventEntry : EventEntryBase
    {
        public AnimationEventEntry(float time, MethodInfo method)
        {
            Type = EventType.AnimationEvent;

            PropertyPath = $"{time:f4}s";

            HasTarget = true; // 아마 있을것으로 예상
            TargetType = method.DeclaringType;

            TargetMethod = method;
            TargetMethodName = method.Name;
            TargetMethodParameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
        }
    }
}