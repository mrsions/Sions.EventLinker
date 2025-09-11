#nullable enable

using System;

using UnityEditor;

namespace Sions.EventLinker
{
    public abstract class ProcessorBase
    {
        public EventGenerator Store { get; set; }
        public string Name { get; }
        public virtual int Order { get; }

        public int ItemCount { get; set; }
        public int ItemIndex { get; set; }
        public int PhaseIndex { get; set; }
        public int PhaseCount { get; set; }

        public ProcessorBase(EventGenerator generator)
        {
            Store = generator;
            Name = GetType().Name.Replace("Processor", "");
        }

        public abstract void Process();

        protected void ShowDialogue(string path = "")
        {
            string title = $"UnityEventLinker ({PhaseIndex}/{PhaseCount})";

            string message = $"{Name}({ItemIndex}/{ItemCount}) {path}";

            if (EditorUtility.DisplayCancelableProgressBar(title, message, (float)ItemIndex / ItemCount))
            {
                throw new OperationCanceledException();
            }
        }

        internal void Print(object obj)
        {
            UnityEngine.Debug.Log($"[{Name}] {obj}");
        }
    }
}