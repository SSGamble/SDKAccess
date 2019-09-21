/****************************************************
    文件：SkillWnd3.cs
	作者：TravelerTD
    日期：2019/08/30 19:52:59
	功能：技能窗口
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillWnd : WndBase {
    public Button btnClose;

    public override void InitWnd(params object[] paralist) {
        OnClickDown(btnClose.gameObject, (object obj) => {
            UIManager.Instance.HideWnd(UIWndType.SkillWnd);
        });
    }

    public override void OnShow(params object[] paralist) {

    }

    public override void OnClose() {

    }
}