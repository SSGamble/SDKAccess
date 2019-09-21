/****************************************************
    文件：SingletonExample.cs
	作者：TravelerTD
    日期：2019/8/7 17:59:18
	功能：单例模板示例
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class SingletonExample : Singleton<SingletonExample> {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/16.SingletonExample", false, 16)]
        private static void MenuClicked() {
            var initInstance = SingletonExample.Instance;
            initInstance = SingletonExample.Instance;
        }
#endif
        private SingletonExample() {
            Debug.Log("SingletonExample ctor");
        }
    }
}