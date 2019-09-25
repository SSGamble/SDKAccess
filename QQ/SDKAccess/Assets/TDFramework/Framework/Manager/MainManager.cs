/****************************************************
    文件：MainManager.cs
	作者：TravelerTD
    日期：2019/8/7 10:29:37
	功能：⼊⼝管理器
*****************************************************/

using UnityEngine;

namespace TDFramework {
    /// <summary>
    /// 生产环境
    /// </summary>
    public enum EnvironmentMode {
        Developing, // 开发阶段
        Test, // 测试阶段
        Production // 发布阶段：上线
    }

    public abstract class MainManager : MonoBehaviour {

        public EnvironmentMode Mode; // 对外可更改的 Mode
        private static EnvironmentMode mSharedMode; // 当前设置的 Mode
        private static bool mModeSetted = false; // 是否设置过 Mode

        private void Start() {
            if (!mModeSetted) {
                mSharedMode = Mode;
                mModeSetted = true;
            }
            switch (mSharedMode) {
                case EnvironmentMode.Developing:
                    LaunchInDevelopingMode();
                    break;
                case EnvironmentMode.Test:
                    LaunchInTestMode();
                    break;
                case EnvironmentMode.Production:
                    LaunchInProductionMode();
                    break;
            }
        }

        /// <summary>
        /// 开发逻辑
        /// </summary>
        protected abstract void LaunchInDevelopingMode();
        /// <summary>
        /// 发布逻辑
        /// </summary>
        protected abstract void LaunchInProductionMode();
        /// <summary>
        /// 测试逻辑
        /// </summary>
        protected abstract void LaunchInTestMode();
    }
}