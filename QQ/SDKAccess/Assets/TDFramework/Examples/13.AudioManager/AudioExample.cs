/****************************************************
    文件：AudioExample.cs
	作者：TravelerTD
    日期：2019/8/7 16:10:16
	功能：Nothing
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class AudioExample : MonoBehaviourSimplify {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/13.AudioManager", false, 13)]
        private static void MenuClicked() {
            UnityEditor.EditorApplication.isPlaying = true;
            new GameObject("AudioExample").AddComponent<AudioExample>();
        }
#endif

        private void Start() {
            AudioManager.Instance.PlaySound("coin");
            AudioManager.Instance.PlaySound("coin");
            Delay(1.0f, () => {
                AudioManager.Instance.PlayMusic("home", true);
            });
            Delay(3.0f, () => { AudioManager.Instance.PauseMusic(); });
            Delay(5.0f, () => { AudioManager.Instance.ResumeMusic(); });
            Delay(7.0f, () => { AudioManager.Instance.StopMusic(); });
            Delay(9.0f, () => {
                AudioManager.Instance.PlayMusic("home", true);
            });
            Delay(11.0f, () => { AudioManager.Instance.MusicOff(); });
            Delay(13.0f, () => { AudioManager.Instance.MusicOn(); });
        }

        protected override void OnBeforeDestroy() { }
    }
}