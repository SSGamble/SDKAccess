/****************************************************
    文件：RFramework.cs
	作者：TravelerTD
    日期：2019/09/21 12:33:29
	功能：资源加载框架
*****************************************************/

using UnityEngine;
using UnityEngine.EventSystems;

public class RFramework : MonoSingleton<RFramework> {

    protected override void Awake() {
        base.Awake();
        PlatMsgManager.Instance.Init(); // 平台管理类的初始化
        Debug.Log("系统总内存：" + PlatMsgManager.Instance.GetLongFromPlatform(2) / (1024.0f * 1024.0f) + "MB  系统可以内存：" + PlatMsgManager.Instance.GetLongFromPlatform(1) / (1024.0f * 1024.0f) + "MB  使用内存：" + PlatMsgManager.Instance.GetLongFromPlatform(3) / (1024.0f * 1024.0f) + "MB");

        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
    }

    private void Start() {
        // 初始化 UI
        UIManager.Instance.Init(transform.Find("UIRoot") as RectTransform, transform.Find("UIRoot/WndRoot") as RectTransform, transform.Find("UIRoot/UICamera").GetComponent<Camera>(), transform.Find("UIRoot/EventSystem").GetComponent<EventSystem>());
        // 注册窗口
        RegisterWnd();
        // 打开窗口
        UIManager.Instance.PopUpWnd(UIWndType.SDKMainWnd);
    }

    void Update() {
        // 窗口更新
        UIManager.Instance.OnUpdate();
    }

    /// <summary>
    /// 注册窗口
    /// </summary>
    private void RegisterWnd() {
        UIManager.Instance.Register(UIWndType.SDKMainWnd, ConStr.SDK_MAIN_WND);
        UIManager.Instance.Register(UIWndType.MainWnd, ConStr.MAIN_WND);
        UIManager.Instance.Register(UIWndType.SetWnd, ConStr.SET_WND);
        UIManager.Instance.Register(UIWndType.SkillWnd, ConStr.SKILL_WND);
    }
}