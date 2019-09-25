/****************************************************
    文件：MainWnd3.cs
	作者：TravelerTD
    日期：2019/08/30 18:21:13
	功能：主窗口
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainWnd : WndBase {

    public Button btnSkill;
    public Button btnSet;

    public override void InitWnd(params object[] paralist) {
        OnClickDown(btnSkill.gameObject, ClickSkillBtn);
        OnClickDown(btnSet.gameObject, (object obj) => {
            UIManager.Instance.PopUpWnd(UIWndType.SetWnd);
        });
    }

    private void ClickSkillBtn(object obj) {
        UIManager.Instance.PopUpWnd(UIWndType.SkillWnd);
    }


    public override void OnShow(params object[] paralist) {


    }

    public override void OnUpdate() {

    }

    public override void OnClose() {

    }
}