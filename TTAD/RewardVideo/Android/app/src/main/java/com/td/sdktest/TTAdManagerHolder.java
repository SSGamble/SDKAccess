package com.td.sdktest;

import android.content.Context;

import com.bytedance.sdk.openadsdk.TTAdConfig;
import com.bytedance.sdk.openadsdk.TTAdConstant;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdSdk;

/**
 * 可以用一个单例来保存 TTAdManager 实例，在需要初始化 sdk 的时候调用
 */
public class TTAdManagerHolder {

    private static boolean sInit;

    public static TTAdManager get() {
        if (!sInit) {
            throw new RuntimeException("TTAdSdk is not init, please check.");
        }
        return TTAdSdk.getAdManager();
    }

    public static void init(Context context) {
        doInit(context);
    }

    // step1:接入网盟广告 sdk 的初始化操作，详情见接入文档和穿山甲平台说明
    private static void doInit(Context context) {
        if (!sInit) {
            TTAdSdk.init(context, buildConfig(context));
            sInit = true;
        }
    }

    private static TTAdConfig buildConfig(Context context) {
        return new TTAdConfig.Builder()
                .appId("5030830")
                .useTextureView(true) // 使用 TextureView 控件播放视频,默认为 SurfaceView,当有 SurfaceView 冲突的场景，可以使用 TextureView
                .appName("接入穿山甲广告测试")
                .titleBarTheme(TTAdConstant.TITLE_BAR_THEME_DARK)
                .allowShowNotify(true) //是否允许 sdk 展示通知栏提示
                .allowShowPageWhenScreenLock(true) // 是否在锁屏场景支持展示广告落地页
                .debug(true) // 测试阶段打开，可以通过日志排查问题，上线时去除该调用
                .directDownloadNetworkType(TTAdConstant.NETWORK_STATE_WIFI, TTAdConstant.NETWORK_STATE_3G) // 允许直接下载的网络状态集合
                .supportMultiProcess(false)
                .build();
    }
}