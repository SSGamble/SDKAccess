using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {

    private AndroidJavaClass jc;
    private AndroidJavaObject jo;
    public Button btnLoadVerticalAD;
    public Button btnShowAD;

    public void Awake() {
#if UNITY_ANDROID && !UNITY_EDITOR
            // 获取 Unity 的 MainActivity 的固定写法
            jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
#endif
        btnLoadVerticalAD.onClick.AddListener(VerticalBtnClick);
        btnShowAD.onClick.AddListener(ShowVedioBtnClick);
    }

    /// <summary>
    /// 加载竖屏广告
    /// </summary>
    public void VerticalBtnClick() {
        if (jo != null) {
            jo.Call("Init");
            jo.Call("LoadVerticalAD");
        }
    }

    /// <summary>
    /// 显示广告视频内容
    /// </summary>
    public void ShowVedioBtnClick() {
        if (jo != null) {
            jo.Call("ShowVedio");
        }
    }
}