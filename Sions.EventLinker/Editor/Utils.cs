#nullable enable

using System;
using System.Linq;
using System.Text.RegularExpressions;

using UnityEditor;

namespace Sions.EventLinker
{
    internal class Utils
    {
        public static string GetAllowMethodName(string path)
        {
            string result =  new String(path.Select(v => v switch
            {
                >= 'a' and <= 'z' => v,
                >= 'A' and <= 'Z' => v,
                >= '0' and <= '9' => v,
                _ => '_'
            }).ToArray());

            result = result.Replace("_m_PersistentCalls_m_Calls_Array_data_", "_");
            result = Regex.Replace(result , "_+", "_");
            result = Regex.Replace(result, "_$", "");
            return result;
        }

        public static string GetTypeName(Type? argumentType)
        {
            if (argumentType == null)
            {
                return "object";
            }
            else if (argumentType == typeof(int))
            {
                return "int";
            }
            else if (argumentType == typeof(float))
            {
                return "float";
            }
            else if (argumentType == typeof(bool))
            {
                return "bool";
            }
            else if (argumentType == typeof(string))
            {
                return "string";
            }
            else
            {
                return argumentType.FullName.Replace('+', '.');
            }
        }

        internal static Type? GetType(string stringValue)
        {
            try
            {
                return Type.GetType(stringValue);
            }
            catch
            {
                return null;
            }
        }
    }
}