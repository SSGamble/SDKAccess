/****************************************************
    文件：GameModule.cs
	作者：TravelerTD
    日期：2019/8/7 10:32:35
	功能：游戏模块的⼊⼝
*****************************************************/

using TDFramework;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Game {
    public class GameModule : MainManager {

        public static void LoadModule() {
            SceneManager.LoadScene("Game");
        }

        protected override void LaunchInDevelopingMode() {
            // 开发逻辑
            // 加载资源
            // 初始化 SDK
            // Game 的⼀些准备逻辑 （⻆⾊选择、准备⼀些假的数据等等)
            Debug.Log("开发逻辑");
        }

        protected override void LaunchInTestMode() {
            // 正常的 测试逻辑
            Debug.Log("测试逻辑");
        }

        protected override void LaunchInProductionMode() {
            // 正常的 ⽣产逻辑
            Debug.Log("⽣产逻辑");
        }
    }
}