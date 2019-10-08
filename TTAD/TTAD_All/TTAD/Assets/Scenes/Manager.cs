using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    private AndroidJavaClass jc;
    private AndroidJavaObject jo;
    public Button btnLoadBanner;

    public void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
            // 获取 Unity 的 MainActivity 的固定写法
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        btnLoadBanner.onClick.AddListener(BannerBtnClick);
    }

    /// <summary>
    /// 加载 Banner 广告
    /// </summary>
    public void BannerBtnClick() {
        if (jo != null) {
            jo.Call("LoadBannerAd");
        }
    }
}