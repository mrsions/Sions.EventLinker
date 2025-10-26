#nullable enable

using System;
using System.Reflection;

namespace Sions.EventLinker
{
    /// <summary>
    /// 이벤트 타입을 정의합니다.
    /// </summary>
    public enum EventType
    {
        UnityEvent,         // Unity Event 타입
        AnimationEvent,     // Animation Event 타입
        SendMessage         // SendMessage 타입
    }

    /// <summary>
    /// 이벤트 엔트리의 기본 클래스입니다.
    /// </summary>
    public abstract class EventEntryBase
    {
        public EventType Type;  // 이벤트 타입

        public bool HasTarget;  // 타겟 존재 여부
        public Type? TargetType;  // 타겟 타입
        public string TargetMethodName = null!;  // 타겟 메서드 이름
        public MethodInfo? TargetMethod;  // 타겟 메서드 정보
        public Type[]? TargetMethodParameters;  // 타겟 메서드 파라미터 타입들

        public string FilePath { get; set; } = "";  // 파일 경로
        public string InternalPath { get; set; } = "";  // 내부 경로
        public string PropertyPath = "";  // 프로퍼티 경로
    }
}
