#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

using UnityEditor;

using UnityEngine;
using UnityEngine.Events;

using uobj = UnityEngine.Object;

namespace Sions.EventLinker
{
    public class SerializeSolver : ISolver
    {
        public SerializeSolver(EventGenerator generator)
        {
            Generator = generator;
        }

        public EventGenerator Generator { get; }

        public void Solve(uobj obj)
        {
            var so = new SerializedObject(obj);
            var it = so.GetIterator();

            const int maxDepth = 50;
            var visitedManagedRefs = new HashSet<long>();

            var enterChildren = true;
            while (it.Next(enterChildren))
            {
                // depth 제한
                if (it.depth >= maxDepth)
                {
                    enterChildren = false;
                    continue;
                }

                // 순환 참조 방지
                if (it.propertyType == SerializedPropertyType.ManagedReference)
                {
                    long id = it.managedReferenceId;

                    // null ref는 제외
                    if (id != 0)
                    {
                        // 이미 본 managed reference면 다시 그 내부로 들어가지 않음
                        if (!visitedManagedRefs.Add(id))
                        {
                            enterChildren = false;
                            continue;
                        }
                    }
                }

                if (!IsEventType(it, out var eventType))
                    continue;

                // 이벤트 프로퍼티를 찾았으면, 그 자식 전체를 다시 타지 않도록 막음
                enterChildren = false;

                var calls = it.FindPropertyRelative("m_PersistentCalls.m_Calls");
                if (calls == null || calls.arraySize == 0)
                    continue;

                for (var i = 0; i < calls.arraySize; i++)
                {
                    var result = new UnityEventEntry(calls.GetArrayElementAtIndex(i), eventType);
                    Generator.AddEvent(result);
                }
            }
        }

        private static string GetVisitKey(SerializedProperty property)
        {
            // managed reference는 propertyPath만으로 부족할 수 있어서 id도 같이 사용
            if (property.propertyType == SerializedPropertyType.ManagedReference)
            {
                return $"{property.serializedObject.targetObject.GetEntityId()}|{property.propertyPath}|depth:{property.depth}|mrid:{property.managedReferenceId}";
            }

            return $"{property.serializedObject.targetObject.GetEntityId()}|{property.propertyPath}|depth:{property.depth}";
        }

        private bool IsEventType(SerializedProperty it, [NotNullWhen(true)] out Type? type)
        {
            type = null;

            if (it.propertyType != SerializedPropertyType.Generic) return false;
            if (it.isArray) return false;

            type = GetUnityEventType(it);
            if (type == null) return false;

            return true;
        }

        private Type? GetUnityEventType(SerializedProperty it)
        {
            try
            {
                var box = it.boxedValue;
                return (box as UnityEventBase)?.GetType();
            }
            catch (NullReferenceException)
            {
                return null;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains(" cannot be retrieved with boxedValue because it is a built-in type "))
                {
                    if (ex.Message.Contains("'UnityEvent`1[["))
                    {
                        var match = Regex.Match(ex.Message, @"UnityEvent`1\[\[([^]]+)]]'");
                        if (match.Success)
                        {
                            var genericArg = Utils.GetType(match.Groups[1].Value);
                            if (genericArg != null)
                            {
                                return typeof(UnityEvent<>).MakeGenericType(genericArg);
                            }
                        }
                    }
                    else if (ex.Message.Contains("'UnityEvent'"))
                    {
                        return typeof(UnityEvent);
                    }
                }
                else if (ex.Message.Contains(" cannot be retrieved with boxedValue because of its type "))
                {
                }
                else
                {
                    Debug.LogError(ex);
                }
            }

            return null;
        }
    }
}