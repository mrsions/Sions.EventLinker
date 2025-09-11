#nullable enable

using System;
using System.IO;
using System.Text;

using UnityEngine;

namespace Sions.EventLinker
{
    [Serializable]
    public class EventLinkerSettings
    {
        const string k_SettingsPath = "ProjectSettings/Sions/EventLinker.json";

        public static EventLinkerSettings Load()
        {
            try
            {
                if (File.Exists(k_SettingsPath))
                {
                    string json = File.ReadAllText(k_SettingsPath, Encoding.UTF8);

                    return JsonUtility.FromJson<EventLinkerSettings>(json);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            return new();
        }

        public static void Save(EventLinkerSettings settings)
        {
            string json = JsonUtility.ToJson(settings);

            Directory.CreateDirectory(Path.GetDirectoryName(k_SettingsPath));

            File.WriteAllText(k_SettingsPath, json, Encoding.UTF8);
        }


        [Header("General")]
        [Tooltip("true일 경우 에디터가 실행될 때, generate를 실행한다.")]
        public bool LinkingOnEditorStartup = false;

        [Tooltip("ture일 경우 package의 내용물도 검사합니다. false일 경우 package의 내용물은 검사하지않습니다.")]
        public bool IncludePackage = true;

        [Tooltip("입력된 위치에 코드가 생성된다.")]
        public string GenerateFolder = "Assets/UnityEvents";

        [Tooltip("true일 경우 링크 코드 생성 전에 'GenerateFolder'의 내용을 지운다.")]
        public bool ClearBeforeGeneration = true;


        [Header("Links")]
        public bool IncludeAnimator = true;
        public bool IncludePrefab= true;
        public bool IncludeScene= true;
        public bool IncludeScriptableObject= true;
        public bool IncludeAnimation= true;
        public bool IncludeSendMessage= true;


        [Header("Validation (If set true, You can has compilation error.)")]
        [Tooltip("true일 경우 object가 missing이거나 method가 missing이더라도 소스코드에 포함한다. false일 경우 주석으로 소스코드에 포함된다.")]
        public bool IncludeMissing = false; // true일 경우 missing 이벤트도 포함시킨다.

        [Tooltip("true일 경우 메서드가 public이 아니더라도 소스코드에 포함한다. false일 경우 주석으로 소스코드에 포함된다.")]
        public bool IncludeInaccessible = false; // true일 경우 missing 이벤트도 포함시킨다.


    }
}