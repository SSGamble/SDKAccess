/****************************************************
    文件：LevelManager.cs
	作者：TravelerTD
    日期：2019/8/7 17:31:53
	功能：关卡管理
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TDFramework {
    public class LevelManager {
        private static List<string> mLevelNames; // 关卡名列表，体现了关卡的连接关系，用于配置关卡顺序
        public static int Index { get; set; } // 场景索引

        public static void Init(List<string> levelNames) {
            Index = 0;
            mLevelNames = levelNames;
        }

        /// <summary>
        /// 加载当前关卡
        /// </summary>
        public static void LoadCurrent() {
            SceneManager.LoadScene(mLevelNames[Index]);
        }

        /// <summary>
        /// 加载下一关卡
        /// </summary>
        public static void LoadNext() {
            Index++;
            // 容错处理
            if (Index >= mLevelNames.Count) {
                Index = 0;
            }
            SceneManager.LoadScene(mLevelNames[Index]);
        }
    }
}