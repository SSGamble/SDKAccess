/****************************************************
    文件：GameRootDemo2.cs
	作者：TravelerTD
    日期：2019/08/28 17:39:39
	功能：基础资源的异步加载和卸载，示例
*****************************************************/

using UnityEngine;

public class GameRootDemo2 : MonoBehaviour {
    public AudioSource audioSource;
    private AudioClip clip;

    private void Awake() {
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载 AssetBundle 配置表
        ResourceManager.Instance.Init(this); // 异步加载，初始化
    }

    private void Start() {
        ResourceManager.Instance.AsyncLoadResource(ConStr.MENUSOUND, OnLoadFinish, LoadResPriority.RES_MIDDLE); // 异步资源加载
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

    /// <summary>
    /// 异步加载完成后的回调
    /// </summary>
    /// <param name="path"></param>
    /// <param name="obj"></param>
    /// <param name="par1"></param>
    /// <param name="par2"></param>
    /// <param name="par3"></param>
    private void OnLoadFinish(string path, Object obj, object par1, object par2, object par3) {
        clip = obj as AudioClip;
        audioSource.clip = clip;
        audioSource.Play();
    }
}