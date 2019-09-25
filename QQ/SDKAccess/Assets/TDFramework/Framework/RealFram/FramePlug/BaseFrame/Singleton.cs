/****************************************************
    文件：Singleton.cs
	作者：TravelerTD
    日期：2019/8/11 14:55:21
	功能：单例基类
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> where T : new() {
    private static T m_Instance;
    public static T Instance {
        get {
            if (m_Instance == null) {
                m_Instance = new T();
            }
            return m_Instance;
        }
    }
}
