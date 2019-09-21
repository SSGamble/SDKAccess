/****************************************************
    文件：EditorUtil.cs
	作者：TravelerTD
    日期：2019/8/6 12:2:42
	功能：编辑器⼯具
*****************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TDFramework {

    public partial class EditorUtil{

#if UNITY_EDITOR
        /// <summary>
        /// MenuItem 复⽤（调用）
        /// </summary>
        /// <param name="menuPath"></param>
        public static void CallMenuItem(string menuPath) {
            EditorApplication.ExecuteMenuItem(menuPath);
        }

        /// <summary>
        /// 打开指定⽂件夹
        /// </summary>
        /// <param name="folderPath"></param>
        public static void OpenInFolder(string folderPath) {
            Application.OpenURL("file:///" + folderPath);
        }

        /// <summary>
        /// 导出 UnityPackage
        /// </summary>
        /// <param name="assetPathName"></param>
        /// <param name="fileName"></param>
        public static void ExportPackage(string assetPathName, string fileName) {
            AssetDatabase.ExportPackage(assetPathName, fileName,
            ExportPackageOptions.Recurse);
        }
#endif
    }
}