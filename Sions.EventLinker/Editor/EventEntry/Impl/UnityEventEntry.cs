#nullable enable

using System;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine.Events;

namespace Sions.EventLinker
{
    public class UnityEventEntry : EventEntryBase
    {
        public readonly UnityEventCallState State;
        public readonly Type? UnityEventType;

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