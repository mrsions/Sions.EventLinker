#nullable enable

using System;
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

            var enterChildren = true;
            while (it.Next(enterChildren))
            {
                enterChildren = true;
                if (!IsEventType(it, out var eventType)) continue;

                enterChildren = false;

                var calls = it.FindPropertyRelative("m_PersistentCalls.m_Calls");
                if (calls.arraySize == 0) continue;

                for (var i = 0; i < calls.arraySize; i++)
                {
                    var result = new UnityEventEntry(calls.GetArrayElementAtIndex(i), eventType);
                    Generator.AddEvent(result);
                }
            }
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