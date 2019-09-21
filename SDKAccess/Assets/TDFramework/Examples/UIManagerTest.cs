/****************************************************
    文件：UIManagerTest.cs
	作者：TravelerTD
    日期：2019/8/7 15:3:4
	功能：Nothing
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class UIManagerTest : MonoBehaviourSimplify {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("QFramework/Example/GUIManager", false, 11)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("GUIExample").AddComponent<UIManagerTest>();
        }
#endif
        private void Start() {
            GUIManager.SetResolution(1280, 720, 0);
            GUIManager.LoadPanel("UIRoot", UILayer.Common);
            Delay(3.0f, () => {
                GUIManager.UnLoadPanel("UIRoot");
            });
        }
        protected override void OnBeforeDestroy() { }
    }
}