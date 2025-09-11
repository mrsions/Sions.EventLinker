#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Sions.EventLinker
{
    public class AnimationEventSolver : ISolver
    {
        public Dictionary<string, List<MethodInfo>> methods;

        public EventGenerator Generator { get; }

        public AnimationEventSolver(EventGenerator generator)
        {
            Generator = generator;

            methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(type => typeof(MonoBehaviour).IsAssignableFrom(type))
                .SelectMany(type =>
                {
                    return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly)
                        .Where(v => ValidParam(v))
                        .GroupBy(m => m.Name).Select(v => v.FirstOrDefault());
                })
                .GroupBy(v => v.Name)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private static bool ValidParam(MethodInfo method)
        {
            if (method.DeclaringType == typeof(MonoBehaviour)) return false;
            else if (method.DeclaringType == typeof(Behaviour)) return false;
            else if (method.DeclaringType == typeof(Component)) return false;
            else if (method.DeclaringType == typeof(UnityEngine.Object)) return false;
            else if (method.DeclaringType == typeof(System.Object)) return false;
            else if (method.IsAbstract) return false;
            else if (method.DeclaringType.IsInterface) return false;

            var mParams = method.GetParameters();
            if (mParams.Length == 0)
            {
                return true;
            }
            else if (mParams.Length == 1)
            {
                var ptype = mParams[0].ParameterType;
                if (ptype == typeof(float)) return true;
                if (ptype == typeof(int)) return true;
                if (ptype == typeof(string)) return true;
                if (typeof(UnityEngine.Object).IsAssignableFrom(ptype)) return true;
            }
            return false;
        }

        public void Solve(AnimationEvent ev)
        {
            if (!methods.TryGetValue(ev.functionName, out var list))
            {
                return;
            }

            foreach (var method in list)
            {
                var result = new AnimationEventEntry(ev.time, method);
                Generator.AddEvent(result);
            }
        }
    }
}