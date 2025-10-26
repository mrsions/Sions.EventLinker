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
    /// <summary>
    /// 이벤트 링크 코드를 생성하고 파일로 저장하는 프로세서
    /// </summary>
    public class SaveProcessor : ProcessorBase
    {
        /// <summary>
        /// SaveProcessor 생성자
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public SaveProcessor(EventGenerator generator) : base(generator)
        {
        }

        /// <summary>
        /// 프로세서 실행 순서 (가장 마지막에 실행)
        /// </summary>
        public override int Order => int.MaxValue;

        /// <summary>
        /// 이벤트 코드를 생성하고 파일로 저장하는 메인 처리 로직
        /// </summary>
        public override void Process()
        {
            ItemCount = 1;

            ShowDialogue("Save CS Code");

            // 생성 폴더 생성 및 기존 파일 삭제
            Directory.CreateDirectory(Store.Settings.GenerateFolder);
            foreach (var file in Directory.GetFiles(Store.Settings.GenerateFolder))
            {
                try { File.Delete(file); } catch { }
            }

            // 파일 경로별로 이벤트 그룹화 및 코드 생성
            var groups = Store.Events.GroupBy(v => v.FilePath);
            foreach (var group in groups)
            {
                string code = BuildCode(group.Key, group.ToList());
                File.WriteAllText($"{Store.Settings.GenerateFolder}/{Utils.GetAllowMethodName(group.Key)}.cs", code);
            }

            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 이벤트 목록으로부터 C# 코드를 생성
        /// </summary>
        /// <param name="path">파일 경로</param>
        /// <param name="events">이벤트 목록</param>
        /// <returns>생성된 C# 코드</returns>
        private string BuildCode(string path, List<EventEntryBase> events)
        {
            var sb = new StringBuilder();

            // 네임스페이스 및 클래스 선언
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
                // 메서드 이름 중복 방지 처리
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