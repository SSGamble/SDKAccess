/****************************************************
    文件：AssetBundleManager.cs
	作者：TravelerTD
    日期：2019/8/11 14:52:5
	功能：AssetBundle 管理
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class AssetBundleManager : Singleton<AssetBundleManager> {
    /// <summary>
    /// 资源关系依赖配表，可以根据 crc 来找到对应资源块
    /// </summary>
    protected Dictionary<uint, ResouceItem> m_ResouceItemDic = new Dictionary<uint, ResouceItem>();
    /// <summary>
    /// 储存已加载的 AB包，防止重复加载和卸载，key：crc
    /// </summary>
    protected Dictionary<uint, AssetBundleItem> m_AssetBundleItemDic = new Dictionary<uint, AssetBundleItem>();
    /// <summary>
    /// AssetBundleItem 类对象池
    /// </summary>
    protected ClassObjectPool<AssetBundleItem> m_AssetBundleItemPool = ObjectManager.Instance.GetOrCreateClassPool<AssetBundleItem>(500);

    protected string m_ABConfigABName = RealFrameConfig.ABCONFIG_ABNAME;

    protected string ABLoadPath {
        get {
            string path = Application.streamingAssetsPath + "/";
#if UNITY_EDITOR
            path = Application.dataPath + "/../AssetBundle/" + UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString() + "/";
#endif
            return path;
        }
    }

    /// <summary>
    /// 加载 AssetBundle 配置表
    /// </summary>
    /// <returns></returns>
    public bool LoadAssetBundleConfig() {
#if UNITY_EDITOR
        if (!ResourceManager.Instance.m_LoadFormAssetBundle)
            return false;
#endif
        m_ResouceItemDic.Clear();
        string configPath = ABLoadPath + m_ABConfigABName;
        Debug.Log(configPath);
        AssetBundle configAB = AssetBundle.LoadFromFile(configPath);
        TextAsset textAsset = configAB.LoadAsset<TextAsset>(m_ABConfigABName);
        if (textAsset == null) {
            Debug.LogError("AssetBundleConfig is no exist!");
            return false;
        }
        MemoryStream stream = new MemoryStream(textAsset.bytes);
        BinaryFormatter bf = new BinaryFormatter();
        AssetBundleConfig config = (AssetBundleConfig)bf.Deserialize(stream);
        stream.Close();
        // 添加到字典里
        for (int i = 0; i < config.ABList.Count; i++) {
            ABBase abBase = config.ABList[i];
            ResouceItem item = new ResouceItem();
            item.m_Crc = abBase.Crc;
            item.m_AssetName = abBase.AssetName;
            item.m_ABName = abBase.ABName;
            item.m_DependAssetBundle = abBase.ABDependce;
            if (m_ResouceItemDic.ContainsKey(item.m_Crc)) {
                Debug.LogError("重复的 crc 资源名:" + item.m_AssetName + "，ab 包名：" + item.m_ABName);
            }
            else {
                m_ResouceItemDic.Add(item.m_Crc, item);
            }
        }
        return true;
    }

    /// <summary>
    /// 根据路径的 crc 加载中间类 ResourceItem
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResouceItem LoadResouceAssetBundle(uint crc) {
        ResouceItem item = null;
        if (!m_ResouceItemDic.TryGetValue(crc, out item) || item == null) {
            Debug.LogError(string.Format("LoadResourceAssetBundle error: can not find crc {0} in AssetBundleConfig", crc.ToString()));
            return item;
        }
        if (item.m_AssetBundle != null) {
            return item;
        }
        item.m_AssetBundle = LoadAssetBundle(item.m_ABName); // 加载 ab
        if (item.m_DependAssetBundle != null) { // 加载依赖项
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++) {
                LoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }
        return item;
    }

    /// <summary>
    /// 加载单个 assetbundle，根据名字
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private AssetBundle LoadAssetBundle(string name) {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);
        if (!m_AssetBundleItemDic.TryGetValue(crc, out item)) {
            // 加载 ab
            AssetBundle assetBundle = null;
            string fullPath = ABLoadPath + name;
            assetBundle = AssetBundle.LoadFromFile(fullPath);
            if (assetBundle == null) {
                Debug.LogError(" Load AssetBundle Error:" + fullPath);
            }
            // 给 AssetBundleItem 赋值
            item = m_AssetBundleItemPool.Spawn(true);
            item.assetBundle = assetBundle;
            item.RefCount++;
            m_AssetBundleItemDic.Add(crc, item);
        }
        else {
            item.RefCount++;  // 已经加载过了，此次加载就不需要重复加载 ab，而是把引用计数 +1
        }
        return item.assetBundle;
    }

    /// <summary>
    /// 释放资源
    /// </summary>
    /// <param name="item"></param>
    public void ReleaseAsset(ResouceItem item) {
        if (item == null) {
            return;
        }
        if (item.m_DependAssetBundle != null && item.m_DependAssetBundle.Count > 0) { // 有被引用
            // 先卸载依赖项
            for (int i = 0; i < item.m_DependAssetBundle.Count; i++) {
                UnLoadAssetBundle(item.m_DependAssetBundle[i]);
            }
        }
        // 再卸载自己
        UnLoadAssetBundle(item.m_ABName);
    }

    /// <summary>
    /// 卸载 AB
    /// </summary>
    /// <param name="name"></param>
    private void UnLoadAssetBundle(string name) {
        AssetBundleItem item = null;
        uint crc = Crc32.GetCrc32(name);
        if (m_AssetBundleItemDic.TryGetValue(crc, out item) && item != null) {
            item.RefCount--;
            if (item.RefCount <= 0 && item.assetBundle != null) { // 确保没有被引用了
                item.assetBundle.Unload(true); // 卸载
                item.Rest(); // 还原 类对象池
                m_AssetBundleItemPool.Recycle(item);
                m_AssetBundleItemDic.Remove(crc);
            }
        }
    }

    /// <summary>
    /// 根据 crc 查找 ResourceItem
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    public ResouceItem FindResourceItme(uint crc) {
        ResouceItem item = null;
        m_ResouceItemDic.TryGetValue(crc, out item);
        return item;
    }
}

public class AssetBundleItem {
    public AssetBundle assetBundle = null;
    public int RefCount; // 引用计数，有一个依赖就+1，防止ab包的重复加载或卸载

    /// <summary>
    /// 还原 类对象池
    /// </summary>
    public void Rest() {
        assetBundle = null;
        RefCount = 0;
    }
}

/// <summary>
/// 资源块，中间类，缓存
/// </summary>
public class ResouceItem {
    // ab 相关
    public uint m_Crc = 0; // 资源路径的 crc
    public string m_AssetName = string.Empty;  // 该资源的文件名
    public string m_ABName = string.Empty; // 该资源所在的 ab名
    public List<string> m_DependAssetBundle = null; // 该资源所依赖的 AB
    public AssetBundle m_AssetBundle = null; // 该资源加载完的 ab
    // 资源相关
    public Object m_Obj = null; // 加载出来的资源对象
    public int m_Guid = 0; // 资源唯一标识
    public float m_LastUseTime = 0.0f; // 资源最后所使用的时间
    public bool m_Clear = true; // 是否跳场景清掉
    protected int m_RefCount = 0; // 资源引用计数
    public int RefCount {
        get { return m_RefCount; }
        set {
            m_RefCount = value;
            if (m_RefCount < 0) {
                Debug.LogError("refcount < 0" + m_RefCount + " ," + (m_Obj != null ? m_Obj.name : "name is null"));
            }
        }
    }
}
