/****************************************************
    文件：ResolutionCheck.cs
	作者：TravelerTD
    日期：2019/8/6 10:14:59
	功能：分辨率检测
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class ResolutionCheck : MonoBehaviour {

        #region 测试
//#if UNITY_EDITOR
//        [MenuItem("TDFramework/8.计算是否是 Pad 分辨率")]
//#endif
//        private static void MenuClicked() {
//            Debug.Log(IsPadResolution() ? "是 Pad 分辨率" : "不是 Pad 分辨率");
//            Debug.Log(IsPhoneResolution() ? "是 Phone 分辨率" : "不是 Phone 分辨率");
//            Debug.Log(IsiPhoneXResolution() ? "是 iPhone X 分辨率" : "不是 iPhone X 分辨率");
//        }
        #endregion
                
        /// <summary>
        /// 获取屏幕宽⾼⽐
        /// </summary>
        public static float GetAspectRatio() {
            return Screen.width > Screen.height ? (float)Screen.width / Screen.height : (float)Screen.height / Screen.width;
        }

        /// <summary>
        /// 分辨率是否在目标范围内
        /// </summary>
        /// <param name="dstAspectRatio">范围边界</param>
        public static bool IsAspectRange(float dstAspectRatio) {
            var aspect = GetAspectRatio();
            return aspect > (dstAspectRatio) && aspect < (dstAspectRatio);
        }

        /// <summary>
        /// 计算是否是 Pad 分辨率
        /// </summary>
        public static bool IsPadResolution() {
            var aspect = GetAspectRatio(); // 获取宽高比
            // 不管是将 Game 视图的分辨率设置成 4:3 还是 3:4，输出的宽⾼⽐值都是 1.333 左右，⽽这个宽⾼⽐值的⼩数点第三位都有⼀点浮动。所以要在判断是否是 Pad 分辨率的时候要把这个浮动范围考虑进去。
            return aspect > (4.0f / 3 - 0.05) && aspect < (4.0f / 3 + 0.05); // 在浮动范围内
        }

        /// <summary>
        /// 是否是⼿机分辨率 16:9
        /// </summary>
        public static bool IsPhoneResolution() {
            var aspect = GetAspectRatio();
            return aspect > 16.0f / 9 - 0.05 && aspect < 16.0f / 9 + 0.05;
        }

        /// <summary>
        /// 是否是iPhone X 分辨率 2436:112565
        /// </summary>
        public static bool IsiPhoneXResolution() {
            var aspect = GetAspectRatio();
            return aspect > 2436.0f / 1125 - 0.05 && aspect < 2436.0f / 1125 +
            0.05;
        }
    }
}