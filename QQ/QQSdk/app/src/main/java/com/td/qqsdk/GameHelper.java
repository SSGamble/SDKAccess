package com.td.qqsdk;

import android.app.ActivityManager;
import android.app.ActivityManager.MemoryInfo;
import android.content.Context;
import android.os.Debug;
import android.util.Log;

import com.unity3d.player.UnityPlayer;

import org.json.JSONException;
import org.json.JSONObject;

import java.io.IOException;
import java.io.RandomAccessFile;

/**
 * 供给 Unity 调用 Android 的接口
 */
public class GameHelper {
    public final static int PLATFORM_MSG_QQLOGINCALLBACK = 1; // QQ 登录回调
    public final static int PLATFORM_MSG_QQLOGIN = 1; // QQ 登录
    public final static int PLATFORM_MSG_QQLOGOUT = 2; // QQ 注销

    private static MainActivity activity = null;
    private static String platformObject = "PlatformObject"; // Unity 用于通信的物体
    private static String methodName = "OnMessage"; // Unity 用于接收通信的方法名称
    public static String TAG = "GameHelper"; // Log 标签

    /**
     * 初始化方法
     */
    public static void Init(MainActivity mainActivity) {
        activity = mainActivity;
    }

    /**
     * android 发送消息到 unity
     */
    public static void SendPlatformMessageToUnity(int iMsgId, int iParam1, int iParam2, int iParam3, String strParam1, String strParam2, String strParam3) {
        String jsonString = GetJsonStr(iMsgId, iParam1, iParam2, iParam3, strParam1, strParam2, strParam3);
        UnityPlayer.UnitySendMessage(platformObject, methodName, jsonString);
    }

    /**
     * 接收 unity 发送到 android 的消息
     */
    public static void SendUnityMessageToPlatform(int iMsgId, int iParam1, int iParam2, int iParam3, int iParam4, String strParam1, String strParam2, String strParam3, String strParam4) {
        Log.d(TAG, "SendUnityMessageToPlatform: iMsgId：" + iMsgId + " iParam1:" + iParam1 + " iParam2:" + iParam2 + " iParam3:" + iParam3 + " iParam4:" + iParam4 + " strParam1:" + strParam1 + " strParam2:" + strParam2 + " strParam3:" + strParam3 + " strParam4:" + strParam4);
        if (activity == null) {
            Log.e(TAG, "SendUnityMessageToPlatform：activity is null");
        }
        switch (iMsgId) {
            case PLATFORM_MSG_QQLOGIN:
                TencentQQ.Login();
                break;
            case PLATFORM_MSG_QQLOGOUT:
                TencentQQ.Logout();
                break;
        }
    }

    /**
     * 从 android 获取 Int 类型
     */
    public static int GetIntFromPlatform(int type) {
        switch (type) {

        }
        return 0;
    }

    /**
     * 从 android 获取 string 类型
     */
    public static String GetStringFromPlatform(int type) {
        switch (type) {
            case 1: // QQ 授权是否过期
                return String.valueOf(TencentQQ.CheckAutorVaild());
            case 2: // 刷新 QQ 票据
                return TencentQQ.RefreshSession().toString();
        }
        return "";
    }

    /**
     * 从 android 获取 long 类型
     */
    public static long GetLongFromPlatform(int type) {
        switch (type) {
            case 1: // 当前系统可以内存
                ActivityManager am = (ActivityManager) activity.getSystemService(Context.ACTIVITY_SERVICE);
                MemoryInfo mi = new MemoryInfo();
                am.getMemoryInfo(mi);
                return mi.availMem;
            case 2: // 总内存
                return GetTotalMemory();
            case 3: // 应用内存
                Debug.MemoryInfo memoryInfo = new Debug.MemoryInfo();
                Debug.getMemoryInfo(memoryInfo);
                return memoryInfo.getTotalPss() * 1024;
        }
        return 0;
    }

    /**
     * 从 android 获取 long 类型
     */
    public static long GetLongFromPlatform2(int type, int iParam1, int iParam2, int iParam3, int iParam4, String strParam1, String strParam2, String strParam3, String strParam4) {
        switch (type) {

        }
        return 0;
    }

    /**
     * 通过 json 对象构造字符串
     */
    public static String GetJsonStr(int iMsgId, int iParam1, int iParam2, int iParam3, String strParam1, String strParam2, String strParam3) {
        try {
            JSONObject object = new JSONObject();
            object.put("iMsgId", iMsgId);
            object.put("iPararm1", iParam1);
            object.put("iPararm2", iParam2);
            object.put("iPararm3", iParam3);
            object.put("strParam1", strParam1);
            object.put("strParam2", strParam2);
            object.put("strParam3", strParam3);
            return object.toString();
        } catch (JSONException e) {
            Log.e(TAG, "GetJsonStr：Json error" + e.toString());
            return "";
        }
    }

    /**
     * 获取设备总内存
     */
    protected static long GetTotalMemory() {
        long tm = 0;
        try {
            RandomAccessFile reader = new RandomAccessFile("/proc/meminfo", "r");
            String load = reader.readLine();
            reader.close();
            String[] totrm = load.split("kB");
            String[] trm = totrm[0].split("");
            tm = Long.parseLong(trm[trm.length - 1]) * 1024;
        } catch (IOException ex) {
            ex.printStackTrace();
        }
        return tm;
    }
}

