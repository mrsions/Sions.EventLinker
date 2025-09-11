#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEngine;

namespace Sions.EventLinker
{
    public class SendMessageSolver : ISolver
    {
        public Dictionary<string, List<MethodInfo>> methods;

        public SendMessageSolver(EventGenerator generator)
        {
            Generator = generator;

            methods = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(asm => asm.GetTypes())
                .Where(type => typeof(MonoBehaviour).IsAssignableFrom(type))
                .SelectMany(type =>
                {
                    // TODO : GenericDefinition이나 Abstract는 포함하지 않아야한다.
                    // TODO : 부모가 GeneircDefinition인 경우 해당 부모의 메서드도 포함해서 가져와야한다.
                    // TODO : 부모에서 가져온 MethodInfo의 DeclaredType이 GenericDefinition인 경우 DeclaredType을 자식Type으로 변경해서 저장할 수 있도록 해야한다.
                    return type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy | BindingFlags.DeclaredOnly)
                        .Where(v => ValidParam(v))
                        .GroupBy(m => m.Name)
                        .SelectMany(v =>
                        {
                            var first = v.FirstOrDefault(v => v.GetParameters().Length == 0);
                            if (first != null) return new[] { first };

                            first = v.FirstOrDefault(v => v.GetParameters()[0].ParameterType == typeof(object));
                            if (first != null) return new[] { first };

                            return v.ToArray();
                        });
                })
                .GroupBy(v => v.Name)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        private static bool ValidParam(MethodInfo method)
        {
            if (method.DeclaringType == typeof(System.Object)) return false;
            else if (method.IsAbstract) return false;
            else if (method.DeclaringType.IsInterface) return false;

            // TODO : 어셈블리 데피니션을 통한 에디터 패키지 판별 필요
            if (method.DeclaringType.FullName.Contains("TestRunner")) return false;
            if (method.DeclaringType.FullName.StartsWith("UnityEditor")) return false;

            var mParams = method.GetParameters();
            if (mParams.Length == 0)
            {
                return true;
            }
            else if (mParams.Length == 1)
            {
                return true;
            }
            return false;
        }

        public EventGenerator Generator { get; }

        public void Solve(string methodName, Type paramType)
        {
            if (!methods.TryGetValue(methodName, out var list))
            {
                return;
            }

            foreach (var method in list)
            {
                var result = new SendMessageEventEntry(method);
                result.PropertyPath = method.DeclaringType.FullName;
                Generator.AddEvent(result);
            }
        }
    }
}