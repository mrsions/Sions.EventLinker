#nullable enable

using System;

using UnityEditor;

namespace Sions.EventLinker
{
    /// <summary>
    /// 프로세서의 기본 추상 클래스입니다.
    /// </summary>
    public abstract class ProcessorBase
    {
        public EventGenerator Store { get; set; }  // 이벤트 생성기 참조
        public string Name { get; }  // 프로세서 이름
        public virtual int Order { get; }  // 실행 순서

        public int ItemCount { get; set; }  // 전체 아이템 개수
        public int ItemIndex { get; set; }  // 현재 아이템 인덱스
        public int PhaseIndex { get; set; }  // 현재 페이즈 인덱스
        public int PhaseCount { get; set; }  // 전체 페이즈 개수

        /// <summary>
        /// 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public ProcessorBase(EventGenerator generator)
        {
            Store = generator;
            Name = GetType().Name.Replace("Processor", "");
        }

        /// <summary>
        /// 프로세서 작업을 수행합니다.
        /// </summary>
        public abstract void Process();

        /// <summary>
        /// 진행 대화상자를 표시합니다.
        /// </summary>
        /// <param name="path">현재 처리 중인 경로</param>
        protected void ShowDialogue(string path = "")
        {
            string title = $"UnityEventLinker ({PhaseIndex}/{PhaseCount})";

            string message = $"{Name}({ItemIndex}/{ItemCount}) {path}";

            if (EditorUtility.DisplayCancelableProgressBar(title, message, (float)ItemIndex / ItemCount))
            {
                throw new OperationCanceledException();
            }
        }

        /// <summary>
        /// 디버그 메시지를 출력합니다.
        /// </summary>
        /// <param name="obj">출력할 객체</param>
        internal void Print(object obj)
        {
            UnityEngine.Debug.Log($"[{Name}] {obj}");
        }
    }
}
