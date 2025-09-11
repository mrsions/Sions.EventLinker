#nullable enable

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using UnityEditor;

namespace Sions.EventLinker
{
    public class SaveProcessor : ProcessorBase
    {
        public SaveProcessor(EventGenerator generator) : base(generator)
        {
        }

        public override int Order => int.MaxValue;

        public override void Process()
        {
            ItemCount = 1;

            ShowDialogue("Save CS Code");

            Directory.CreateDirectory(Store.Settings.GenerateFolder);
            foreach (var file in Directory.GetFiles(Store.Settings.GenerateFolder))
            {
                try { File.Delete(file); } catch { }
            }

            var groups = Store.Events.GroupBy(v => v.FilePath);
            foreach (var group in groups)
            {
                string code = BuildCode(group.Key, group.ToList());
                File.WriteAllText($"{Store.Settings.GenerateFolder}/{Utils.GetAllowMethodName(group.Key)}.cs", code);
            }

            AssetDatabase.Refresh();
        }

        private string BuildCode(string path, List<EventEntryBase> events)
        {
            var sb = new StringBuilder();

            sb.AppendLine("namespace SionsEventLink.Runtime");
            sb.AppendLine("{");
            sb.Append("    /// DIRECTORY : file://").AppendLine(Path.GetFullPath(Path.GetDirectoryName(path)).Replace('\\', '/'));
            sb.Append("    /// FILE      : file://").AppendLine(Path.GetFullPath(path).Replace('\\', '/'));
            sb.AppendLine("    [UnityEngine.Scripting.Preserve]");
            sb.Append("    public class ").AppendLine(Utils.GetAllowMethodName(path));
            sb.AppendLine("    {");

            HashSet<string> methodNames = new(events.Count);
            foreach (var ev in events.OrderBy(v => v.TargetMethodName))
            {
                var eventMethodNameSource = Utils.GetAllowMethodName($"{ev.InternalPath}/{ev.PropertyPath}");

                var eventMethodName = eventMethodNameSource;
                for (int i = 0; !methodNames.Add(eventMethodName); i++)
                {
                    eventMethodName = $"{eventMethodNameSource}_{i}";
                }

                var callMethod = ev.TargetMethod;
                var callMethodName = callMethod?.Name ?? ev.TargetMethodName;

                //-------------------------------------------------------------------
                switch (ev.Type)
                {
                    case EventType.SendMessage:
                        sb.Append("        /// POSITION : ").AppendLine(ev.InternalPath);
                        sb.Append("        /// DECLARED : ").AppendLine(ev.PropertyPath);
                        break;
                    case EventType.UnityEvent:
                        sb.Append("        /// HIERARCHY : ").AppendLine(ev.InternalPath);
                        sb.Append("        /// PROPERTY  : ").AppendLine(ev.PropertyPath);
                        break;
                    case EventType.AnimationEvent:
                        sb.Append("        /// CLIP : ").AppendLine(ev.InternalPath);
                        sb.Append("        /// TIME : ").AppendLine(ev.PropertyPath);
                        break;
                }
                //-------------------------------------------------------------------
                var isMissing = !ev.HasTarget || callMethod == null;
                if (isMissing)
                {
                    if (!ev.HasTarget)
                    {
                        sb.AppendLine("        // !----- MISSING TARGET -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    }
                    else
                    {
                        sb.AppendLine("        // !----- MISSING METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    }
                    if (Store.Settings.IncludeMissing)
                    {
                        isMissing = false;
                    }
                }
                else if (callMethod!.IsPublic == false)
                {
                    sb.AppendLine("        // !----- INACCESSIBLE METHOD -----!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    if (!Store.Settings.IncludeInaccessible)
                    {
                        isMissing = true;
                    }
                }


                //-------------------------------------------------------------------
                sb.Append("        ");
                if (isMissing) sb.Append("//");
                sb.Append("public void ");
                sb.Append(eventMethodName);
                sb.Append("(");
                sb.Append(Utils.GetTypeName(ev.TargetType)).Append(" target");
                if (ev.TargetMethodParameters?.Length > 0)
                {
                    for (int i = 0; i < ev.TargetMethodParameters.Length; i++)
                    {
                        Type? paramType = ev.TargetMethodParameters[i];
                        sb.Append(", ")
                            .Append(Utils.GetTypeName(paramType))
                            .Append(" p").Append(i);
                    }
                }
                sb.AppendLine(")");

                //-------------------------------------------------------------------
                sb.Append("        ");
                if (isMissing) sb.Append("//");

                PropertyInfo? callProp = null;
                bool isGetProp = false;
                if (callMethod?.IsSpecialName ?? false)
                {
                    var targetType = ev.TargetType ?? callMethod.DeclaringType;

                    if (callMethodName.StartsWith("get_", StringComparison.Ordinal))
                    {
                        callProp = targetType.GetProperty(callMethodName[4..], BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty);
                        isGetProp = true;
                    }
                    else if (callMethodName.StartsWith("set_", StringComparison.Ordinal))
                    {
                        callProp = targetType.GetProperty(callMethodName[4..], BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty);
                    }
                }
                if (callProp != null)
                {
                    if (isGetProp)
                    {
                        sb.Append("    => _ = target.").Append(callProp.Name);
                        if (ev.TargetMethodParameters?.Length > 0)
                        {
                            sb.Append("[");
                            for (int i = 0; i < ev.TargetMethodParameters.Length; i++)
                            {
                                if (i != 0) sb.Append(", ");
                                sb.Append("p").Append(i);
                            }
                            sb.Append("]");
                        }

                        sb.AppendLine(";");
                    }
                    else
                    {
                        sb.Append("    => target.").Append(callProp.Name);

                        if (ev.TargetMethodParameters?.Length > 1)
                        {
                            sb.Append("[");
                            int i = 0;
                            for (; i < ev.TargetMethodParameters.Length; i++)
                            {
                                if (i != 0) sb.Append(", ");
                                sb.Append("p").Append(i);
                            }
                            sb.Append("] = p").Append(i).AppendLine(";");
                        }
                        else
                        {
                            sb.AppendLine(" = p0;");
                        }
                    }
                }
                else
                {
                    sb.Append("    => target.").Append(callMethodName).Append("(");
                    for (int i = 0; i < ev.TargetMethodParameters?.Length; i++)
                    {
                        if (i != 0) sb.Append(", ");
                        sb.Append("p").Append(i);
                    }
                    sb.AppendLine(");");
                }
                sb.AppendLine();
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}