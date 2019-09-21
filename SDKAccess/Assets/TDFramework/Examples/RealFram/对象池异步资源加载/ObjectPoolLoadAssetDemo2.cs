/****************************************************
	文件：ObjectPoolLoadAssetDemo2.cs
	作者：TravelerTD
	日期：2019/08/14 16:35   	
	功能：对象池异步资源加载，示例
*****************************************************/

using UnityEngine;

public class ObjectPoolLoadAssetDemo2 : MonoBehaviour {
    private GameObject obj;

    private void Awake() {
        GameObject.DontDestroyOnLoad(gameObject); // 跳场景不卸载
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载配置表
        ResourceManager.Instance.Init(this); // 异步加载初始化
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
    }

    private void Start() {
        ObjectManager.Instance.InstantiateObjectAsync(ConStr.ATTACK, OnLoadFinish, LoadResPriority.RES_HIGHT, true); // 异步加载资源
    }

    /// <summary>
    /// 异步加载完成的回调
    /// </summary>
    private void OnLoadFinish(string path, Object o, object param1 = null, object param2 = null, object param3 = null) {
        obj = o as GameObject;
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            ObjectManager.Instance.ReleaseObject(obj); // 释放资源，不删除缓存，放到回收节点下面
            obj = null;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            ObjectManager.Instance.InstantiateObjectAsync(ConStr.ATTACK, OnLoadFinish, LoadResPriority.RES_HIGHT, true); // 异步加载资源
        }
        else if (Input.GetKeyDown(KeyCode.S)) {
            ObjectManager.Instance.ReleaseObject(obj, 0, true, true); // 释放资源，删除缓存
            obj = null;
        }
    }

    private void OnApplicationQuit() {
#if UNITY_EDITOR
        ResourceManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
#endif
    }
}
