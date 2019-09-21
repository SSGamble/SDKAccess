/****************************************************
    文件：MonoSingleton.cs
	作者：TravelerTD
    日期：2019/8/7 18:8:9
	功能：MonoBehaviour 单例的模板
*****************************************************/
using UnityEngine;

namespace TDFramework {
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
        protected static T mInstance = null;

        public static T Instance {
            get {
                if (mInstance == null) {
                    mInstance = FindObjectOfType<T>();
                    // 约束 GameObject 的个数，这就不叫单例了
                    if (FindObjectsOfType<T>().Length > 1) {
                        Debug.LogWarning("More than 1");
                        return mInstance;
                    }
                    if (mInstance == null) {
                        var instanceName = typeof(T).Name;
                        Debug.LogFormat("Instance Name: {0}", instanceName);
                        var instanceObj = GameObject.Find(instanceName);
                        if (!instanceObj) {
                            instanceObj = new GameObject(instanceName);
                        }
                        mInstance = instanceObj.AddComponent<T>();
                        DontDestroyOnLoad(instanceObj); // 保证实例不会被释放
                        Debug.LogFormat("Add New Singleton {0} in Game!", instanceName);
                    }
                    else {
                        Debug.LogFormat("Already exist: {0}", mInstance.name);
                    }
                }
                return mInstance;
            }
        }

        /// <summary>
        /// 在脚本销毁时，把静态实例置空
        /// </summary>
        protected virtual void OnDestroy() {
            mInstance = null;
        }
    }
}