#nullable enable

using System;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Sions.EventLinker
{
    /// <summary>
    /// 이벤트 링커 설정 프로바이더 클래스입니다.
    /// </summary>
    public class EventLinkerSettingsProvider : SettingsProvider
    {
        private const string k_MenuPath = "Project/Sions/Event Link Generator";

        /// <summary>
        /// 설정 객체를 담는 ScriptableObject입니다.
        /// </summary>
        public class SettingObject : ScriptableObject
        {
            public EventLinkerSettings setting;
        }

        /// <summary>
        /// 설정 프로바이더를 생성합니다.
        /// </summary>
        /// <returns>SettingsProvider 인스턴스</returns>
        [SettingsProvider]
        public static SettingsProvider CreateEventLinkerSettingProvider()
        {
            return new EventLinkerSettingsProvider(k_MenuPath, SettingsScope.Project);
        }

        private SettingObject? m_Settings;  // 설정 객체
        private SerializedObject? m_SettingsObject;  // Serialize된 설정 객체

        /// <summary>
        /// 설정 프로바이더를 초기화합니다.
        /// </summary>
        /// <param name="path">설정 경로</param>
        /// <param name="scope">설정 범위</param>
        public EventLinkerSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        {
        }

        /// <summary>
        /// 설정 창이 활성화될 때 호출됩니다.
        /// </summary>
        /// <param name="searchContext">검색 컨텍스트</param>
        /// <param name="rootElement">루트 엘리먼트</param>
        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_Settings = ScriptableObject.CreateInstance<SettingObject>();
            m_Settings.setting = EventLinkerSettings.Load();
            m_SettingsObject = new SerializedObject(m_Settings);
        }

        /// <summary>
        /// 설정 창이 비활성화될 때 호출됩니다.
        /// </summary>
        public override void OnDeactivate()
        {
            base.OnDeactivate();

            if (m_Settings != null) ScriptableObject.DestroyImmediate(m_Settings);
            m_SettingsObject?.Dispose();
        }

        /// <summary>
        /// 설정 GUI를 그립니다.
        /// </summary>
        /// <param name="searchContext">검색 컨텍스트</param>
        public override void OnGUI(string searchContext)
        {
            base.OnGUI(searchContext);

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical();

            OnDrawLayout();

            EditorGUILayout.EndVertical();
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 설정 레이아웃을 그립니다.
        /// </summary>
        private void OnDrawLayout()
        {
            Assert.IsNotNull(m_Settings);
            Assert.IsNotNull(m_SettingsObject);

            var prop = m_SettingsObject!.FindProperty(nameof(SettingObject.setting));
            int baseDepth = prop.depth;
            bool enterChildren = true;
            while (prop.NextVisible(enterChildren) && prop.depth > baseDepth)
            {
                EditorGUILayout.PropertyField(prop);
            }
            m_SettingsObject.ApplyModifiedProperties();


            if (EditorGUI.EndChangeCheck())
            {
                EventLinkerSettings.Save(m_Settings!.setting);
            }

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(1, true);
            if (GUILayout.Button("Generate", GUILayout.Width(150)))
            {
                new EventGenerator().Generate();
            }
            if (GUILayout.Button("Clear", GUILayout.Width(100)))
            {
                Clear();
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 생성된 파일들을 삭제합니다.
        /// </summary>
        private void Clear()
        {
            Assert.IsNotNull(m_Settings);

            var folder = m_Settings!.setting.GenerateFolder;

            if (!Directory.Exists(folder))
            {
                return;
            }

            const int retryCount = 5;
            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    Directory.Delete(m_Settings!.setting.GenerateFolder, true);
                    break;
                }
                catch (Exception ex)
                {
                    if (i + 1 == retryCount)
                    {
                        Debug.LogException(ex);
                    }
                }
            }

            try
            {
                File.Delete(m_Settings!.setting.GenerateFolder + ".meta");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            AssetDatabase.Refresh();
        }
    }
}