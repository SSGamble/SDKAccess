/****************************************************
    文件：SystemRoot.cs
	作者：TravelerTD
    日期：2019/09/01 22:29:57
	功能：业务系统基类
*****************************************************/

using UnityEngine;

public class SystemRoot : MonoBehaviour {

    protected ResSvc resSvc;
    protected AudioSvc audioSvc;
    protected TimerSvc timerSvc;

    public virtual void InitSys() {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;
        timerSvc = TimerSvc.Instance;
    }
}