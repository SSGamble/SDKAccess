package com.td.sdktest;

import android.content.Context;
import android.content.Intent;
import android.os.Bundle;
import android.widget.Toast;
import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdConstant;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTAppDownloadListener;
import com.bytedance.sdk.openadsdk.TTRewardVideoAd;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {

    // 广告接口持有者
    private TTAdNative mTTAdNative;
    // 视频播放持有者
    private TTRewardVideoAd mttRewardVideoAd;
    // 是否加载完成
    private boolean mHasShowDownloadActive = false;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        if (!isTaskRoot()) {
            Intent intent = getIntent();
            String action = intent.getAction();
            if (intent.hasCategory(Intent.CATEGORY_LAUNCHER) && Intent.ACTION_MAIN.equals(action)) {
                finish();
                return;
            }
        }
        TTAdManagerHolder.init(this); // Unity 启动活动的时候，初始化 SDK
    }

    //初始化广告配置
    public void Init() {
        // step1:初始化 sdk
        TTAdManager ttAdManager = TTAdManagerHolder.get();
        // step2:(可选，强烈建议在合适的时机调用):申请部分权限，如 read_phone_state,防止获取不了 IMEI 时候，下载类广告没有填充的问题。
        TTAdManagerHolder.get().requestPermissionIfNecessary(this);
        // step3:创建 TTAdNative 对象,用于调用广告请求接口
        mTTAdNative = ttAdManager.createAdNative(getApplicationContext());
    }

    // 加载（缓存）横屏广告
    public void LoadHorizontalAD() {
        loadAd("930830162", TTAdConstant.HORIZONTAL);
    }

    // 加载（缓存）竖屏广告
    public void LoadVerticalAD() {
        loadAd("930830893", TTAdConstant.VERTICAL);
    }

    // 加载完成，显示广告内容
    public void ShowVedio() {
        //在Android的UI线程显示
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (mttRewardVideoAd != null) {
                    //step6:在获取到广告后展示
                    TToast(MainActivity.this, "加载成功显示");
                    mttRewardVideoAd.showRewardVideoAd(MainActivity.this);
                    TToast(MainActivity.this, "显示调用完成");
                    mttRewardVideoAd = null;
                } else {
                    TToast(MainActivity.this, "请先加载广告");
                }
            }
        });
    }

    // TToast 显示信息
    public void TToast(final Context context, final String msg) {
        // 在 Android 的 UI 线程显示
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(context, msg, Toast.LENGTH_SHORT).show();
            }
        });
    }

    // 配置广告配置，请求广告
    private void loadAd(String codeId, int orientation) {
        // step4:创建广告请求参数 AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(codeId)
                .setSupportDeepLink(true)
                .setImageAcceptedSize(1080, 1920)
                .setRewardName("金币") // 奖励的名称
                .setRewardAmount(3)  // 奖励的数量
                .setUserID("15592")// 用户id,必传参数
                .setMediaExtra("media_extra") // 附加参数，可选
                .setOrientation(orientation) // 必填参数，期望视频的播放方向：TTAdConstant.HORIZONTAL 或 TTAdConstant.VERTICAL
                .build();
        //step5:请求广告
        mTTAdNative.loadRewardVideoAd(adSlot, new TTAdNative.RewardVideoAdListener() {
            @Override
            public void onError(int code, String message) {
                TToast(MainActivity.this, message);
            }
            //视频广告加载后，视频资源缓存到本地的回调，在此回调后，播放本地视频，流畅不阻塞。
            @Override
            public void onRewardVideoCached() {
                TToast(MainActivity.this, "rewardVideoAd video cached");
            }
            //视频广告的素材加载完毕，比如视频url等，在此回调后，可以播放在线视频，网络不好可能出现加载缓冲，影响体验。
            @Override
            public void onRewardVideoAdLoad(TTRewardVideoAd ad) {
                TToast(MainActivity.this, "rewardVideoAd loaded");
                mttRewardVideoAd = ad;
//                mttRewardVideoAd.setShowDownLoadBar(false);
                mttRewardVideoAd.setRewardAdInteractionListener(new TTRewardVideoAd.RewardAdInteractionListener() {
                    @Override
                    public void onAdShow() {
                        TToast(MainActivity.this, "rewardVideoAd show");
                    }
                    @Override
                    public void onAdVideoBarClick() {
                        TToast(MainActivity.this, "rewardVideoAd bar click");
                    }
                    @Override
                    public void onAdClose() {
                        TToast(MainActivity.this, "rewardVideoAd close");
                    }
                    // 视频播放完成回调
                    @Override
                    public void onVideoComplete() {
                        TToast(MainActivity.this, "rewardVideoAd complete");
                    }
                    @Override
                    public void onVideoError() {
                        TToast(MainActivity.this, "rewardVideoAd error");
                    }
                    // 视频播放完成后，奖励验证回调，rewardVerify：是否有效，rewardAmount：奖励梳理，rewardName：奖励名称
                    @Override
                    public void onRewardVerify(boolean rewardVerify, int rewardAmount, String rewardName) {
                        TToast(MainActivity.this, "verify:" + rewardVerify + " amount:" + rewardAmount +
                                " name:" + rewardName);
                    }
                });
                mttRewardVideoAd.setDownloadListener(new TTAppDownloadListener() {
                    @Override
                    public void onIdle() {
                        mHasShowDownloadActive = false;
                    }
                    @Override
                    public void onDownloadActive(long totalBytes, long currBytes, String fileName, String appName) {
                        if (!mHasShowDownloadActive) {
                            mHasShowDownloadActive = true;
                            TToast(MainActivity.this, "下载中，点击下载区域暂停");
                        }
                    }
                    @Override
                    public void onDownloadPaused(long totalBytes, long currBytes, String fileName, String appName) {
                        TToast(MainActivity.this, "下载暂停，点击下载区域继续");
                    }
                    @Override
                    public void onDownloadFailed(long totalBytes, long currBytes, String fileName, String appName) {
                        TToast(MainActivity.this, "下载失败，点击下载区域重新下载");
                    }
                    @Override
                    public void onDownloadFinished(long totalBytes, String fileName, String appName) {
                        TToast(MainActivity.this, "下载完成，点击下载区域重新下载");
                    }
                    @Override
                    public void onInstalled(String fileName, String appName) {
                        TToast(MainActivity.this, "安装完成，点击下载区域打开");
                    }
                });
            }
        });
    }
}
