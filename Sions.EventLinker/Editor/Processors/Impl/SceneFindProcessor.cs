#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.SceneManagement;

using UnityEngine;
using UnityEngine.SceneManagement;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Sions.EventLinker
{
    /// <summary>
    /// Scene 파일을 검색하고 처리하는 프로세서입니다.
    /// </summary>
    public class SceneFindProcessor : FindProcessorBase
    {
        private Dictionary<string, PackageInfo>? m_PackageList;  // 패키지 정보 목록

        private GameObjectSolver m_Solver;  // GameObject 솔버

        public override int Order => 1000;  // 실행 순서 (높은 값 = 나중 실행)

        /// <summary>
        /// Scene 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public SceneFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<GameObjectSolver>();
            m_PackageList = GetPackagesAsync();
        }


        /// <summary>
        /// Scene 검색 작업을 수행합니다.
        /// </summary>
        public override void Process()
        {
            if (!Store.Settings.IncludeScene) return;
            base.Process();
        }

        /// <summary>
        /// Scene 파일 목록을 가져옵니다.
        /// </summary>
        /// <returns>Scene GUID 목록</returns>
        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:scene").ToList();
        }

        /// <summary>
        /// Scene 파일을 열어 GameObject를 처리합니다.
        /// </summary>
        protected override void Open()
        {
            Scene? scene = null;
            try
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

                Store.CurrentFileName = path;
                Store.CurrentInternalPath = "";

                // 루트 GameObject들을 순회하며 처리
                foreach (var go in scene.Value.GetRootGameObjects())
                {
                    m_Solver.Solve(go);
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                if (scene.HasValue)
                {
                    EditorSceneManager.UnloadSceneAsync(scene.Value);
                }
            }
        }

        /// <summary>
        /// 경로가 유효한지 검증합니다. (패키지 경로 추가 검증)
        /// </summary>
        /// <param name="path">검증할 경로</param>
        /// <returns>유효 여부</returns>
        protected override bool ValidPath(string path)
        {
            if (!base.ValidPath(path)) return false;

            // 패키지 경로인 경우 추가 검증
            if (path.StartsWith("Packages/"))
            {
                var packageName = path.Split('/')[1];
                if (!m_PackageList!.TryGetValue(packageName, out var package))
                {
                    return false;
                }

                // Local 또는 Embedded 패키지만 허용
                if (package.source != PackageSource.Local
                    && package.source != PackageSource.Embedded)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 패키지 목록을 비동기로 가져옵니다.
        /// </summary>
        /// <returns>패키지 정보 딕셔너리</returns>
        private Dictionary<string, PackageInfo> GetPackagesAsync()
        {
            var req = UnityEditor.PackageManager.Client.List();
            while (!req.IsCompleted)
            {
                Thread.Sleep(1);
            }

            return req.Result.ToDictionary(v => v.name);
        }
    }
}
