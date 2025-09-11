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
    public class SceneFindProcessor : FindProcessorBase
    {
        private Dictionary<string, PackageInfo>? m_PackageList;

        private GameObjectSolver m_Solver;

        public override int Order => 1000;

        public SceneFindProcessor(EventGenerator generator) : base(generator)
        {
            m_Solver = generator.GetSolver<GameObjectSolver>();
            m_PackageList = GetPackagesAsync();
        }


        public override void Process()
        {
            if (!Store.Settings.IncludeScene) return;
            base.Process();
        }

        protected override List<string> GetItems()
        {
            return AssetDatabase.FindAssets("t:scene").ToList();
        }

        protected override void Open()
        {
            Scene? scene = null;
            try
            {
                scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

                Store.CurrentFileName = path;
                Store.CurrentInternalPath = "";

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

        protected override bool ValidPath(string path)
        {
            if (!base.ValidPath(path)) return false;

            if (path.StartsWith("Packages/"))
            {
                var packageName = path.Split('/')[1];
                if (!m_PackageList!.TryGetValue(packageName, out var package))
                {
                    return false;
                }

                if (package.source != PackageSource.Local
                    && package.source != PackageSource.Embedded)
                {
                    return false;
                }
            }
            return true;
        }

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