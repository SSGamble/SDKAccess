/****************************************************
    文件：GameRootDemo3.cs
	作者：TravelerTD
    日期：2019/08/28 18:03:21
	功能：基础资源预加载，示例
*****************************************************/

using UnityEngine;

public class GameRootDemo3 : MonoBehaviour {
    public AudioSource audioSource;
    private AudioClip clip;

    private void Awake() {
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载 AssetBundle 配置表
    }

    private void Start() {
        ResourceManager.Instance.PreloadRes(ConStr.MENUSOUND); // 基础资源预加载，可注释此行再进行测试，比对时间
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.S)) {
            long time = System.DateTime.Now.Ticks;
            clip = ResourceManager.Instance.LoadResource<AudioClip>(ConStr.MENUSOUND); // 同步资源加载，资源已经预加载
            Debug.Log("预加载的时间：" + (System.DateTime.Now.Ticks - time));
            audioSource.clip = clip;
            audioSource.Play();
        }
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