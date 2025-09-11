#nullable enable

using System;
using System.Reflection;

namespace Sions.EventLinker
{
    public enum EventType
    {
        UnityEvent,
        AnimationEvent,
        SendMessage
    }

    public abstract class EventEntryBase
    {
        public EventType Type;

        public bool HasTarget;
        public Type? TargetType;
        public string TargetMethodName = null!;
        public MethodInfo? TargetMethod;
        public Type[]? TargetMethodParameters;

        public string FilePath { get; set; } = "";
        public string InternalPath { get; set; } = "";
        public string PropertyPath = "";
    }
}