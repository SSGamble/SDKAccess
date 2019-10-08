package com.td.ttad;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import com.bytedance.sdk.openadsdk.AdSlot;
import com.bytedance.sdk.openadsdk.TTAdConstant;
import com.bytedance.sdk.openadsdk.TTAdManager;
import com.bytedance.sdk.openadsdk.TTAdNative;
import com.bytedance.sdk.openadsdk.TTFullScreenVideoAd;
import com.td.ttad.utils.TToast;

public class FullScreenVideoActivity extends AppCompatActivity {

    private Button mLoadAd;
    private Button mLoadAdVertical;
    private Button mShowAd;
    private TTAdNative mTTAdNative;
    private TTFullScreenVideoAd mttFullVideoAd;

    @SuppressWarnings("RedundantCast")
    @Override
    protected void onCreate(@Nullable Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_full_screen_video);
        TTAdManagerHolder.init(this); // Unity 启动活动的时候，初始化 SDK
        mLoadAd = (Button) findViewById(R.id.btn_reward_load);
        mLoadAdVertical = (Button) findViewById(R.id.btn_reward_load_vertical);
        mShowAd = (Button) findViewById(R.id.btn_reward_show);
        //step1:初始化sdk
        TTAdManager ttAdManager = TTAdManagerHolder.get();
        //step2:(可选，强烈建议在合适的时机调用):申请部分权限，如read_phone_state,防止获取不了imei时候，下载类广告没有填充的问题。
        TTAdManagerHolder.get().requestPermissionIfNecessary(this);
        //step3:创建TTAdNative对象,用于调用广告请求接口
        mTTAdNative = ttAdManager.createAdNative(getApplicationContext());
        initClickEvent();
    }


    private void initClickEvent() {
        mLoadAd.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                loadAd("901121184", TTAdConstant.HORIZONTAL);
            }
        });
        mLoadAdVertical.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                loadAd("901121375", TTAdConstant.VERTICAL);
            }
        });
        mShowAd.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                if (mttFullVideoAd != null) {
                    //step6:在获取到广告后展示
                    //该方法直接展示广告
                    mttFullVideoAd.showFullScreenVideoAd(FullScreenVideoActivity.this);

                    //展示广告，并传入广告展示的场景
//                    mttFullVideoAd.showFullScreenVideoAd(FullScreenVideoActivity.this,TTAdConstant.RitScenes.GAME_GIFT_BONUS,null);
//                    mttFullVideoAd = null;
                } else {
                    TToast.show(FullScreenVideoActivity.this, "请先加载广告");
                }
            }
        });
    }

    @SuppressWarnings("SameParameterValue")
    private void loadAd(String codeId, int orientation) {
        //step4:创建广告请求参数AdSlot,具体参数含义参考文档
        AdSlot adSlot = new AdSlot.Builder()
                .setCodeId(codeId)
                .setSupportDeepLink(true)
                .setImageAcceptedSize(1080, 1920)
                .setOrientation(orientation)//必填参数，期望视频的播放方向：TTAdConstant.HORIZONTAL 或 TTAdConstant.VERTICAL
                .build();
        //step5:请求广告
        mTTAdNative.loadFullScreenVideoAd(adSlot, new TTAdNative.FullScreenVideoAdListener() {
            @Override
            public void onError(int code, String message) {
                TToast.show(FullScreenVideoActivity.this, message);
            }

            @Override
            public void onFullScreenVideoAdLoad(TTFullScreenVideoAd ad) {
                TToast.show(FullScreenVideoActivity.this, "FullVideoAd loaded");
                mttFullVideoAd = ad;
                mttFullVideoAd.setFullScreenVideoAdInteractionListener(new TTFullScreenVideoAd.FullScreenVideoAdInteractionListener() {

                    @Override
                    public void onAdShow() {
                        TToast.show(FullScreenVideoActivity.this, "FullVideoAd show");
                    }

                    @Override
                    public void onAdVideoBarClick() {
                        TToast.show(FullScreenVideoActivity.this, "FullVideoAd bar click");
                    }

                    @Override
                    public void onAdClose() {
                        TToast.show(FullScreenVideoActivity.this, "FullVideoAd close");
                    }

                    @Override
                    public void onVideoComplete() {
                        TToast.show(FullScreenVideoActivity.this, "FullVideoAd complete");
                    }

                    @Override
                    public void onSkippedVideo() {
                        TToast.show(FullScreenVideoActivity.this, "FullVideoAd skipped");

                    }

                });
            }

            @Override
            public void onFullScreenVideoCached() {
                TToast.show(FullScreenVideoActivity.this, "FullVideoAd video cached");
            }
        });


    }
}
