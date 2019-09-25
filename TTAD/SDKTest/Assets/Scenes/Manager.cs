using ByteDance.Union;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public sealed class Manager : MonoBehaviour {
    [SerializeField]
    private Text information;

    private AdNative adNative;  // AdNative 对象为加载广告的入口，可用于广告获取。
    private RewardVideoAd rewardAd;
    private FullScreenVideoAd fullScreenVideoAd;
    private NativeAd bannerAd;
    private NativeAd intersititialAd;
    private AndroidJavaObject mBannerAd;
    private AndroidJavaObject mIntersititialAd;
    private AndroidJavaObject activity;
    private AndroidJavaObject mNativeAdManager;

    private AdNative AdNative {
        get {
            if (this.adNative == null) {
                this.adNative = SDK.CreateAdNative(); //  构建 AdNative 对象 ，一定要在初始化后才能调用，否则为空
            }

            return this.adNative;
        }
    }

    public void LoadNativeNannerAd() {
#if UNITY_IOS
        if (this.bannerAd != null)
        {
            Debug.LogError("广告已经加载");
            this.information.text = "广告已经加载";
            return;
        }
#else
        if (this.mBannerAd != null) {
            Debug.LogError("广告已经加载");
            this.information.text = "广告已经加载";
            return;
        }
#endif

        var adSlot = new AdSlot.Builder() // 构建 AdSlot 对象，AdSlot 对象为加载广告时需要设置的广告信息，在 AdNative 加载广告时使用。
            .SetCodeId("901121423")
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(600, 257)
            .SetNativeAdType(AdSlotType.Banner)
            .SetAdCount(1)
            .Build();
        this.AdNative.LoadNativeAd(adSlot, new NativeAdListener(this));
    }

    public void LoadNativeIntersititialAd() {
#if UNITY_IOS
        if (this.intersititialAd != null)
        {
            Debug.LogError("广告已经加载");
            this.information.text = "广告已经加载";
            return;
        }
#else
        if (this.mIntersititialAd != null) {
            Debug.LogError("广告已经加载");
            this.information.text = "广告已经加载";
            return;
        }
#endif
        var adSlot = new AdSlot.Builder()
            .SetCodeId("901121435")
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(600, 257)
            .SetNativeAdType(AdSlotType.InteractionAd)
            .SetAdCount(1)
            .Build();
        this.AdNative.LoadNativeAd(adSlot, new NativeAdListener(this));
    }

    /// <summary>
    /// 加载 奖励视频 广告
    /// </summary>
    public void LoadRewardAd() {
        if (this.rewardAd != null) {
            Debug.LogError("广告已经加载");
            this.information.text = "广告已经加载";
            return;
        }

        var adSlot = new AdSlot.Builder()
            .SetCodeId("930830162")
            .SetSupportDeepLink(true)
            .SetImageAcceptedSize(1080, 1920)
            .SetRewardName("金币") // 奖励的名称
            .SetRewardAmount(3) // 奖励的数量
            .SetUserID("15592") // 用户id,必传参数
            .SetMediaExtra("media_extra") // 附加参数，可选
            .SetOrientation(AdOrientation.Vertical) // 必填参数，期望视频的播放方向
            .Build();

        this.AdNative.LoadRewardVideoAd(
            adSlot, new RewardVideoAdListener(this));
    }

    /// <summary>
    /// Show the reward Ad.
    /// </summary>
    public void ShowRewardAd() {
        if (this.rewardAd == null) {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        this.rewardAd.ShowRewardVideoAd();
    }

    /// <summary>
    /// Loads the full screen video ad.
    /// </summary>
    public void LoadFullScreenVideoAd() {
        var adSlot = new AdSlot.Builder()
                             .SetCodeId("901121375")
                             .SetSupportDeepLink(true)
                             .SetImageAcceptedSize(1080, 1920)
                             .SetOrientation(AdOrientation.Horizontal)
                             .Build();
        this.AdNative.LoadFullScreenVideoAd(adSlot, new FullScreenVideoAdListener(this));

    }

    /// <summary>
    /// Show the reward Ad.
    /// </summary>
    public void ShowFullScreenVideoAd() {
#if UNITY_IOS
        if (this.fullScreenVideoAd == null)
        {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        this.fullScreenVideoAd.ShowFullScreenVideoAd();
#else
        if (this.fullScreenVideoAd == null) {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }

        this.fullScreenVideoAd.ShowFullScreenVideoAd();
        this.fullScreenVideoAd = null;
#endif
    }

    public void ShowNativeNannerAd() {
#if UNITY_IOS
       if (bannerAd == null)
        {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        this.bannerAd.ShowNativeAd();
#else
        if (mBannerAd == null) {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        if (mNativeAdManager == null) {
            mNativeAdManager = GetNativeAdManager();
        }
        mNativeAdManager.Call("showNativeBannerAd", activity, mBannerAd);
#endif
    }

    public void ShowNativeIntersititialAd() {
#if UNITY_IOS
        if (intersititialAd == null)
        {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        this.intersititialAd.ShowNativeAd();
#else
        if (mIntersititialAd == null) {
            Debug.LogError("请先加载广告");
            this.information.text = "请先加载广告";
            return;
        }
        if (mNativeAdManager == null) {
            mNativeAdManager = GetNativeAdManager();
        }
        mNativeAdManager.Call("showNativeIntersititialAd", activity, mIntersititialAd);
#endif
    }

    public AndroidJavaObject GetNativeAdManager() {
        if (mNativeAdManager != null) {
            return mNativeAdManager;
        }
        if (activity == null) {
            var unityPlayer = new AndroidJavaClass(
            "com.unity3d.player.UnityPlayer");
            activity = unityPlayer.GetStatic<AndroidJavaObject>(
           "currentActivity");
        }
        var jc = new AndroidJavaClass(
                    "com.bytedance.android.NativeAdManager");
        mNativeAdManager = jc.CallStatic<AndroidJavaObject>("getNativeAdManager");
        return mNativeAdManager;
    }

    /// <summary>
    /// Dispose the reward Ad.
    /// </summary>
    public void DisposeAds() {
#if UNITY_IOS
        if (this.rewardAd != null)
        {
            this.rewardAd.Dispose();
            this.rewardAd = null;
        }
        if (this.fullScreenVideoAd != null)
        {
            this.fullScreenVideoAd.Dispose();
            this.fullScreenVideoAd = null;
        }

        if (this.bannerAd != null)
        {
            this.bannerAd.Dispose();
            this.bannerAd = null;
        }
        if (this.intersititialAd != null)
        {
            this.intersititialAd.Dispose();
            this.intersititialAd = null;
        }
#else
        if (this.rewardAd != null) {
            this.rewardAd = null;
        }
        if (this.fullScreenVideoAd != null) {
            this.fullScreenVideoAd = null;
        }
        if (this.mBannerAd != null) {
            this.mBannerAd = null;
        }
        if (this.mIntersititialAd != null) {
            this.mIntersititialAd = null;
        }
#endif
    }

    private sealed class RewardVideoAdListener : IRewardVideoAdListener {
        private Manager example;

        public RewardVideoAdListener(Manager example) {
            this.example = example;
        }

        public void OnError(int code, string message) {
            Debug.LogError("OnRewardError: " + message);
            this.example.information.text = "OnRewardError: " + message;
        }

        public void OnRewardVideoAdLoad(RewardVideoAd ad) {
            Debug.Log("OnRewardVideoAdLoad");
            this.example.information.text = "OnRewardVideoAdLoad";

            ad.SetRewardAdInteractionListener(
                new RewardAdInteractionListener(this.example));
            ad.SetDownloadListener(
                new AppDownloadListener(this.example));

            this.example.rewardAd = ad;
        }

        public void OnRewardVideoCached() {
            Debug.Log("OnRewardVideoCached");
            this.example.information.text = "OnRewardVideoCached";
        }
    }
    /// <summary>
    /// Full screen video ad listener.
    /// </summary>
    private sealed class FullScreenVideoAdListener : IFullScreenVideoAdListener {
        private Manager example;

        public FullScreenVideoAdListener(Manager example) {
            this.example = example;
        }

        public void OnError(int code, string message) {
            Debug.LogError("OnFullScreenError: " + message);
            this.example.information.text = "OnFullScreenError: " + message;
        }

        public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad) {
            Debug.Log("OnFullScreenAdLoad");
            this.example.information.text = "OnFullScreenAdLoad";

            ad.SetFullScreenVideoAdInteractionListener(
                new FullScreenAdInteractionListener(this.example));
            ad.SetDownloadListener(
                new AppDownloadListener(this.example));

            this.example.fullScreenVideoAd = ad;
        }

        public void OnFullScreenVideoCached() {
            Debug.Log("OnFullScreenVideoCached");
            this.example.information.text = "OnFullScreenVideoCached";
        }
    }
    private sealed class NativeAdInteractionListener : IInteractionAdInteractionListener {
        private Manager example;

        public NativeAdInteractionListener(Manager example) {
            this.example = example;
        }

        public void OnAdShow() {
            Debug.Log("NativeAd show");
            this.example.information.text = "NativeAd show";
        }

        public void OnAdClicked() {
            Debug.Log("NativeAd click");
            this.example.information.text = "NativeAd click";
        }

        public void OnAdDismiss() {
            Debug.Log("NativeAd close");
            this.example.information.text = "NativeAd close";
        }
    }
    private sealed class NativeAdListener : INativeAdListener {
        private Manager example;

        public NativeAdListener(Manager example) {
            this.example = example;
        }

        public void OnError(int code, string message) {
            Debug.LogError("OnNativeAdError: " + message);
            this.example.information.text = "OnNativeAdError: " + message;
        }

        public void OnNativeAdLoad(AndroidJavaObject list, NativeAd ad) {
#if UNITY_IOS
            this.example.bannerAd = ad;
            this.example.intersititialAd = ad;
            ad.SetNativeAdInteractionListener(
                new NativeAdInteractionListener(this.example)
            );
#else
            var size = list.Call<int>("size");

            if (size > 0) {
                this.example.mBannerAd = list.Call<AndroidJavaObject>("get", 0);
                this.example.mIntersititialAd = list.Call<AndroidJavaObject>("get", 0);
            }

#endif
            //if (ads == null && ads.[0])
            //{
            //    return;
            //}
            ////this.example.bannerAd = ads.[0];
            //this.example.bannerAd = ads.[0];
            Debug.Log("OnNativeAdLoad");
            this.example.information.text = "OnNativeAdLoad";


            //bannerAd.;
            //bannerAd.SetDownloadListener(
            //new AppDownloadListener(this.example));

        }
    }


    /// <summary>
    /// Full screen ad interaction listener.
    /// </summary>
    private sealed class FullScreenAdInteractionListener : IFullScreenVideoAdInteractionListener {
        private Manager example;

        public FullScreenAdInteractionListener(Manager example) {
            this.example = example;
        }

        public void OnAdShow() {
            Debug.Log("fullScreenVideoAd show");
            this.example.information.text = "fullScreenVideoAd show";
        }

        public void OnAdVideoBarClick() {
            Debug.Log("fullScreenVideoAd bar click");
            this.example.information.text = "fullScreenVideoAd bar click";
        }

        public void OnAdClose() {
            Debug.Log("fullScreenVideoAd close");
            this.example.information.text = "fullScreenVideoAd close";
        }

        public void OnVideoComplete() {
            Debug.Log("fullScreenVideoAd complete");
            this.example.information.text = "fullScreenVideoAd complete";
        }

        public void OnVideoError() {
            Debug.Log("fullScreenVideoAd OnVideoError");
            this.example.information.text = "fullScreenVideoAd OnVideoError";
        }

        public void OnSkippedVideo() {
            Debug.Log("fullScreenVideoAd OnSkippedVideo");
            this.example.information.text = "fullScreenVideoAd skipped";

        }
    }

    private sealed class RewardAdInteractionListener : IRewardAdInteractionListener {
        private Manager example;

        public RewardAdInteractionListener(Manager example) {
            this.example = example;
        }

        public void OnAdShow() {
            Debug.Log("rewardVideoAd show");
            this.example.information.text = "rewardVideoAd show";
        }

        public void OnAdVideoBarClick() {
            Debug.Log("rewardVideoAd bar click");
            this.example.information.text = "rewardVideoAd bar click";
        }

        public void OnAdClose() {
            Debug.Log("rewardVideoAd close");
            this.example.information.text = "rewardVideoAd close";
        }

        public void OnVideoComplete() {
            Debug.Log("rewardVideoAd complete");
            this.example.information.text = "rewardVideoAd complete";
        }

        public void OnVideoError() {
            Debug.LogError("rewardVideoAd error");
            this.example.information.text = "rewardVideoAd error";
        }

        public void OnRewardVerify(
            bool rewardVerify, int rewardAmount, string rewardName) {
            Debug.Log("verify:" + rewardVerify + " amount:" + rewardAmount +
                " name:" + rewardName);
            this.example.information.text =
                "verify:" + rewardVerify + " amount:" + rewardAmount +
                " name:" + rewardName;
        }
    }

    private sealed class AppDownloadListener : IAppDownloadListener {
        private Manager example;

        public AppDownloadListener(Manager example) {
            this.example = example;
        }

        public void OnIdle() {
        }

        public void OnDownloadActive(
            long totalBytes, long currBytes, string fileName, string appName) {
            Debug.Log("下载中，点击下载区域暂停");
            this.example.information.text = "下载中，点击下载区域暂停";
        }

        public void OnDownloadPaused(
            long totalBytes, long currBytes, string fileName, string appName) {
            Debug.Log("下载暂停，点击下载区域继续");
            this.example.information.text = "下载暂停，点击下载区域继续";
        }

        public void OnDownloadFailed(
            long totalBytes, long currBytes, string fileName, string appName) {
            Debug.LogError("下载失败，点击下载区域重新下载");
            this.example.information.text = "下载失败，点击下载区域重新下载";
        }

        public void OnDownloadFinished(
            long totalBytes, string fileName, string appName) {
            Debug.Log("下载完成，点击下载区域重新下载");
            this.example.information.text = "下载完成，点击下载区域重新下载";
        }

        public void OnInstalled(string fileName, string appName) {
            Debug.Log("安装完成，点击下载区域打开");
            this.example.information.text = "安装完成，点击下载区域打开";
        }
    }
}
