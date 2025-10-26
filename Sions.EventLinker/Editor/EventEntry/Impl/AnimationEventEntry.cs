#nullable enable

using System.Linq;
using System.Reflection;

using UnityEditor;

namespace Sions.EventLinker
{
    /// <summary>
    /// Animation 이벤트 엔트리를 나타냅니다.
    /// </summary>
    public class AnimationEventEntry : EventEntryBase
    {
        /// <summary>
        /// Animation 이벤트 엔트리를 생성합니다.
        /// </summary>
        /// <param name="time">이벤트 발생 시간</param>
        /// <param name="method">메서드 정보</param>
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
