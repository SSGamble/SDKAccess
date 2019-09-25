/****************************************************
    文件：PhongManager.cs
	作者：TravelerTD
    日期：2019/09/21 16:35:48
	功能：手机登录管理类
*****************************************************/

using LitJson;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 平台
/// </summary>
public enum PlatEnum {
    None,
    QQ = 1,
    WX = 2,
}

public class PhoneManager : Singleton<PhoneManager> {
    /// <summary>
    /// 当前是否已经登陆了
    /// </summary>
    public bool IsLogin { get; set; } = false;
    /// <summary>
    /// 当前平台
    /// </summary>
    public PlatEnum CurrentPlat = PlatEnum.None;
    /// <summary>
    /// 登录成功的回调
    /// </summary>
    public Action<PlatEnum> LoginSuccess;
    /// <summary>
    /// 登录失败的回调
    /// </summary>
    public Action<PlatEnum> LoginFail;
    /// <summary>
    /// 取消登录的回调
    /// </summary>
    public Action<PlatEnum> LoginCanacle;


    #region 从底层获取 String
    public const int UNITY_GET_STRING_QQAUTORVAILD = 1; // QQ 票据是否有效
    public const int UNITY_GET_STRING_QQREFRESHSESSION = 2; // QQ 刷新票据
    #endregion

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="plat"></param>
    public void Login(PlatEnum plat) {
        if (plat == PlatEnum.QQ) {
            PlatMsgManager.Instance.SendUnityMessageToPlatform(PlatMsgManager.PLATFORM_MSG_QQLOGIN);
        }
    }

    /// <summary>
    /// 注销
    /// </summary>
    public void Logout() {
        if (CurrentPlat == PlatEnum.QQ) {
            PlatMsgManager.Instance.SendUnityMessageToPlatform(PlatMsgManager.PLATFORM_MSG_QQLOGOUT);
        }
    }

    /// <summary>
    /// QQ 授权是否有效
    /// </summary>
    /// <returns></returns>
    public bool QQAuthorVaild() {
        return Convert.ToBoolean(PlatMsgManager.Instance.GetStringFromPlatform(UNITY_GET_STRING_QQAUTORVAILD));
    }

    /// <summary>
    /// 刷新票据
    /// </summary>
    /// <returns></returns>
    public JsonData GetQQSession() {
        JsonData jd = JsonMapper.ToObject(PlatMsgManager.Instance.GetStringFromPlatform(UNITY_GET_STRING_QQREFRESHSESSION));
        return jd;
    }

    /// <summary>
    /// 自动登录
    /// </summary>
    public void AutoLogin() {
#if !UNITY_EDITOR
        if (PlayerPrefs.HasKey("Login")) {
            PlatEnum plat = (PlatEnum)Enum.Parse(typeof(PlatEnum), PlayerPrefs.GetString("Login"));
            if (plat == PlatEnum.QQ) {
                if (QQAuthorVaild()) {
                    JsonData jd = GetQQSession();
                    LoginCallBack(jd, PlatEnum.QQ);
                }
            }
        }
        else {
            if (QQAuthorVaild()) {
                JsonData jd = GetQQSession();
                LoginCallBack(jd, PlatEnum.QQ);
            }
        }
#endif
    }

    /// <summary>
    /// 登录回调
    /// </summary>
    /// <param name="jd">数据</param>
    /// <param name="plat">平台类型</param>
    /// <param name="result">登录结果，0==成功，1==失败，2==取消</param>
    public void LoginCallBack(JsonData jd, PlatEnum plat, int result = 0) {
        if (result == 0) { // 登录成功
            Debug.Log("票据获取： " + jd.ToJson().ToString());
            string openid = (string)jd["openid"];
            string unionid = (string)jd["unionid"];
            string token = (string)jd["token"];
            IsLogin = true;
            CurrentPlat = plat;
            if (LoginSuccess != null) { // 传送到服务器 XXX
                LoginSuccess(plat);
            }
            PlayerPrefs.SetString("Login", plat.ToString());
            PlayerPrefs.Save();
        }
        else if (result == 1) { // 登录失败
            if (LoginFail != null) {
                LoginFail(plat);
            }
        }
        else if (result == 2) { // 取消登录
            if (LoginCanacle != null) {
                LoginCanacle(plat);
            }
        }
    }

}
