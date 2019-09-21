/****************************************************
    文件：TDUIFramGameRoot.cs
	作者：TravelerTD
    日期：2019/08/30 17:56:36
	功能：UI 框架示例，游戏入口
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class UIFramGameRoot : MonoBehaviour {
    private void Start() {
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
        // 初始化 UI
        UIManager.Instance.Init(transform.Find("UIRoot") as RectTransform, transform.Find("UIRoot/WndRoot") as RectTransform, transform.Find("UIRoot/UICamera").GetComponent<Camera>(), transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>());
        // 注册窗口
        RegisterWnd();
        // 打开窗口
        UIManager.Instance.PopUpWnd(UIWndType.MainWnd);
    }

    void Update() {
        // 窗口更新
        UIManager.Instance.OnUpdate();
    }

    /// <summary>
    /// 注册窗口
    /// </summary>
    private void RegisterWnd() {
        UIManager.Instance.Register(UIWndType.MainWnd, ConStr.MAIN_WND);
        UIManager.Instance.Register(UIWndType.SetWnd, ConStr.SET_WND);
        UIManager.Instance.Register(UIWndType.SkillWnd, ConStr.SKILL_WND);
    }
}