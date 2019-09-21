/****************************************************
    文件：TimerSvc.cs
	作者：TravelerTD
    日期：2019/09/01 22:28:39
	功能：计时服务
*****************************************************/

using System;
using TDFramework;
using UnityEngine;

public class TimerSvc : SystemRoot {

    public static TimerSvc Instance = null;
    private TDTimer tt;

    public void InitSvc() {
        Instance = this;

        tt = new TDTimer();

        // 设置 TDTimer 的日志输出
        tt.SetLog((string info) => {
            TDLog.Log(info);
        });

        TDLog.Log("Init TimerSvc...");
    }

    public void Update() {
        tt.Update(); // 驱动整个定时任务的检测
    }

    /// <summary>
    /// 添加定时任务
    /// </summary>
    public int AddTimeTask(Action<int> callback, double delay, TimeUnit timeUnit = TimeUnit.Millisecond, int count = 1) {
        return tt.AddTimeTask(callback, delay, timeUnit, count);
    }

    /// <summary>
    /// 获取当前时间
    /// </summary>
    public double GetNowTime() {
        return tt.GetMillisecondsTime();
    }

    /// <summary>
    /// 删除任务
    /// </summary>
    /// <param name="tid"></param>
    public void DelTask(int tid) {
        tt.DelTimeTask(tid);
    }
}