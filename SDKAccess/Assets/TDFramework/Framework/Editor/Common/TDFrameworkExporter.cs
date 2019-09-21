/****************************************************
    文件：Exporter.cs
	作者：TravelerTD
    日期：2019/8/6 12:1:21
	功能：导出器,导出 UnityPackage，以时间自动命名（精确到分钟），并可以在导出后自动打开文件所在的文件夹
            eg：TDFramework_20190804_1823
*****************************************************/

using UnityEditor;
using System;
using UnityEngine;
using System.IO;

namespace TDFramework {

    public class TDFrameworkExporter : MonoBehaviour {

        /// <summary>
        /// 以当前时间获取⽂件名
        /// </summary>
        /// <returns></returns>
        private static string GenerateUnityPackageName() {
            return "TDFramework_" + DateTime.Now.ToString("yyyyMMdd_hhmm");
        }

        /// <summary>
        /// 导出 UnityPackage，以时间自动命名（精确到分钟），并在导出后自动打开文件所在的文件夹，eg：TDFramework_20190804_1823
        /// </summary>
        //[MenuItem("TDFramework/导出 UnityPackage")]
        public static void ExportPackage() {
            var assetPath = "Assets";
            EditorUtil.ExportPackage(assetPath, TDFrameworkExporter.GenerateUnityPackageName());
        }

        /// <summary>
        /// 导出 UnityPackage 并打开输出文件夹
        /// </summary>
        [MenuItem("TDFramework/导出 UnityPackage 并打开文件夹 %e", false, 1)]
        public static void ExportPackageAndOpen() {
            //EditorUtil.CallMenuItem("TDFramework/导出 UnityPackage"); // 执行指定菜单
            var assetPath = "Assets";
            EditorUtil.ExportPackage(assetPath, TDFrameworkExporter.GenerateUnityPackageName());
            EditorUtil.OpenInFolder("file:///" + Path.Combine(Application.dataPath, "../")); // 打开 Assets 的上一级文件夹
        }
    }
}
