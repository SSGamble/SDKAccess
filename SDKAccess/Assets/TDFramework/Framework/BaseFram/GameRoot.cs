/****************************************************
    文件：GameRoot.cs
	作者：TravelerTD
    日期：2019/09/01 22:33:54
	功能：游戏的启动入口，初始化各个系统，保存核心数据
*****************************************************/

using TDFramework;
using UnityEngine;

public class GameRoot : MonoSingleton<GameRoot> {

    private void Start() {
        DontDestroyOnLoad(this); // 当前 GameRoot 不需要销毁
        TDLog.Log("Game Start...");
        ClearUIRoot();
        Init();
    }

    /// <summary>
    /// 初始化关闭所有 UI 窗口
    /// </summary>
    private void ClearUIRoot() {
        UIManager.Instance.ShowOrHideUI(false);
    }

    /// <summary>
    /// 初始化各个业务系统和服务模块，应当先初始化服务后初始化业务
    /// </summary>
    private void Init() {
        // 服务模块初始化
        ResSvc res = GetComponent<ResSvc>();
        res.InitSvc();
        AudioSvc audio = GetComponent<AudioSvc>();
        audio.InitSvc();
        TimerSvc timer = GetComponent<TimerSvc>();
        timer.InitSvc();

        // 业务模块初始化
        // 进入登录场景并加载相应 UI
    }
}