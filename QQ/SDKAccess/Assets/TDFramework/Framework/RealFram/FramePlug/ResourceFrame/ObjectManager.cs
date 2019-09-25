/****************************************************
    文件：ObjectManager.cs
	作者：TravelerTD
    日期：2019/8/11 14:53:2
	功能：基于 ResourceManager 的对象管理
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ObjectManager : Singleton<ObjectManager> {
    /// <summary>
    /// 对象池节点，所有的对象池对象都会放在这下面
    /// </summary>
    public Transform RecyclePoolTrs;
    /// <summary>
    /// 场景节点
    /// </summary>
    public Transform SceneTrs;
    /// <summary>
    /// 对象池，key：crc
    /// </summary>
    protected Dictionary<uint, List<ResourceObj>> m_ObjectPoolDic = new Dictionary<uint, List<ResourceObj>>();
    /// <summary>
    /// 暂存 ResObj 的 Dic
    /// </summary>
    protected Dictionary<int, ResourceObj> m_ResourceObjDic = new Dictionary<int, ResourceObj>();
    /// <summary>
    /// ReourceObj 的类对象池
    /// </summary>
    protected ClassObjectPool<ResourceObj> m_ResourceObjClassPool = null;
    /// <summary>
    /// 根据异步的 guid 储存 ResourceObj，来判断是否正在异步加载
    /// </summary>
    protected Dictionary<long, ResourceObj> m_AsyncResObjs = new Dictionary<long, ResourceObj>();

    /// <summary>
    /// 初始化，创建类对象池，归置对象池对象位置
    /// </summary>
    /// <param name="recycleTrs">回收节点</param>
    /// <param name="sceneTrs">场景默认节点</param>
    public void Init(Transform recycleTrs, Transform sceneTrs) {
        m_ResourceObjClassPool = GetOrCreateClassPool<ResourceObj>(1000);
        RecyclePoolTrs = recycleTrs;
        SceneTrs = sceneTrs;
    }

    /// <summary>
    /// 清空对象池
    /// </summary>
    public void ClearCache() {
        List<uint> tempList = new List<uint>();
        foreach (uint key in m_ObjectPoolDic.Keys) {
            List<ResourceObj> st = m_ObjectPoolDic[key];
            for (int i = st.Count - 1; i >= 0; i--) {
                ResourceObj resObj = st[i];
                if (!System.Object.ReferenceEquals(resObj.m_CloneObj, null) && resObj.m_bClear) {
                    GameObject.Destroy(resObj.m_CloneObj);
                    m_ResourceObjDic.Remove(resObj.m_CloneObj.GetInstanceID());
                    resObj.Reset();
                    m_ResourceObjClassPool.Recycle(resObj);
                    st.Remove(resObj);
                }
            }
            if (st.Count <= 0) {
                tempList.Add(key);
            }
        }
        for (int i = 0; i < tempList.Count; i++) {
            uint temp = tempList[i];
            if (m_ObjectPoolDic.ContainsKey(temp)) {
                m_ObjectPoolDic.Remove(temp);
            }
        }
        tempList.Clear();
    }

    /// <summary>
    /// 清除某个资源在对象池中所有的对象
    /// </summary>
    /// <param name="crc"></param>
    public void ClearPoolObject(uint crc) {
        List<ResourceObj> st = null;
        if (!m_ObjectPoolDic.TryGetValue(crc, out st) || st == null)
            return;
        for (int i = st.Count - 1; i >= 0; i--) {
            ResourceObj resObj = st[i];
            if (resObj.m_bClear) {
                st.Remove(resObj);
                int tempID = resObj.m_CloneObj.GetInstanceID();
                GameObject.Destroy(resObj.m_CloneObj);
                resObj.Reset();
                m_ResourceObjDic.Remove(tempID);
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }
        if (st.Count <= 0) {
            m_ObjectPoolDic.Remove(crc);
        }
    }

    /// <summary>
    /// 根据实例化对象直接获取离线数据
    /// </summary>
    /// <param name="obj">GameObject</param>
    /// <returns></returns>
    public OfflineData FindOfflineData(GameObject obj) {
        OfflineData data = null;
        ResourceObj resObj = null;
        m_ResourceObjDic.TryGetValue(obj.GetInstanceID(), out resObj);
        if (resObj != null) {
            data = resObj.m_OfflineData;
        }
        return data;
    }

    /// <summary>
    /// 从对象池取对象
    /// </summary>
    /// <param name="crc"></param>
    /// <returns></returns>
    protected ResourceObj GetObjectFromPool(uint crc) {
        List<ResourceObj> st = null;
        if (m_ObjectPoolDic.TryGetValue(crc, out st) && st != null && st.Count > 0) {
            ResourceManager.Instance.IncreaseResouceRef(crc); // ResourceManager 引用计数 +1
            ResourceObj resObj = st[0];
            st.RemoveAt(0);
            GameObject obj = resObj.m_CloneObj;
            if (!System.Object.ReferenceEquals(obj, null)) {
                // 还原离线数据
                if (!System.Object.ReferenceEquals(resObj.m_OfflineData, null)) {
                    resObj.m_OfflineData.ResetProp();
                }
                resObj.m_Already = false;
#if UNITY_EDITOR
                // 在编辑器下进行改名
                if (obj.name.EndsWith("(Recycle)")) {
                    obj.name = obj.name.Replace("(Recycle)", "");
                }
#endif
            }
            return resObj;
        }
        return null;
    }

    /// <summary>
    /// 取消异步加载
    /// </summary>
    /// <param name="guid"></param>
    public void CancleLoad(long guid) {
        ResourceObj resObj = null;
        if (m_AsyncResObjs.TryGetValue(guid, out resObj) && ResourceManager.Instance.CancleLoad(resObj)) {
            m_AsyncResObjs.Remove(guid);
            // 直接回收
            resObj.Reset();
            m_ResourceObjClassPool.Recycle(resObj);
        }
    }

    /// <summary>
    /// 是否正在异步加载
    /// </summary>
    /// <param name="guid"></param>
    /// <returns></returns>
    public bool IsingAsyncLoad(long guid) {
        return m_AsyncResObjs[guid] != null;
    }

    /// <summary>
    /// 该对象是否是对象池创建的
    /// </summary>
    /// <returns></returns>
    public bool IsObjectManagerCreate(GameObject obj) {
        ResourceObj resObj = m_ResourceObjDic[obj.GetInstanceID()];
        return resObj == null ? false : true;
    }

    /// <summary>
    /// 预加载 GamObject，供外界调用
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="count">预加载个数</param>
    /// <param name="clear">跳场景是否清除</param>
    public void PreloadGameObject(string path, int count = 1, bool clear = false) {
        List<GameObject> tempGameObjectList = new List<GameObject>();
        for (int i = 0; i < count; i++) {
            GameObject obj = InstantiateObject(path, false, bClear: clear);
            tempGameObjectList.Add(obj);
        }
        for (int i = 0; i < count; i++) {
            GameObject obj = tempGameObjectList[i];
            ReleaseObject(obj);
            obj = null;
        }
        tempGameObjectList.Clear();
    }

    /// <summary>
    /// 同步对象加载，供外界调用
    /// </summary>
    /// <param name="path">资源路径</param>
    /// <param name="setSceneObj">是否要放到默认的节点下</param>
    /// <param name="bClear">跳转场景时是否清除</param>
    /// <returns></returns>
    public GameObject InstantiateObject(string path, bool setSceneObj = false, bool bClear = true) {
        uint crc = Crc32.GetCrc32(path);
        ResourceObj resourceObj = GetObjectFromPool(crc);
        if (resourceObj == null) {
            resourceObj = m_ResourceObjClassPool.Spawn(true);
            resourceObj.m_Crc = crc;
            resourceObj.m_bClear = bClear;
            // ResouceManager 提供加载方法
            resourceObj = ResourceManager.Instance.LoadResource(path, resourceObj);
            if (resourceObj.m_ResItem.m_Obj != null) {
                resourceObj.m_CloneObj = GameObject.Instantiate(resourceObj.m_ResItem.m_Obj) as GameObject;
                resourceObj.m_OfflineData = resourceObj.m_CloneObj.GetComponent<OfflineData>(); // 获取离线数据
            }
        }
        // 放置到指定节点下面
        if (setSceneObj) {
            resourceObj.m_CloneObj.transform.SetParent(SceneTrs, false);
        }
        // 加到暂存 Dic
        int tempID = resourceObj.m_CloneObj.GetInstanceID();
        if (!m_ResourceObjDic.ContainsKey(tempID)) {
            m_ResourceObjDic.Add(tempID, resourceObj);
        }
        return resourceObj.m_CloneObj;
    }

    /// <summary>
    /// 异步对象加载，供外界调用
    /// </summary>
    /// <param name="path"></param>
    /// <param name="dealFinish">异步加载，资源加载完成的回调</param>
    /// <param name="priority">资源加载的优先级</param>
    /// <param name="setSceneObject">是否放到默认节点下</param>
    /// <param name="param1">可能的参数</param>
    /// <param name="param2">可能的参数</param>
    /// <param name="param3">可能的参数</param>
    /// <param name="bClear">是否跳场景清除</param>
    /// <returns>返回唯一的 GUID，可以利用该标识取消异步加载</returns>
    public long InstantiateObjectAsync(string path, OnAsyncObjFinish dealFinish, LoadResPriority priority, bool setSceneObject = false, object param1 = null, object param2 = null, object param3 = null, bool bClear = true) {
        if (string.IsNullOrEmpty(path)) {
            return 0;
        }
        uint crc = Crc32.GetCrc32(path);
        ResourceObj resObj = GetObjectFromPool(crc);
        if (resObj != null) {
            // 放到默认节点下
            if (setSceneObject) {
                resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
            }
            // 执行回调
            if (dealFinish != null) {
                dealFinish(path, resObj.m_CloneObj, param1, param2, param3);
            }
            return resObj.m_Guid;
        }
        long guid = ResourceManager.Instance.CreatGuid();
        resObj = m_ResourceObjClassPool.Spawn(true);
        resObj.m_Crc = crc;
        resObj.m_SetSceneParent = setSceneObject;
        resObj.m_bClear = bClear;
        resObj.m_DealFinish = dealFinish;
        resObj.m_Param1 = param1;
        resObj.m_Param2 = param2;
        resObj.m_Param3 = param3;
        // 调用 ResouceManager 的异步加载接口
        ResourceManager.Instance.AsyncLoadResource(path, resObj, OnLoadResourceObjFinish, priority);
        return guid;
    }

    /// <summary>
    /// 资源加载完成的回调
    /// </summary>
    /// <param name="path">路径</param>
    /// <param name="resObj">中间类</param>
    /// <param name="param1">参数1</param>
    /// <param name="param2">参数2</param>
    /// <param name="param3">参数3</param>
    void OnLoadResourceObjFinish(string path, ResourceObj resObj, object param1 = null, object param2 = null, object param3 = null) {
        if (resObj == null)
            return;
        if (resObj.m_ResItem.m_Obj == null) {
#if UNITY_EDITOR
            Debug.LogError("异步资源加载的资源为空：" + path);
#endif
        }
        else {
            resObj.m_CloneObj = GameObject.Instantiate(resObj.m_ResItem.m_Obj) as GameObject;
            resObj.m_OfflineData = resObj.m_CloneObj.GetComponent<OfflineData>(); // 获取离线数据
        }
        if (m_AsyncResObjs.ContainsKey(resObj.m_Guid)) { // 如果加载完成就从正在加载的异步中移除
            m_AsyncResObjs.Remove(resObj.m_Guid);
        }
        if (resObj.m_CloneObj != null && resObj.m_SetSceneParent) {
            resObj.m_CloneObj.transform.SetParent(SceneTrs, false);
        }
        if (resObj.m_DealFinish != null) {
            int tempID = resObj.m_CloneObj.GetInstanceID();
            if (!m_ResourceObjDic.ContainsKey(tempID)) {
                m_ResourceObjDic.Add(tempID, resObj);
            }
            resObj.m_DealFinish(path, resObj.m_CloneObj, resObj.m_Param1, resObj.m_Param2, resObj.m_Param3);
        }
    }

    /// <summary>
    /// 释放对象资源，供外界调用
    /// </summary>
    /// <param name="obj">资源对象</param>
    /// <param name="maxCacheCount">最大缓存个数，-1：不限，0：不缓存</param>
    /// <param name="destoryCache">是否删除缓存</param>
    /// <param name="recycleParent">是否要回收到父节点下</param>
    public void ReleaseObject(GameObject obj, int maxCacheCount = -1, bool destoryCache = false, bool recycleParent = true) {
        if (obj == null)
            return;
        ResourceObj resObj = null;
        int tempID = obj.GetInstanceID();
        if (!m_ResourceObjDic.TryGetValue(tempID, out resObj)) {
            Debug.Log(obj.name + " 对象不是 ObjectManager 创建的！");
            return;
        }
        if (resObj == null) {
            Debug.LogError("缓存的 ResourceObj 为空！");
            return;
        }
        if (resObj.m_Already) {
            Debug.LogError("该对象已经放回对象池了，检测自己是否清空引用!");
            return;
        }
#if UNITY_EDITOR
        // 在编辑器下进行改名
        obj.name += "(Recycle)";
#endif
        List<ResourceObj> st = null;
        // 不缓存，直接回收
        if (maxCacheCount == 0) {
            m_ResourceObjDic.Remove(tempID);
            ResourceManager.Instance.ReleaseResouce(resObj, destoryCache);
            resObj.Reset();
            m_ResourceObjClassPool.Recycle(resObj);
        }
        // 回收到对象池
        else {
            if (!m_ObjectPoolDic.TryGetValue(resObj.m_Crc, out st) || st == null) {
                st = new List<ResourceObj>();
                m_ObjectPoolDic.Add(resObj.m_Crc, st);
            }
            if (resObj.m_CloneObj) {
                if (recycleParent) { // 回收到父节点
                    resObj.m_CloneObj.transform.SetParent(RecyclePoolTrs);
                }
                else {
                    resObj.m_CloneObj.SetActive(false);
                }
            }
            // 没有达到最大缓存个数
            if (maxCacheCount < 0 || st.Count < maxCacheCount) {
                st.Add(resObj);
                resObj.m_Already = true;
                // ResourceManager 引用计数 -1
                ResourceManager.Instance.DecreaseResoucerRef(resObj);
            }
            // 达到最大缓存个数，直接回收
            else {
                m_ResourceObjDic.Remove(tempID);
                ResourceManager.Instance.ReleaseResouce(resObj, destoryCache);
                resObj.Reset();
                m_ResourceObjClassPool.Recycle(resObj);
            }
        }
    }

    #region 类对象池的使用
    // 类对象池的字典，key：类型，val：ClassObjectPool
    protected Dictionary<Type, object> m_ClassPoolDic = new Dictionary<Type, object>();

    /// <summary>
    /// 创建类对象池，创建完成后外面可以保存 ClassObjectPool<T> ，然后调用 Spawn 和 Recycle 来创建和回收对象
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="maxCount"></param>
    /// <returns></returns>
    public ClassObjectPool<T> GetOrCreateClassPool<T>(int maxcount) where T : class, new() {
        Type type = typeof(T);
        object outObj = null;
        if (!m_ClassPoolDic.TryGetValue(type, out outObj) || outObj == null) {
            ClassObjectPool<T> newPool = new ClassObjectPool<T>(maxcount);
            m_ClassPoolDic.Add(type, newPool);
            return newPool;
        }

        return outObj as ClassObjectPool<T>;
    }
    #endregion
}
