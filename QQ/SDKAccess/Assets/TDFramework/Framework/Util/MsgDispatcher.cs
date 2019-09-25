/****************************************************
    文件：MsgDispatcher.cs
	作者：TravelerTD
    日期：2019/8/6 17:51:43
	功能：消息机制 - 消息分发器
*****************************************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TDFramework {
    public class MsgDispatcher {
        // 已注册的全局消息
        private static Dictionary<string, Action<object>> registeredMsgs = new Dictionary<string, Action<object>>();

        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void Register(string msgName, Action<object> onMsgReceived) {
            // 如果字典中没有这个消息名，就在字典中添加这个消息名，并且传⼀个默认的回调进去
            if (!registeredMsgs.ContainsKey(msgName)) {
                registeredMsgs.Add(msgName, _ => { });
            }
            registeredMsgs[msgName] += onMsgReceived; // 实现⼀对多注册
        }

        /// <summary>
        /// 注销同⼀消息名下的所有回调
        /// </summary>
        /// <param name="msgName"></param>
        public static void UnRegisterAll(string msgName) {
            registeredMsgs.Remove(msgName);
        }

        /// <summary>
        /// 注销同⼀消息名下的特定回调
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="onMsgReceived"></param>
        public static void UnRegister(string msgName, Action<object> onMsgReceived) {
            if (registeredMsgs.ContainsKey(msgName)) {
                registeredMsgs[msgName] -= onMsgReceived;
            }
        }

        /// <summary>
        /// 发送事件
        /// </summary>
        /// <param name="msgName"></param>
        /// <param name="data"></param>
        public static void Send(string msgName, object data) {
            if (registeredMsgs.ContainsKey(msgName)) {
                registeredMsgs[msgName](data);
            }
        }

        #region 测试
//#if UNITY_EDITOR
//        [UnityEditor.MenuItem("TDFramework/简易消息机制", false, 13)]
//#endif
//        private static void MenuClicked() {
//            UnRegisterAll("消息1"); // 全部清空，确保测试有效
//            Register("消息1", OnMsgReceived);
//            Register("消息1", OnMsgReceived);
//            Send("消息1", "hello world");
//            UnRegister("消息1", OnMsgReceived);
//            Send("消息1", "hello");
//        }

//        private static void OnMsgReceived(object data) {
//            Debug.LogFormat("消息1:{0}", data);
//        }
        #endregion
    }
}