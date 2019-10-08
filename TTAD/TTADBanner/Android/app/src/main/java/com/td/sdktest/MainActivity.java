package com.td.sdktest;

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

import androidx.appcompat.app.AppCompatActivity;

public class MainActivity extends AppCompatActivity {
    private static final String TAG = "MainActivity";
    //广告接口持有者
    private TTAdNative mTTAdNative;
    //视频播放持有者
    private TTRewardVideoAd mttRewardVideoAd;
    //是否加载完成
    private boolean mHasShowDownloadActive = false;

    private FrameLayout mBannerContainer;
//    private Button mButtonLandingPage;
//    private Button mButtonInterstitialAd;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
//        setContentView(R.layout.activity_main);
        TTAdManagerHolder.init(this); // Unity启动活动的时候，初始化SDK
        //step2:创建TTAdNative对象，createAdNative(Context context) banner广告context需要传入Activity对象
        mTTAdNative = TTAdManagerHolder.get().createAdNative(this);
        //step3:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
        TTAdManagerHolder.get().requestPermissionIfNecessary(this);

//        Init();

//        mBannerContainer = (FrameLayout) findViewById(R.id.banner_container);

        mBannerContainer = new FrameLayout(this);//创建帧布局对象layout
        FrameLayout.LayoutParams frameLayout = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.MATCH_PARENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        );//设置帧布局的高宽属性
        FrameLayout.LayoutParams viewPream = new FrameLayout.LayoutParams(
                ViewGroup.LayoutParams.WRAP_CONTENT,
                ViewGroup.LayoutParams.WRAP_CONTENT
        );//设置布局控件的属性
        TextView txt = new TextView(this);//创建TextView控件
        txt.setText("Hello World");//设置文字
        mBannerContainer.addView(txt, viewPream);
        super.addContentView(mBannerContainer, frameLayout);//显示布局管理器


//        mButtonLandingPage = (Button) findViewById(R.id.btnLoadBanner);
//        mButtonLandingPage.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                bannerLandingpage();
//            }
//        });
//        mButtonInterstitialAd = (Button) findViewById(R.id.btnLoadInteraction);
//        mButtonInterstitialAd.setOnClickListener(new View.OnClickListener() {
//            @Override
//            public void onClick(View view) {
//                loadInteractionAd("901121725");
//            }
//        });


        bannerLandingpage();
    }

    //初始化广告配置
    public void Init() {
        //step1:初始化sdk
        TTAdManager ttAdManager = TTAdManagerHolder.get();
        //step2:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
        TTAdManagerHolder.get().requestPermissionIfNecessary(this);
        //step3:创建 TTAdNative 对象,用于调用广告请求接口
        mTTAdNative = ttAdManager.createAdNative(getApplicationContext());
    }

    private void bannerLandingpage() {
        loadBannerAd("901121987");
    }

    /**
     * 加载 Banner 广告
     */
    private void loadBannerAd(String codeId) {
        //step4:创建广告请求参数AdSlot,具体参数含义参考文档
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
                mBannerContainer.removeAllViews();
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
                mBannerContainer.removeAllViews();
                mBannerContainer.addView(bannerView);
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
                        mBannerContainer.removeAllViews();
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

    /**
     * TToast 显示信息
     */
    public void TToast(final Context context, final String msg) {
        //在 Android 的 UI 线程显示
        runOnUiThread(new Runnable() {
            @Override
            public void run() {
                Toast.makeText(context, msg, Toast.LENGTH_SHORT).show();
            }
        });
    }
}