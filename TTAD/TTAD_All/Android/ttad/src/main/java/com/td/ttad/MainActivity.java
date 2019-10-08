package com.td.ttad;

import android.content.Context;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;
import android.widget.TextView;
import android.widget.Toast;
import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdConstant;
import com.bytedance.sdk.openadsdk.TTAdDislike;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTAppDownloadListener;
import com.bytedance.sdk.openadsdk.TTBannerAd;
import com.bytedance.sdk.openadsdk.TTInteractionAd;
import com.bytedance.sdk.openadsdk.TTRewardVideoAd;
import com.unity3d.player.UnityPlayerActivity;

public class MainActivity extends UnityPlayerActivity {
    private static final String TAG = "MainActivity";

    // 广告接口持有者
    private TTAdNative mTTAdNative;
    // 视频播放持有者
    private TTRewardVideoAd mTTRewardVideoAd;
    // 是否加载完成
    private boolean mHasShowDownloadActive = false;

    // Banner 广告
    private FrameLayout mFLBannerContainer;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        TTAdManagerHolder.init(this); // Unity 启动活动的时候，初始化 SDK
        Init(); // 初始化 广告 SDK
        newFLBannerContainer();
    }

    // 初始化广告配置
    public void Init() {
        mTTAdNative = TTAdManagerHolder.get().createAdNative(this);
        //step3:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
        TTAdManagerHolder.get().requestPermissionIfNecessary(this);

//        //step1:初始化sdk
//        TTAdManager ttAdManager = TTAdManagerHolder.get();
//        //step2:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
//        TTAdManagerHolder.get().requestPermissionIfNecessary(this);
//        //step3:创建 TTAdNative 对象,用于调用广告请求接口
//        mTTAdNative = ttAdManager.createAdNative(this);
    }

    // TToast 显示信息
    public void TToast(final Context context, final String msg) {
        //在Android的UI线程显示
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(context, msg, Toast.LENGTH_SHORT).show();
            }
        });
    }

    // --------------------------------------- Banner 广告 start -----------------------------------
    private void LoadBannerAd() {
        TToast(MainActivity.this, "加载 Banner 广告...");
//        newFLBannerContainer();
        loadBannerAd("901121987");
    }

    /**
     * 动态创建 FrameLayout 作为 Banner 广告的容器
     */
    public void newFLBannerContainer(){
        mFLBannerContainer = new FrameLayout(this); // 创建帧布局对象layout
        FrameLayout.LayoutParams frameLayout = new FrameLayout.LayoutParams( // 设置帧布局的高宽属性
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        );
        FrameLayout.LayoutParams viewPream = new FrameLayout.LayoutParams( // 设置布局控件的属性
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        );
        TextView txt = new TextView(this); // 创建 TextView 控件
        txt.setText("Banner 广告 占位"); // 设置文字
        mFLBannerContainer.addView(txt, viewPream); // 添加进 FrameLayout 里
        super.addContentView(mFLBannerContainer, frameLayout);// 显示布局管理器
    }

    /**
     * 加载 Banner 广告
     */
    private void loadBannerAd(String codeId) {
        //step4:创建广告请求参数 AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(codeId) //广告位id
                .setSupportDeepLink(true)
                .setImageAcceptedSize(600, 300)
                .build();
        //step5:请求广告，对请求回调的广告作渲染处理
        mTTAdNative.loadBannerAd(adSlot, new TTAdNative.BannerAdListener() {
            @Override
            public void onError(int code, String message) {
                TToast(MainActivity.this, "load error : " + code + ", " + message);
                mFLBannerContainer.removeAllViews();
            }

            @Override
            public void onBannerAdLoad(final TTBannerAd ad) {
                if (ad == null) {
                    return;
                }
                View bannerView = ad.getBannerView();
                if (bannerView == null) {
                    return;
                }
                //设置轮播的时间间隔  间隔在30s到120秒之间的值，不设置默认不轮播
                ad.setSlideIntervalTime(30 * 1000);
                mFLBannerContainer.removeAllViews();
                mFLBannerContainer.addView(bannerView);
                //设置广告互动监听回调
                ad.setBannerInteractionListener(new TTBannerAd.AdInteractionListener() {
                    @Override
                    public void onAdClicked(View view, int type) {
                        TToast(MainActivity.this, "广告被点击");
                    }

                    @Override
                    public void onAdShow(View view, int type) {
                        TToast(MainActivity.this, "广告展示");
                    }
                });
                //（可选）设置下载类广告的下载监听
                bindDownloadListener(ad);
                //在banner中显示网盟提供的dislike icon，有助于广告投放精准度提升
                ad.setShowDislikeIcon(new TTAdDislike.DislikeInteractionCallback() {
                    @Override
                    public void onSelected(int position, String value) {
                        TToast(MainActivity.this, "点击 " + value);
                        //用户选择不喜欢原因后，移除广告展示
                        mFLBannerContainer.removeAllViews();
                    }

                    @Override
                    public void onCancel() {
                        TToast(MainActivity.this, "点击取消 ");
                    }
                });
            }
        });
    }

    /**
     * Banner 下载监听
     */
    private void bindDownloadListener(TTBannerAd ad) {
        ad.setDownloadListener(new TTAppDownloadListener() {
            @Override
            public void onIdle() {
                TToast(MainActivity.this, "点击图片开始下载");
            }

            @Override
            public void onDownloadActive(long totalBytes, long currBytes, String fileName, String appName) {
                if (!mHasShowDownloadActive) {
                    mHasShowDownloadActive = true;
                    TToast(MainActivity.this, "下载中，点击图片暂停");
                }
            }

            @Override
            public void onDownloadPaused(long totalBytes, long currBytes, String fileName, String appName) {
                TToast(MainActivity.this, "下载暂停，点击图片继续");
            }

            @Override
            public void onDownloadFailed(long totalBytes, long currBytes, String fileName, String appName) {
                TToast(MainActivity.this, "下载失败，点击图片重新下载");
            }

            @Override
            public void onInstalled(String fileName, String appName) {
                TToast(MainActivity.this, "安装完成，点击图片打开");
            }

            @Override
            public void onDownloadFinished(long totalBytes, String fileName, String appName) {
                TToast(MainActivity.this, "点击图片安装");
            }
        });
    }
    // --------------------------------------- Banner 广告 end -------------------------------------

    // ---------------------------------------- 插屏广告 start -------------------------------------

    public void LoadInteractionAd(){
        loadInteractionAd("901121725");
    }

    /**
     * 加载插屏广告
     */
    private void loadInteractionAd(String codeId) {
        //step4:创建插屏广告请求参数AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(codeId)
                .setSupportDeepLink(true)
                .setImageAcceptedSize(600, 600) //根据广告平台选择的尺寸，传入同比例尺寸
                .build();
        //step5:请求广告，调用插屏广告异步请求接口
        mTTAdNative.loadInteractionAd(adSlot, new TTAdNative.InteractionAdListener() {
            @Override
            public void onError(int code, String message) {
                TToast(MainActivity.this, "code: " + code + "  message: " + message);
            }

            @Override
            public void onInteractionAdLoad(TTInteractionAd ttInteractionAd) {
                TToast(MainActivity.this, "type:  " + ttInteractionAd.getInteractionType());
                ttInteractionAd.setAdInteractionListener(new TTInteractionAd.AdInteractionListener() {
                    @Override
                    public void onAdClicked() {
                        Log.d(TAG, "被点击");
                        TToast(MainActivity.this, "广告被点击");
                    }

                    @Override
                    public void onAdShow() {
                        Log.d(TAG, "被展示");
                        TToast(MainActivity.this, "广告被展示");
                    }

                    @Override
                    public void onAdDismiss() {
                        Log.d(TAG, "插屏广告消失");
                        TToast(MainActivity.this, "广告消失");
                    }
                });
                //如果是下载类型的广告，可以注册下载状态回调监听
                if (ttInteractionAd.getInteractionType() == TTAdConstant.INTERACTION_TYPE_DOWNLOAD) {
                    ttInteractionAd.setDownloadListener(new TTAppDownloadListener() {
                        @Override
                        public void onIdle() {
                            Log.d(TAG, "点击开始下载");
                            TToast(MainActivity.this, "点击开始下载");
                        }

                        @Override
                        public void onDownloadActive(long totalBytes, long currBytes, String fileName, String appName) {
                            Log.d(TAG, "下载中");
                            TToast(MainActivity.this, "下载中");
                        }

                        @Override
                        public void onDownloadPaused(long totalBytes, long currBytes, String fileName, String appName) {
                            Log.d(TAG, "下载暂停");
                            TToast(MainActivity.this, "下载暂停");
                        }

                        @Override
                        public void onDownloadFailed(long totalBytes, long currBytes, String fileName, String appName) {
                            Log.d(TAG, "下载失败");
                            TToast(MainActivity.this, "下载失败");
                        }

                        @Override
                        public void onDownloadFinished(long totalBytes, String fileName, String appName) {
                            Log.d(TAG, "下载完成");
                            TToast(MainActivity.this, "下载完成");
                        }

                        @Override
                        public void onInstalled(String fileName, String appName) {
                            Log.d(TAG, "安装完成");
                            TToast(MainActivity.this, "安装完成");
                        }
                    });
                }
                //弹出插屏广告
                ttInteractionAd.showInteractionAd(MainActivity.this);
            }
        });
    }

    // ---------------------------------------- 插屏广告 end -------------------------------------

    // --------------------------------------- 激励视频 start --------------------------------------
    // 加载（缓存）横屏广告
    public void LoadHorizontalAD() {
        loadAd("901121430", TTAdConstant.HORIZONTAL);
    }

    // 加载（缓存）竖屏广告
    public void LoadVerticalAD() {
        loadAd("901121365", TTAdConstant.VERTICAL);
    }

    // 加载完成，显示广告内容
    public void ShowVedio() {
        //在Android的UI线程显示
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                if (mTTRewardVideoAd != null) {
                    //step6:在获取到广告后展示
                    TToast(MainActivity.this, "加载成功显示");
                    mTTRewardVideoAd.showRewardVideoAd(MainActivity.this);
                    TToast(MainActivity.this, "显示调用完成");
                    mTTRewardVideoAd = null;
                } else {
                    TToast(MainActivity.this, "请先加载广告");
                }
            }
        });
    }

    // 配置广告配置，请求广告
    private void loadAd(String codeId, int orientation) {
        //step4:创建广告请求参数AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(codeId)
                .setSupportDeepLink(true)
                .setImageAcceptedSize(1080, 1920)
                .setRewardName("金币") //奖励的名称
                .setRewardAmount(3)  //奖励的数量
                .setUserID("user123")//用户id,必传参数
                .setMediaExtra("media_extra") //附加参数，可选
                .setOrientation(orientation) //必填参数，期望视频的播放方向：TTAdConstant.HORIZONTAL 或 TTAdConstant.VERTICAL
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
                mTTRewardVideoAd = ad;
//                mTTRewardVideoAd.setShowDownLoadBar(false);
                mTTRewardVideoAd.setRewardAdInteractionListener(new TTRewardVideoAd.RewardAdInteractionListener() {
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

                    //视频播放完成回调
                    @Override
                    public void onVideoComplete() {
                        TToast(MainActivity.this, "rewardVideoAd complete");
                    }

                    @Override
                    public void onVideoError() {
                        TToast(MainActivity.this, "rewardVideoAd error");
                    }

                    //视频播放完成后，奖励验证回调，rewardVerify：是否有效，rewardAmount：奖励梳理，rewardName：奖励名称
                    @Override
                    public void onRewardVerify(boolean rewardVerify, int rewardAmount, String rewardName) {
                        TToast(MainActivity.this, "verify:" + rewardVerify + " amount:" + rewardAmount +
                                " name:" + rewardName);
                    }
                });
                mTTRewardVideoAd.setDownloadListener(new TTAppDownloadListener() {
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
    // --------------------------------------- 激励视频 end ----------------------------------------

}
