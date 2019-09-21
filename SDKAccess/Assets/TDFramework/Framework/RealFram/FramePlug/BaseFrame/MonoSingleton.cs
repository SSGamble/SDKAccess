/****************************************************
	文件：MonoSingleton.cs
	作者：TravelerTD
	日期：2019/08/19 10:26   	
	功能：MonoBehaviour 的单例模板
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
    protected static T instance;

    public static T Instance {
        get { return instance; }
    }

    protected virtual void Awake() {
        if (instance == null) {
            instance = (T)this;
        }
        else {
            Debug.LogError("Get a second instance of this class " + this.GetType());
        }
    }
}
