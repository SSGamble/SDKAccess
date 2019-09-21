/****************************************************
    文件：LevelExample.cs
	作者：TravelerTD
    日期：2019/8/7 17:35:48
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

namespace TDFramework {
    public class LevelExample : MonoBehaviourSimplify {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/15.LevelManager", false, 15)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject().AddComponent<LevelExample>();
        }
#endif
        private void Start() {
            DontDestroyOnLoad(this);
            LevelManager.Init(new List<string> { "Home", "Level" });
            LevelManager.LoadCurrent();
            Delay(10.0f, LevelManager.LoadNext);
        }
        protected override void OnBeforeDestroy() { }
    }
}