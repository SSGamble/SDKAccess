/****************************************************
    文件：SDKMainWnd.cs
	作者：TravelerTD
    日期：2019/09/21 20:13:09
	功能：SDK 测试，主界面
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.UI;

public class SDKMainWnd : WndBase {

    public Button btnLoginQQ;
    public Button btnLogoutQQ;

    public override void InitWnd(params object[] paralist) {

#if UNITY_ANDROID || UNITY_IOS
        PhoneManager.Instance.LoginSuccess += LoginSuccess;
        PhoneManager.Instance.LoginFail += LoginFail;
        PhoneManager.Instance.LoginCanacle += LoginCancle;

        if (!PhoneManager.Instance.IsLogin) { // 没有登录
            PhoneManager.Instance.AutoLogin(); // 自动登录
        }
        OnClickDown(btnLoginQQ.gameObject, ClickLoginQQBtn);
        OnClickDown(btnLoginQQ.gameObject, (object obj) => {
            UIManager.Instance.PopUpWnd(UIWndType.MainWnd);
        });
        OnClickDown(btnLogoutQQ.gameObject, ClickLogoutQQBtn);
        OnClickDown(btnLogoutQQ.gameObject, (object obj) => {
            UIManager.Instance.PopUpWnd(UIWndType.MainWnd);
        });
#endif

    }

    private void ClickLoginQQBtn(object obj) {
        PhoneManager.Instance.Login(PlatEnum.QQ);
    }

    private void ClickLogoutQQBtn(object obj) {
        PhoneManager.Instance.Logout();
    }

    public override void OnClose() {
        PhoneManager.Instance.LoginSuccess -= LoginSuccess;
        PhoneManager.Instance.LoginFail -= LoginFail;
        PhoneManager.Instance.LoginCanacle -= LoginCancle;
    }

    void LoginSuccess(PlatEnum plat) {
        if (plat == PlatEnum.QQ) {
            Debug.Log("QQ 登录成功！");
        }
        UIManager.Instance.HideWnd(UIWndType.SDKMainWnd);
        UIManager.Instance.PopUpWnd(UIWndType.MainWnd);
    }

    void LoginFail(PlatEnum plat) {
        if (plat == PlatEnum.QQ) {
            Debug.Log("QQ 登录失败！");
        }
    }

    void LoginCancle(PlatEnum plat) {
        if (plat == PlatEnum.QQ) {
            Debug.Log("QQ 取消登录！");
        }
    }
}