#nullable enable

using System.Collections.Generic;

using UnityEditor;

namespace Sions.EventLinker
{
    /// <summary>
    /// 파일 검색 프로세서의 기본 추상 클래스입니다.
    /// </summary>
    public abstract class FindProcessorBase: ProcessorBase
    {
        public string guid = "";  // 현재 처리 중인 GUID
        public string path = "";  // 현재 처리 중인 경로

        /// <summary>
        /// 검색 프로세서를 초기화합니다.
        /// </summary>
        /// <param name="generator">이벤트 생성기</param>
        public FindProcessorBase(EventGenerator generator):base(generator)
        {
        }

        /// <summary>
        /// 검색 작업을 수행합니다.
        /// </summary>
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

        /// <summary>
        /// 검색할 아이템 목록을 가져옵니다.
        /// </summary>
        /// <returns>GUID 목록</returns>
        protected abstract List<string> GetItems();

        /// <summary>
        /// 파일을 열어 처리합니다.
        /// </summary>
        protected abstract void Open();

        /// <summary>
        /// 경로가 유효한지 검증합니다.
        /// </summary>
        /// <param name="path">검증할 경로</param>
        /// <returns>유효 여부</returns>
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
