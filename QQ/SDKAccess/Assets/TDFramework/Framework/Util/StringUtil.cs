/****************************************************
    文件：StringUtil.cs
	作者：TravelerTD
    日期：2019/08/27 16:45:52
	功能：字符串工具类
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class StringUtil : MonoBehaviour {
        /// <summary>
        /// 复制⽂本到剪切板
        /// </summary>
        /// <param name="text">需要复制的文本</param>
        public static void CopyText(string text) {
            GUIUtility.systemCopyBuffer = text;
        }
    }
}