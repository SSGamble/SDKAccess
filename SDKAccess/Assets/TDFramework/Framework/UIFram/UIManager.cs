/****************************************************
    文件：TDUIManaher.cs
	作者：TravelerTD
    日期：2019/08/30 17:35:39
	功能：UI 框架管理
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIManager {
    // 单例
    private static UIManager _instance;
    public static UIManager Instance {
        get {
            if (_instance == null) {
                _instance = new UIManager();
            }
            return _instance;
        }
    }
    // 单例，私有化构造方法
    private UIManager() {
        //RegisterWnd();
    }

    /// <summary>
    /// 注册窗口
    /// </summary>
    private void RegisterWnd() {
        Register(UIWndType.MainWnd, "Assets/TDUIFram/UIPanel/MainWnd.prefab");
        Register(UIWndType.SetWnd, "Assets/TDUIFram/UIPanel/SetWnd.prefab");
        Register(UIWndType.SkillWnd, "Assets/TDUIFram/UIPanel/SkillWnd.prefab");
    }

    /// <summary>
    /// UI 节点
    /// </summary>
    public RectTransform uiRoot;
    /// <summary>
    /// 窗口节点
    /// </summary>
    private RectTransform wndRoot;
    /// <summary>
    /// UI 摄像机
    /// </summary>
    private Camera uiCamera;
    /// <summary>
    /// EventSystem 节点
    /// </summary>
    private EventSystem eventSystem;
    /// <summary>
    /// 屏幕的宽高比
    /// </summary>
    private float canvasRate = 0;

    /// <summary>
    /// 注册窗口的字典，存储所有 面板 Prefab 的路径；key：面板名，value：路径
    /// </summary>
    private Dictionary<UIWndType, string> registerDic = new Dictionary<UIWndType, string>();
    /// <summary>
    /// 所有实例化的窗口,存储所有实例化的面板的游戏物体身上的 BasePanel 组件；key：面板名，value：面板组件(游戏物体)
    /// </summary>
    private Dictionary<UIWndType, WndBase> preWndDic = new Dictionary<UIWndType, WndBase>();

    /// <summary>
    /// 打开的窗口列表，用于遍历关闭
    /// </summary>
    private List<WndBase> openWndList = new List<WndBase>();

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="uiRoot">UI 父节点</param>
    /// <param name="wndRoot">窗口父节点</param>
    /// <param name="uiCamera">UI 摄像机</param>
    public void Init(RectTransform uiRoot, RectTransform wndRoot, Camera uiCamera, EventSystem eventSystem) {
        this.uiRoot = uiRoot;
        this.wndRoot = wndRoot;
        this.uiCamera = uiCamera;
        this.eventSystem = eventSystem;
        canvasRate = Screen.height / (uiCamera.orthographicSize * 2);
    }


    /// <summary>
    /// 显示或者隐藏所有 UI，直接对父节点进行操作
    /// </summary>
    /// <param name="isShow">默认显示</param>
    public void ShowOrHideUIRoot(bool isShow = true) {
        if (uiRoot != null) {
            uiRoot.gameObject.SetActive(isShow);
        }
    }

    /// <summary>
    /// 显示或者隐藏所有 UI，遍历子节点进行操作
    /// </summary>
    /// <param name="isShow">默认显示</param>
    public void ShowOrHideUI(bool isShow = true) {
        if (uiRoot != null) { 
            for (int i = 0; i < uiRoot.childCount; i++) {
                uiRoot.GetChild(i).gameObject.SetActive(isShow);
            }
        }
    }

    /// <summary>
    /// 设置默认选择对象，eg: 以进入菜单界面，就默认选择了某个按钮
    /// </summary>
    /// <param name="obj">默认选择对象</param>
    public void SetNormalSelectObj(GameObject obj) {
        if (eventSystem == null) {
            eventSystem = EventSystem.current;
        }
        eventSystem.firstSelectedGameObject = obj;
    }

    /// <summary>
    /// 窗口的更新
    /// </summary>
    public void OnUpdate() {
        for (int i = 0; i < openWndList.Count; i++) {
            if (openWndList[i] != null) {
                openWndList[i].OnUpdate();
            }
        }
    }

    /// <summary>
    /// 窗口注册方法
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <param name="preName">UIWnd 的 Pre 名称</param>
    public void Register(UIWndType type, string preName) {
        registerDic.Add(type, ConStr.UIWND_PRE_PATH + preName);
    }

    /// <summary>
    /// 发送消息给窗口
    /// </summary>
    /// <param name="name">窗口名</param>
    /// <param name="msgID">消息 ID</param>
    /// <param name="paralist">参数数组</param>
    /// <returns></returns>
    public bool SendMessageToWnd(UIWndType type, UIMsgID msgID = 0, params object[] paralist) {
        WndBase wnd = FindWndByName<WndBase>(type);
        if (wnd != null) {
            return wnd.OnMessage(msgID, paralist);
        }
        return false;
    }

    /// <summary>
    /// 在实例化的窗口中，根据窗口名查找窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <returns></returns>
    public T FindWndByName<T>(UIWndType type) where T : WndBase {
        WndBase wnd = null;
        if (preWndDic.TryGetValue(type, out wnd)) {
            return (T)wnd;
        }
        return null;
    }

    /// <summary>
    /// 打开窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <param name="bTop">是否在最上面</param>
    /// <param name="paralist">可传递的参数</param>
    /// <returns></returns>
    public WndBase PopUpWnd(UIWndType type, bool bTop = false, params object[] paralist) {
        WndBase wnd = FindWndByName<WndBase>(type);
        if (wnd == null) {
            string path = registerDic.TryGet(type); // 得到面板对应的路径
            GameObject wndObj = ObjectManager.Instance.InstantiateObject(path, true); // 加载窗口，窗口的加载应该是同步的
            wnd = wndObj.GetComponent<WndBase>();
            if (wndObj == null) {
                Debug.Log("创建窗口 Prefab 失败：" + type);
                return null;
            }
            if (!preWndDic.ContainsKey(type)) {
                openWndList.Add(wnd);
                preWndDic.Add(type, wnd);
            }
            wnd.Name = type;
            wnd.InitWnd(paralist);
            wndObj.transform.SetParent(wndRoot, false);
            if (bTop) {
                wndObj.transform.SetAsLastSibling();
            }
            wnd.OnShow(paralist);
        }
        else {
            ShowWnd(type, bTop, paralist);
        }
        return wnd;
    }

    /// <summary>
    /// 根据窗口名关闭窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <param name="destory">是否释放对象资源</param>
    public void CloseWnd(UIWndType type, bool destory = false) {
        WndBase wnd = FindWndByName<WndBase>(type);
        CloseWnd(wnd, destory);
    }

    /// <summary>
    /// 根据窗口对象关闭窗口
    /// </summary>
    /// <param name="window">窗口对象</param>
    /// <param name="destory">是否释放对象资源</param>
    public void CloseWnd(WndBase window, bool destory = false) {
        if (window != null) {
            window.OnDisable();
            window.OnClose();
            if (preWndDic.ContainsKey(window.Name)) {
                preWndDic.Remove(window.Name);
                openWndList.Remove(window);
            }
            if (destory) {
                ObjectManager.Instance.ReleaseObject(window.gameObject, 0, true);
            }
            else {
                ObjectManager.Instance.ReleaseObject(window.gameObject, recycleParent: false);
            }
            window = null;
        }
    }

    /// <summary>
    /// 关闭所有窗口
    /// </summary>
    public void CloseAllWnd() {
        for (int i = openWndList.Count - 1; i >= 0; i--) {
            CloseWnd(openWndList[i]);
        }
    }

    /// <summary>
    /// 切换到唯一窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <param name="bTop">是否在最上面</param>
    /// <param name="paralist">可供传递的参数</param>
    public void SwitchStateByName(UIWndType type, bool bTop = true, params object[] paralist) {
        CloseAllWnd();
        PopUpWnd(type, bTop, paralist);
    }

    /// <summary>
    /// 根据名字隐藏窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    public void HideWnd(UIWndType type) {
        WndBase wnd = FindWndByName<WndBase>(type);
        HideWnd(wnd);
    }

    /// <summary>
    /// 根据窗口对象隐藏窗口
    /// </summary>
    /// <param name="wnd">窗口对象</param>
    public void HideWnd(WndBase wnd) {
        if (wnd != null) {
            wnd.gameObject.SetActive(false);
            wnd.OnDisable();
        }
        else {
            Debug.LogError("wnd == null");
        }
    }

    /// <summary>
    /// 根据窗口名字显示窗口
    /// </summary>
    /// <param name="type">窗口类型</param>
    /// <param name="bTop">是否在最上面</param>
    /// <param name="paralist">可供传递的参数</param>
    public void ShowWnd(UIWndType type, bool bTop = true, params object[] paralist) {
        WndBase wnd = FindWndByName<WndBase>(type);
        ShowWnd(wnd, bTop, paralist);
    }

    /// <summary>
    /// 根据窗口对象显示窗口
    /// </summary>
    /// <param name="wnd">窗口类型</param>
    /// <param name="bTop">是否在最上面</param>
    /// <param name="paralist">可供传递的参数</param>
    public void ShowWnd(WndBase wnd, bool bTop = true, params object[] paralist) {
        if (wnd != null) {
            if (wnd.gameObject != null && !wnd.gameObject.activeSelf) {
                wnd.gameObject.SetActive(true);
            }
            if (bTop) {
                wnd.transform.SetAsLastSibling();
            }
            wnd.OnShow(paralist);
        }
    }
}
