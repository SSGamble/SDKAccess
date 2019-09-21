/****************************************************
    文件：HomeModule.cs
	作者：TravelerTD
    日期：2019/8/7 10:33:20
	功能：主⻚模块
*****************************************************/

using Game;
using TDFramework;
using UnityEngine;

namespace Home {
    public class HomeModule : MainManager {

        protected override void LaunchInDevelopingMode() {
            // 开发逻辑
            GameModule.LoadModule();
        }

        protected override void LaunchInTestMode() {
            // 测试逻辑
            // 加载资源
            // 初始化 SDK
            // 点击开始游戏
            GameModule.LoadModule();
        }

        protected override void LaunchInProductionMode() {
            // ⽣产逻辑
            // 加载资源
            // 初始化 SDK
            // 点击开始游戏
            GameModule.LoadModule();
        }
    }
}