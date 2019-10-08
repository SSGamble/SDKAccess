using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    private AndroidJavaClass jc;
    private AndroidJavaObject jo;
    public Button btnLoadBanner;
    public Button btnLoadInteraction;

    public void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
            // 获取 Unity 的 MainActivity 的固定写法
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        btnLoadBanner.onClick.AddListener(BannerBtnClick);
        btnLoadInteraction.onClick.AddListener(InteractionBtnClick);
    }

    private void Start() {
        BannerBtnClick();
    }

    /// <summary>
    /// 加载竖屏广告
    /// </summary>
    public void BannerBtnClick() {
        if (jo != null) {
            //jo.Call("Init");
            jo.Call("bannerLandingpage");
        }
    }

    /// <summary>
    /// 显示广告视频内容
    /// </summary>
    public void InteractionBtnClick() {
        if (jo != null) {
            jo.Call("loadInteractionAd", "901121725");
        }
    }
}