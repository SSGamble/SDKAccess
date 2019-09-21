/****************************************************
    文件：PlatformScript.cs
	作者：TravelerTD
    日期：2019/09/21 10:48:50
	功能：平台消息类，用于处理各个平台发送过来的消息
*****************************************************/

using System.Collections.Generic;
using LitJson;
using UnityEngine;

#if UNITY_IOS || UNITY_ANDROID

public class PlatformScript : MonoBehaviour {

    /// <summary>
    /// 其他平台(底层)向 Unity 传的消息数据
    /// </summary>
    private struct PlatformMsg {
        public int iMsgId;
        // 基本结构值
        public int iPararm1;
        public int iPararm2;
        public int iPararm3;
        public string strParam1;
        public string strParam2;
        public string strParam3;
    }

    private const int PLATFORM_MSG_QQLOGINCALLBACK = 1; // QQ 登录回调

    /// <summary>
    /// 消息队列
    /// </summary>
    private Queue<PlatformMsg> msgQueue = new Queue<PlatformMsg>();

    /// <summary>
    /// 底层向 Unity 传递数据，解析 json
    /// </summary>
    /// <param name="param">json</param>
    protected void OnMessage(string param) {
        JsonData jd = JsonMapper.ToObject(param);
        PlatformMsg msg;
        msg.iMsgId = (int)jd["iMsgId"];
        msg.iPararm1 = (int)jd["iPararm1"];
        msg.iPararm2 = (int)jd["iPararm2"];
        msg.iPararm3 = (int)jd["iPararm3"];
        msg.strParam1 = (string)jd["strParam1"];
        msg.strParam2 = (string)jd["strParam2"];
        msg.strParam3 = (string)jd["strParam3"];
        msgQueue.Enqueue(msg); // 加进消息队列
    }

    void Update() {
        // 解析消息队列
        while (msgQueue.Count > 0) {
            PlatformMsg msg = msgQueue.Dequeue();
            switch (msg.iMsgId) {
                case PLATFORM_MSG_QQLOGINCALLBACK: // QQ 登录回调
                    JsonData qqJd = JsonMapper.ToObject(msg.strParam1);
                    PhoneManager.Instance.LoginCallBack(qqJd, PlatEnum.QQ, msg.iPararm1);
                    break;
                default:
                    break;
            }
        }
    }
}

#endif