/****************************************************
	文件：PreloadLoadDemo.cs
	作者：TravelerTD
	日期：2019/08/29 10:08:50   	
	功能：对象池预加载，示例
*****************************************************/

using UnityEngine;

public class PreloadLoadDemo : MonoBehaviour {
    private GameObject obj;

    private void Awake() {
        GameObject.DontDestroyOnLoad(gameObject); // 跳场景不卸载
        AssetBundleManager.Instance.LoadAssetBundleConfig(); // 加载配置表
        ObjectManager.Instance.Init(transform.Find("RecyclePoolTrs"), transform.Find("SceneTrs")); // 归置对象池对象位置
    }

    private void Start() {
        ObjectManager.Instance.PreloadGameObject(ConStr.ATTACK, 20); // 预加载资源
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            ObjectManager.Instance.ReleaseObject(obj); // 释放资源，不删除缓存，放到回收节点下面
            obj = null;
        }
        else if (Input.GetKeyDown(KeyCode.D)) {
            obj = ObjectManager.Instance.InstantiateObject(ConStr.ATTACK, true); // 同步加载资源，放到默认的节点下
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
