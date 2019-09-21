/****************************************************
    文件：ResSvc.cs
	作者：TravelerTD
    日期：2019/09/01 22:05:17
	功能：资源加载服务
*****************************************************/

using TDFramework;
using UnityEngine;

public class ResSvc : MonoSingleton<ResSvc> {

    /// <summary>
    /// 初始化资源加载服务
    /// </summary>
    public void InitSvc() {
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载 AssetBundle 配置表
        ResourceManager.Instance.Init(this); // 异步加载，初始化
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
        TDLog.Log("Init ResSvc..");
    }

    /// <summary>
    /// 加载声音资源
    /// </summary>
    /// <param name="path">声音路径</param>
    /// <returns></returns>
    public AudioClip LoadAudio(string path) {
        return ResourceManager.Instance.LoadResource<AudioClip>(path);
    }



}