/****************************************************
    文件：CommonUtil.cs
	作者：TravelerTD
    日期：2019/8/6 12:0:43
	功能：通用工具
*****************************************************/

using UnityEngine;

namespace TDFramework {

    public partial class CommonUtil : MonoBehaviour {

        /// <summary>
        /// 复制⽂本到剪切板
        /// </summary>
        /// <param name="text"></param>
        public static void CopyText(string text) {
            GUIUtility.systemCopyBuffer = text;
        }

        /// <summary>
        /// 判断是否为模拟器，根据是否存在光传感器来进行判断
        /// </summary>
        /// <returns></returns>
        public static bool GetIsSimulator() {
#if UNITY_ANDROID && !UNITY_EDITOR
            //AndroidJavaObject sensorManager = currentActivity.Call<AndroidJavaObject>("getSystemService", "sensor");
            //AndroidJavaObject sensor = sensorManager.Call<AndroidJavaObject>("getDefaultSensor", 5); // 光传感器
            //if (sensor == null) {
            //    return true;
            //}
            //else {
            //    return false;
            //}
#endif
            return false;
        }
    }
}