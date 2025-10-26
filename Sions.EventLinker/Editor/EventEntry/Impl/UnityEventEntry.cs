#nullable enable

using System;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine.Events;

namespace Sions.EventLinker
{
    /// <summary>
    /// Unity Event 엔트리를 나타냅니다.
    /// </summary>
    public class UnityEventEntry : EventEntryBase
    {
        public readonly UnityEventCallState State;  // 이벤트 호출 상태
        public readonly Type? UnityEventType;  // Unity Event 타입

        /// <summary>
        /// Unity Event 엔트리를 생성합니다.
        /// </summary>
        /// <param name="callProp">SerializedProperty의 호출 프로퍼티</param>
        /// <param name="unityEventType">Unity Event 타입</param>
        public UnityEventEntry(SerializedProperty callProp, Type unityEventType)
        {
            Type = EventType.UnityEvent;

            UnityEventType = unityEventType;
            PropertyPath = callProp.propertyPath;

            State = (UnityEventCallState)callProp.FindPropertyRelative("m_CallState").intValue;

            var target = callProp.FindPropertyRelative("m_Target").objectReferenceValue;
            TargetType = Utils.GetType(callProp.FindPropertyRelative("m_TargetAssemblyTypeName").stringValue);

            if (target)
            {
                HasTarget = true;
                TargetType = target.GetType();
            }

            TargetMethodName = callProp.FindPropertyRelative("m_MethodName").stringValue;

            var arguments = callProp.FindPropertyRelative("m_Arguments");
            var argumentMode = (PersistentListenerMode)callProp.FindPropertyRelative("m_Mode").intValue;
            TargetMethodParameters = GetParameterTypes(argumentMode, arguments, unityEventType);

            if (TargetType != null)
            {
                var methods = TargetType.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                    .Where(v => v.Name == TargetMethodName)
                    .ToList();

                if (methods.Count > 0)
                {
                    TargetMethod = methods.FirstOrDefault(t => t.GetParameters().Select(v => v.ParameterType).SequenceEqual(TargetMethodParameters));
                }
            }
        }

        /// <summary>
        /// 인자 모드에 따라 파라미터 타입들을 가져옵니다.
        /// </summary>
        /// <param name="argumentMode">인자 모드</param>
        /// <param name="args">인자 프로퍼티</param>
        /// <param name="UnityEventType">Unity Event 타입</param>
        /// <returns>파라미터 타입 배열</returns>
        private static Type[]? GetParameterTypes(PersistentListenerMode argumentMode, SerializedProperty args, Type UnityEventType)
        {
            switch (argumentMode)
            {
                case PersistentListenerMode.EventDefined:
                    return UnityEventType?.GenericTypeArguments;
                case PersistentListenerMode.Void:
                    return new Type[0];
                case PersistentListenerMode.Object:
                    return new Type[] { Utils.GetType(args.FindPropertyRelative("m_ObjectArgumentAssemblyTypeName").stringValue)! };
                case PersistentListenerMode.Int:
                    return new Type[] { typeof(int) };
                case PersistentListenerMode.Float:
                    return new Type[] { typeof(float) };
                case PersistentListenerMode.String:
                    return new Type[] { typeof(string) };
                case PersistentListenerMode.Bool:
                    return new Type[] { typeof(bool) };
            }

            return null;
        }
    }
}
