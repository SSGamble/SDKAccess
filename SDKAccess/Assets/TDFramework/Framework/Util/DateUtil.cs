/****************************************************
    文件：DateUtil.cs
	作者：TravelerTD
    日期：2019/08/21 09:50:01
	功能：日期/时间 工具类
*****************************************************/

using System;
using System.Globalization;
using UnityEngine;

namespace TDFramework {

    public class DateUtil : MonoBehaviour {

        /// <summary>
        /// 获取当前时间，2019/08/21 09:50:01
        /// </summary>
        /// <returns></returns>
        public static string DateTimeNow() {
            string format = "yyyy/MM/dd HH:mm:ss";
            string date = DateTime.Now.ToString(format, DateTimeFormatInfo.InvariantInfo);
            return date;
        }
    }
}