/****************************************************
    文件：GameRootTest.cs
	作者：TravelerTD
    日期：2019/08/28 15:42:30
	功能：基础资源的同步加载和卸载，示例
*****************************************************/

using UnityEngine;

public class GameRootDemo : MonoBehaviour {
    public AudioSource audioSource;
    private AudioClip clip;

    private void Awake() {
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载 AssetBundle 配置表
    }

    private void Start() {
        clip = ResourceManager.Instance.LoadResource<AudioClip>(ConStr.MENUSOUND); // 同步资源加载
        audioSource.clip = clip;
        audioSource.Play();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            audioSource.Stop();
            audioSource.clip = null; // 防止出现 Miss
            ResourceManager.Instance.ReleaseResouce(clip); // 资源的卸载，不删除缓存
            clip = null; // 因为全局变量对其存在引用
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            audioSource.Stop();
            audioSource.clip = null; // 防止出现 Miss
            ResourceManager.Instance.ReleaseResouce(clip, true); // 资源的卸载，删除缓存
            clip = null; // 因为全局变量对其存在引用
        }
    }
}