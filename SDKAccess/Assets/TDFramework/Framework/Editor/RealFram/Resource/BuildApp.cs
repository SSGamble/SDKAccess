/****************************************************
	文件：BuildApp.cs
	作者：TravelerTD
	日期：2019/08/19 15:09:05   	
	功能：Build 工程
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class BuildApp {
    private static string m_AppName = PlayerSettings.productName; // Unity PlayerSettings 中的 APP Name
    public static string m_AndroidPath = Application.dataPath + RealFrameConfig.buildPath + "Android/";
    public static string m_IOSPath = Application.dataPath + RealFrameConfig.buildPath + "IOS/";
    public static string m_WindowsPath = Application.dataPath + RealFrameConfig.buildPath + "/Windows/";

    [MenuItem("RealFram/Build 标准包")]
    public static void Build() {
        // 打 ab 包
        BundleEditor.Build();
        // 生成可执行程序
        string abPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString() + "/"; // ab包路径
        Copy(abPath, Application.streamingAssetsPath); // 将打好的 ab 包复制到当前项目中
        string savePath = ""; // 生成可执行程序的存储路径
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android) {
            savePath = m_AndroidPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now) + ".apk";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS) {
            savePath = m_IOSPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}", DateTime.Now);
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows || EditorUserBuildSettings.activeBuildTarget == BuildTarget.StandaloneWindows64) {
            savePath = m_WindowsPath + m_AppName + "_" + EditorUserBuildSettings.activeBuildTarget + string.Format("_{0:yyyy_MM_dd_HH_mm}/{1}.exe", DateTime.Now, m_AppName);
        }
        BuildPipeline.BuildPlayer(FindEnableEditorrScenes(), savePath, EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
        DeleteDir(Application.streamingAssetsPath); // 删除 ab 包
    }

    /// <summary>
    /// 查找已经加到 Build 的场景
    /// </summary>
    /// <returns></returns>
    private static string[] FindEnableEditorrScenes() {
        List<string> editorScenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes) {
            if (!scene.enabled)
                continue;
            editorScenes.Add(scene.path);
        }
        return editorScenes.ToArray();
    }

    /// <summary>
    /// 复制文件到另一个文件夹
    /// </summary>
    /// <param name="srcPath">原路径</param>
    /// <param name="targetPath">目标路径</param>
    private static void Copy(string srcPath, string targetPath) {
        try {
            if (!Directory.Exists(targetPath)) {
                Directory.CreateDirectory(targetPath);
            }
            string scrdir = Path.Combine(targetPath, Path.GetFileName(srcPath));
            if (Directory.Exists(srcPath))
                scrdir += Path.DirectorySeparatorChar;
            if (!Directory.Exists(scrdir)) {
                Directory.CreateDirectory(scrdir);
            }
            string[] files = Directory.GetFileSystemEntries(srcPath); // 原路径下的所有文件
            foreach (string file in files) {
                if (Directory.Exists(file)) {
                    Copy(file, scrdir); // 文件夹，递归
                }
                else {
                    File.Copy(file, scrdir + Path.GetFileName(file), true); // 文件，复制
                }
            }
        }
        catch {
            Debug.LogError("无法复制：" + srcPath + "  到" + targetPath);
        }
    }

    /// <summary>
    /// 删除文件夹下的所有文件
    /// </summary>
    /// <param name="scrPath"></param>
    public static void DeleteDir(string scrPath) {
        try {
            DirectoryInfo dir = new DirectoryInfo(scrPath);
            FileSystemInfo[] fileInfo = dir.GetFileSystemInfos();
            foreach (FileSystemInfo info in fileInfo) {
                if (info is DirectoryInfo) {
                    DirectoryInfo subdir = new DirectoryInfo(info.FullName);
                    subdir.Delete(true);
                }
                else {
                    File.Delete(info.FullName);
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(e);
        }
    }
}
