/****************************************************
    文件：FileUtil.cs
	作者：TravelerTD
    日期：2019/8/21 9:40:24
	功能：文件工具类
*****************************************************/

using System;
using System.IO;
using UnityEngine;

namespace TDFramework {

    public class FileUtil : MonoBehaviour {

        /// <summary>
        /// 若路径不存在则创建文件夹
        /// </summary>
        /// <param name="path">路径</param>
        public static void CreateDirectory(string path) {
            if (!Directory.Exists(path)) {
                Directory.CreateDirectory(path);
            }
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        private static bool FileIsUsed(string path) {
            bool result = false;
            if (!File.Exists(path)) {
                result = false;
            }
            else {
                // 尝试用文件流打开文件，如果出异常说明文件已经被打开了
                FileStream fileStream = null;
                try {
                    fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    result = false;
                }
                catch (Exception e) {
                    Debug.LogError(e);
                    result = true;
                }
                finally {
                    if (fileStream != null) {
                        fileStream.Close();
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 复制指定文件夹下的所有文件到另一个文件夹
        /// </summary>
        /// <param name="srcPath">原路径</param>
        /// <param name="targetPath">目标路径</param>
        private static void CopyDirFiles(string srcPath, string targetPath) {
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
                        CopyDirFiles(file, scrdir); // 文件夹，递归
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
        /// 删除指定文件夹下的所有文件
        /// </summary>
        /// <param name="scrPath">文件夹路径</param>
        public static void DelDirFiles(string scrPath) {
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
}