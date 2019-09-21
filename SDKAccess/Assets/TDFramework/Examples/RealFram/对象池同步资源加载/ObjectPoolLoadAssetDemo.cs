/****************************************************
	文件：ObjectPoolLoadAssetDemo.cs
	作者：TravelerTD
	日期：2019/08/14 15:39   	
	功能：对象池同步资源加载与卸载的演示
*****************************************************/

using UnityEngine;

public class ObjectPoolLoadAssetDemo : MonoBehaviour {

    private GameObject obj;

    private void Awake() {
        GameObject.DontDestroyOnLoad(gameObject); // 跳场景不卸载
        //AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载配置表
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
    }

    private void Start() {
        obj = ObjectManager.Instance.InstantiateObject(ConStr.ATTACK, true); // 同步加载资源，且放到默认节点下
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.D)) {
            obj = ObjectManager.Instance.InstantiateObject(ConStr.ATTACK, true); // 同步加载资源，放到默认的节点下
        }
        if (Input.GetKeyDown(KeyCode.A)) {
            ObjectManager.Instance.ReleaseObject(obj); // 释放资源，不删除缓存，放到回收节点下面
            obj = null;
        }
        if (Input.GetKeyDown(KeyCode.S)) {
            ObjectManager.Instance.ReleaseObject(obj, 0, true, true); // 释放资源，删除缓存
            obj = null;
        }
    }

    private void OnDestroy() {
        ObjectManager.Instance.ClearCache();
        ResourceManager.Instance.ClearCache();
    }

    private void OnApplicationQuit() {
#if UNITY_EDITOR
        ResourceManager.Instance.ClearCache();
        Resources.UnloadUnusedAssets();
        Debug.Log("清空编辑器缓存");
#endif
    }
}
