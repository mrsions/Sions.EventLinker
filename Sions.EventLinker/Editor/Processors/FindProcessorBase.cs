#nullable enable

using System.Collections.Generic;

using UnityEditor;

namespace Sions.EventLinker
{
    public abstract class FindProcessorBase: ProcessorBase
    {
        public string guid = "";
        public string path = "";

        public FindProcessorBase(EventGenerator generator):base(generator)
        {
        }

        public override void Process()
        {
            var guidList = GetItems();
            Print($"GetItems({guidList.Count})");

            ItemCount = guidList.Count;
            ItemIndex = 0;
            ShowDialogue("");

            foreach (var guid in guidList)
            {
                this.path = AssetDatabase.GUIDToAssetPath(guid);
                this.guid = guid;

                ItemIndex++;
                ShowDialogue(path);

                if (ValidPath(path))
                {
                    Print($"Open({path})");
                    Open();
                }
            }
        }

        protected abstract List<string> GetItems();

        protected abstract void Open();

        protected virtual bool ValidPath(string path)
        {
            if (!Store.Settings.IncludePackage)
            {
                if (path.StartsWith("Packages/"))
                {
                    return false;
                }
            }
            return true;
        }
    }
}