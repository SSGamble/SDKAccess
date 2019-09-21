/****************************************************
    文件：TDLog.cs
	作者：TravelerTD
    笔记：429
    日期：2019/08/27 10:32:21
	功能：日志工具
    示例：
            // 1. 使用前必须针对使用平台进行初始化
            TDLog.InitLog(LogType.Unity); // 初始化日志输出接口
            // 2. 输出
            TDLog.Log("Info Log");
            TDLog.LogWarn("Warn Log");
            TDLog.LogError("Error Log");
*****************************************************/

using System;
using UnityEngine;

namespace TDFramework {

    /// <summary>
    /// 日志级别
    /// </summary>
    public enum LogLevel {
        Info = 0,
        Warn = 1,
        Error = 2
    }

    /// <summary>
    /// 日志输出平台
    /// </summary>
    public enum LogType {
        Unity = 0,
        Console = 1,
    }

    public class TDLog : MonoBehaviour {

        /// <summary>
        /// 日志开关
        /// </summary>
        private static bool isOpenLog = true;
        /// <summary>
        /// 回调，自定义日志输出接口，每个平台不一样
        /// </summary>
        private static Action<string, int> logCB = null;

        /// <summary>
        /// 初始化日志输出接口
        /// </summary>
        /// <param name="lt"></param>
        public static void InitLog(LogType lt = LogType.Unity) {
            switch (lt) {
                case LogType.Unity:
                    InitUnity();
                    break;
                case LogType.Console:
                    InitConsole();
                    break;
                default:
                    InitUnity();
                    break;
            }
        }

        /// <summary>
        /// Unity 编辑器下的日志输出接口
        /// </summary>
        private static void InitUnity() {
            TDLog.SetLog(isOpenLog, (string msg, int lv) => {
                switch (lv) {
                    case 0:
                        msg = "Info: " + msg;
                        Debug.Log(msg);
                        break;
                    case 1:
                        msg = "Warn: " + msg;
                        Debug.LogWarning(msg);
                        break;
                    case 2:
                        msg = "Error: " + msg;
                        Debug.LogError(msg);
                        break;
                }
            });
        }

        /// <summary>
        /// Console 控制台下的日志输出接口
        /// </summary>
        private static void InitConsole() {
            TDLog.SetLog(true, (string msg, int lv) => {
                switch (lv) {
                    case 0:
                        msg = "Info:" + msg;
                        Console.WriteLine("//--------------------Info--------------------//");
                        Console.WriteLine(msg);
                        break;
                    case 1:
                        msg = "Warn:" + msg;
                        Console.WriteLine("//--------------------Warn--------------------//");
                        Console.WriteLine(msg);
                        break;
                    case 2:
                        msg = "Error:" + msg;
                        Console.WriteLine("//--------------------Error--------------------//");
                        Console.WriteLine(msg);
                        break;
                    default:
                        Console.WriteLine("//--------------------Error--------------------//");
                        Console.WriteLine(msg + " >> Unknow Log Type\n");
                        break;
                }
            });
        }

        /// <summary>
        /// 设置日志
        /// </summary>
        /// <param name="log">日志开关</param>
        /// <param name="logCB">回调，自定义日志输出接口</param>
        private static void SetLog(bool log = true, Action<string, int> logCB = null) {
            if (log == false) {
                TDLog.isOpenLog = false;
            }
            if (logCB != null) {
                TDLog.logCB = logCB;
            }
        }

        #region 打印日志，按日志级别封装
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="msg">日志内容</param>
        public static void Log(string msg) {
            if (isOpenLog != true) {
                return;
            }
            // 添加时间标记
            msg = DateUtil.DateTimeNow() + " >>> " + msg;
            // 是否自行设置输出接口
            if (logCB != null) {
                logCB(msg, (int)LogLevel.Info);
            }
            // 默认日志输出接口
            else {
                Debug.LogError(msg + " >> Unknow Log Type\n");
            }
        }

        public static void LogWarn(string msg) {
            if (isOpenLog != true) {
                return;
            }
            msg = DateUtil.DateTimeNow() + " >>> " + msg;
            if (logCB != null) {
                logCB(msg, (int)LogLevel.Warn);
            }
            else {
                Debug.LogError(msg + " >> Unknow Log Type\n");
            }
        }

        public static void LogError(string msg) {
            if (isOpenLog != true) {
                return;
            }
            msg = DateUtil.DateTimeNow() + " >>> " + msg;
            if (logCB != null) {
                logCB(msg, (int)LogLevel.Error);
            }
            else {
                Debug.LogError(msg + " >> Unknow Log Type\n");
            }
        }
        #endregion
    }
}