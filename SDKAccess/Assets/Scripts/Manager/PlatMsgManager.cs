/****************************************************
    文件：PlatMsgManager.cs
	作者：TravelerTD
    日期：2019/09/21 10:22:34
	功能：平台管理类，用于管理各个平台，方便之后扩展平台
*****************************************************/

using UnityEngine;
using TDFramework;
using System.Runtime.InteropServices;

public class PlatMsgManager : Singleton<PlatMsgManager> {
    GameObject platformObject = null; // 挂载 PlatformScript 的物体
    public PlatformScript platformScript = null; // 供底层调用的脚本

    public void Init() {
        if (platformObject == null) {
            platformObject = new GameObject("PlatformObject");
            platformObject.hideFlags = HideFlags.HideAndDontSave; // 隐藏且不能删除
            platformScript = platformObject.AddComponent<PlatformScript>();
#if UNITY_EDITOR
            if (!Application.isPlaying) {
                return;
            }
#endif
            GameObject.DontDestroyOnLoad(platformObject);
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        // 初始化 AndroidJavaClass
        gameHelperJavaClass = new AndroidJavaClass("com.td.sdkaccess.GameHelper"); // 指定类名
        TDLog.Log("Platform init com.td.sdkaccess.GameHelper");
#endif
    }


    #region 发送消息到平台
    public const int PLATFORM_MSG_QQLOGIN = 1; // QQ 登录
    public const int PLATFORM_MSG_QQLOGOUT = 2; // QQ 注销
    #endregion

    #region 从底层获取 long，获取当前 android 设备的可用内存
    /// <summary>
    /// 可用内存
    /// </summary>
    public const int UNITY_GET_LONG_AVAILABLEMEMORY = 1;
    /// <summary>
    /// 总共内存大小
    /// </summary>
    public const int UNITY_GET_LONG_TOTALMEMORY = 2;
    /// <summary>
    /// 应用运行内存大小
    /// </summary>
    public const int UNITY_GET_LONG_USEDMEMORY = 3;
    #endregion

#if UNITY_ANDROID && !UNITY_EDITOR
    #region Android 平台消息的发送和获取
    /// <summary>
    /// Unity 调用 Android
    /// </summary>
    AndroidJavaClass gameHelperJavaClass = null;

    /// <summary>
    /// 从 Unity 发送消息到 android 平台
    /// </summary>
    public void SendUnityMessageToPlatform(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "") {
        if (gameHelperJavaClass == null) return;
        // 调用 gameHelperJavaClass 类里的静态 Java 方法
        gameHelperJavaClass.CallStatic("SendUnityMessageToPlatform", iMsgId, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
    }

    /// <summary>
    /// 从平台获取 int 数据
    /// </summary>
    public int GetIntFromPlatform(int type) {
        if (gameHelperJavaClass == null) return 0;
        return gameHelperJavaClass.CallStatic<int>("GetIntFromPlatform", type);
    }

    /// <summary>
    /// 从平台获取 long 数据
    /// </summary>
    public long GetLongFromPlatform(int type) {
        if (gameHelperJavaClass == null) return 0;
        return gameHelperJavaClass.CallStatic<long>("GetLongFromPlatform", type);
    }

    /// <summary>
    /// 从平台获取 long 数据
    /// </summary>
    public long GetLongFromPlatform2(int type, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "") {
        if (gameHelperJavaClass == null) return 0;
        return gameHelperJavaClass.CallStatic<long>("GetLongFromPlatform2", type, iParam1, iParam2, iParam3, iParam4, strParam1, strParam2, strParam3, strParam4);
    }

    /// <summary>
    /// 从平台获取 string 数据
    /// </summary>
    public string GetStringFromPlatform(int type) {
        if (gameHelperJavaClass == null) return string.Empty;

        return gameHelperJavaClass.CallStatic<string>("GetStringFromPlatform", type);
    }
    #endregion
#else
    #region 其他平台消息的发送和获取
    /// <summary>
    /// 从 Unity 发送消息到其他平台
    /// </summary>
    public void SendUnityMessageToPlatform(int iMsgId, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "") {

    }

    /// <summary>
    /// 从平台获取 int 数据
    /// </summary>
    public int GetIntFromPlatform(int type) {
        return 0;
    }

    /// <summary>
    /// 从平台获取 long 数据
    /// </summary>
    public long GetLongFromPlatform(int type) {
        switch (type) {
            case UNITY_GET_LONG_AVAILABLEMEMORY:
                return (long)GetWinAvailMemory();
            case UNITY_GET_LONG_TOTALMEMORY:
                return (long)GetWinTotalMemory();
            case UNITY_GET_LONG_USEDMEMORY:
                return GetWinUsedMemory();
        }
        return 0;
    }

    /// <summary>
    /// 从平台获取 long 数据
    /// </summary>
    public long GetLongFromPlatform2(int type, int iParam1 = 0, int iParam2 = 0, int iParam3 = 0, int iParam4 = 0, string strParam1 = "", string strParam2 = "", string strParam3 = "", string strParam4 = "") {
        return 0;
    }

    /// <summary>
    /// 从平台获取 string 数据
    /// </summary>
    public string GetStringFromPlatform(int type) {
        return string.Empty;
    }
    #endregion
#endif

    #region Unitiy 获取 PC 内存数据
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct MEMORYSTATUSEX {
        public uint dwLength;
        public uint dwMemoryLoad;
        public ulong ullTotalPhys;
        public ulong ullAvailPhys;
        public ulong ullTotalPageFile;
        public ulong ullAvailPageFile;
        public ulong ullTotalVirtual;
        public ulong ullAvailVirtual;
        public ulong ullAvailExtendedVirtual;
    }

    // 调用其他平台 dll
    [DllImport("kernel32.dll")]
    protected static extern void GlobalMemoryStatus(ref MEMORYSTATUSEX lpBuff);

    /// <summary>
    /// 获取可用内存
    /// </summary>
    /// <returns></returns>
    protected ulong GetWinAvailMemory() {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullAvailPhys;
    }

    /// <summary>
    /// 获取总内存
    /// </summary>
    /// <returns></returns>
    protected ulong GetWinTotalMemory() {
        MEMORYSTATUSEX ms = new MEMORYSTATUSEX();
        ms.dwLength = 64;
        GlobalMemoryStatus(ref ms);
        return ms.ullTotalPhys;
    }

    /// <summary>
    /// 返回应用使用内存
    /// </summary>
    /// <returns></returns>
    protected long GetWinUsedMemory() {
        return UnityEngine.Profiling.Profiler.GetTotalReservedMemoryLong();
    }
    #endregion

}