/****************************************************
    文件：MonoSingletonExample.cs
	作者：TravelerTD
    日期：2019/8/7 18:12:51
	功能：MonoBehaviour 单例的模板 示例
*****************************************************/

using UnityEngine;

namespace TDFramework {

    public class MonoSingletonExample : MonoSingleton<MonoSingletonExample> {
#if UNITY_EDITOR
        //[UnityEditor.MenuItem("TDFramework/Example/17.MonoSingletonExample", false, 17)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
        }
#endif
        [RuntimeInitializeOnLoadMethod] // 这个属性修饰的⽅法，会在游戏真正运⾏之后才会调⽤。
        private static void Example() {
            var initInstance = MonoSingletonExample.Instance;
            initInstance = MonoSingletonExample.Instance;
        }
    }
}