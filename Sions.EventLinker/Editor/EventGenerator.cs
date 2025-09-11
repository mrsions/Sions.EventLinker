#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Sions.EventLinker
{
    public class EventGenerator
    {
        public List<ProcessorBase> Processors = new();
        public List<object> Solvers = new();
        public EventLinkerSettings Settings;

        private HashSet<object> m_Insepctions = new(1000);
        public HashSet<Type> IgnoreComponentType { get; } = new();
        public List<EventEntryBase> Events { get; } = new();

        public string CurrentFileName { get; internal set; } = "";
        public string CurrentInternalPath { get; internal set; } = "";

        public EventGenerator()
        {
            Settings = EventLinkerSettings.Load();

            AddIgnoreTypes(typeof(Transform).Assembly);
            AddIgnoreTypes(typeof(Canvas).Assembly);
            AddIgnoreTypes(typeof(AudioListener).Assembly);
            AddIgnoreTypes(typeof(Animator).Assembly);
            AddIgnoreTypes(typeof(Cloth).Assembly);
            AddIgnoreTypes(typeof(ParticleSystem).Assembly);
            AddIgnoreTypes(typeof(UIBehaviour).Assembly
                , excludes: new Type[] { typeof(UIBehaviour), typeof(EventTrigger) }
                , includes: new Type[] { typeof(BaseRaycaster), typeof(LayoutGroup) });

            IgnoreComponentType.Add(typeof(CanvasScaler));
            IgnoreComponentType.Add(typeof(EventSystem));
            IgnoreComponentType.Add(typeof(LayoutElement));

            var processors = typeof(ProcessorBase).Assembly.GetTypes()
                .Where(v => !v.IsAbstract && typeof(ProcessorBase).IsAssignableFrom(v));

            foreach (var procType in processors)
            {
                GetProcessor(procType);
            }

            Processors.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        public void Generate()
        {
            try
            {
                for (int i = 0; i < Processors.Count; i++)
                {
                    var procesosr = Processors[i];
                    procesosr.PhaseIndex = i;
                    procesosr.PhaseCount = Processors.Count;
                    procesosr.Process();
                }
            }
            catch (OperationCanceledException)
            {
                Debug.LogError("Cancelled");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private void AddIgnoreTypes(Assembly asm, Type[]? excludes = null, Type[]? includes = null)
        {
            foreach (var type in asm.GetTypes().Where(type => typeof(Component).IsAssignableFrom(type)))
            {
                if (includes != null && includes.Any(t => t.IsAssignableFrom(type)))
                {
                    // FORCE
                }
                else if (excludes != null && excludes.Any(t => t.IsAssignableFrom(type)))
                {
                    continue;
                }

                IgnoreComponentType.Add(type);
            }
        }

        internal ProcessorBase GetProcessor(Type type)
        {
            foreach (var proc in Processors)
            {
                if (type.IsInstanceOfType(proc))
                {
                    return proc;
                }
            }

            var result = (ProcessorBase)Activator.CreateInstance(type, this);
            Processors.Add(result);
            return result;
        }

        internal T GetProcessor<T>()
            where T : ProcessorBase
        {
            return (T)GetProcessor(typeof(T));

        }

        internal void AddEvent(EventEntryBase result)
        {
            result.FilePath = CurrentFileName;
            result.InternalPath = CurrentInternalPath;
            Events.Add(result);
        }

        internal bool AddInspection(UnityEngine.Object obj)
        {
            return m_Insepctions.Add(obj);
        }

        internal T GetSolver<T>()
        {
            foreach (var proc in Solvers)
            {
                if (proc is T rst)
                {
                    return rst;
                }
            }

            var result = (T)Activator.CreateInstance(typeof(T), this);
            Solvers.Add(result);
            return result;
        }
    }
}