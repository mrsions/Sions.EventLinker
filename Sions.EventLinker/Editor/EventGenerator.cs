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
    /// <summary>
    /// 이벤트 생성기 클래스입니다.
    /// </summary>
    public class EventGenerator
    {
        public List<ProcessorBase> Processors = new();  // 프로세서 목록
        public List<object> Solvers = new();  // 솔버 목록
        public EventLinkerSettings Settings;  // 이벤트 링커 설정

        private HashSet<object> m_Insepctions = new(1000);  // 검사 객체 해시셋
        public HashSet<Type> IgnoreComponentType { get; } = new();  // 무시할 컴포넌트 타입 목록
        public List<EventEntryBase> Events { get; } = new();  // 이벤트 엔트리 목록

        public string CurrentFileName { get; internal set; } = "";  // 현재 파일명
        public string CurrentInternalPath { get; internal set; } = "";  // 현재 내부 경로

        /// <summary>
        /// 이벤트 생성기를 초기화합니다.
        /// </summary>
        public EventGenerator()
        {
            Settings = EventLinkerSettings.Load();

            // 무시할 타입 추가
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

            // 프로세서 초기화
            var processors = typeof(ProcessorBase).Assembly.GetTypes()
                .Where(v => !v.IsAbstract && typeof(ProcessorBase).IsAssignableFrom(v));

            foreach (var procType in processors)
            {
                GetProcessor(procType);
            }

            Processors.Sort((a, b) => a.Order.CompareTo(b.Order));
        }

        /// <summary>
        /// 이벤트 생성을 실행합니다.
        /// </summary>
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

        /// <summary>
        /// 무시할 타입들을 어셈블리에서 추가합니다.
        /// </summary>
        /// <param name="asm">어셈블리</param>
        /// <param name="excludes">제외할 타입 배열</param>
        /// <param name="includes">포함할 타입 배열</param>
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

        /// <summary>
        /// 프로세서를 가져오거나 생성합니다.
        /// </summary>
        /// <param name="type">프로세서 타입</param>
        /// <returns>프로세서 인스턴스</returns>
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

        /// <summary>
        /// 제네릭 타입으로 프로세서를 가져옵니다.
        /// </summary>
        /// <typeparam name="T">프로세서 타입</typeparam>
        /// <returns>프로세서 인스턴스</returns>
        internal T GetProcessor<T>()
            where T : ProcessorBase
        {
            return (T)GetProcessor(typeof(T));

        }

        /// <summary>
        /// 이벤트를 추가합니다.
        /// </summary>
        /// <param name="result">이벤트 엔트리</param>
        internal void AddEvent(EventEntryBase result)
        {
            result.FilePath = CurrentFileName;
            result.InternalPath = CurrentInternalPath;
            Events.Add(result);
        }

        /// <summary>
        /// 검사 대상 객체를 추가합니다.
        /// </summary>
        /// <param name="obj">Unity 객체</param>
        /// <returns>추가 성공 여부</returns>
        internal bool AddInspection(UnityEngine.Object obj)
        {
            return m_Insepctions.Add(obj);
        }

        /// <summary>
        /// 솔버를 가져오거나 생성합니다.
        /// </summary>
        /// <typeparam name="T">솔버 타입</typeparam>
        /// <returns>솔버 인스턴스</returns>
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
