/****************************************************
    文件：SetWnd3.cs
	作者：TravelerTD
    日期：2019/08/30 19:50:36
	功能：设置窗口
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetWnd : WndBase {
    public Button btnClose;

    public override void InitWnd(params object[] paralist) {
        OnClickDown(btnClose.gameObject, (object obj) => {
            UIManager.Instance.HideWnd(UIWndType.SetWnd);
        });
    }

    public override void OnShow(params object[] paralist) {

    }

    public override void OnClose() {

    }
}