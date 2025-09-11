#nullable enable

using System;
using System.IO;

using UnityEditor;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Sions.EventLinker
{
    public class EventLinkerSettingsProvider : SettingsProvider
    {
        private const string k_MenuPath = "Project/Sions/Event Link Generator";

        public class SettingObject : ScriptableObject
        {
            public EventLinkerSettings setting;
        }

        [SettingsProvider]
        public static SettingsProvider CreateEventLinkerSettingProvider()
        {
            return new EventLinkerSettingsProvider(k_MenuPath, SettingsScope.Project);
        }

        private SettingObject? m_Settings;
        private SerializedObject? m_SettingsObject;

        public EventLinkerSettingsProvider(string path, SettingsScope scope)
            : base(path, scope)
        {
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            m_Settings = ScriptableObject.CreateInstance<SettingObject>();
            m_Settings.setting = EventLinkerSettings.Load();
            m_SettingsObject = new SerializedObject(m_Settings);
        }

        public override void OnDeactivate()
        {
            base.OnDeactivate();

            if (m_Settings != null) ScriptableObject.DestroyImmediate(m_Settings);
            m_SettingsObject?.Dispose();
        }

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