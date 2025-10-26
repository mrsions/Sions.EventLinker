#nullable enable

using System.Linq;
using System.Reflection;

using UnityEditor;

namespace Sions.EventLinker
{
    /// <summary>
    /// SendMessage 이벤트 엔트리를 나타냅니다.
    /// </summary>
    public class SendMessageEventEntry : EventEntryBase
    {
        /// <summary>
        /// SendMessage 이벤트 엔트리를 생성합니다.
        /// </summary>
        /// <param name="method">메서드 정보</param>
        public SendMessageEventEntry(MethodInfo method)
        {
            Type = EventType.SendMessage;

            HasTarget = true; // 아마 있을것으로 예상
            TargetType = method.DeclaringType;

            TargetMethod = method;
            TargetMethodName = method.Name;
            TargetMethodParameters = method.GetParameters().Select(p => p.ParameterType).ToArray();

        }
    }
}
