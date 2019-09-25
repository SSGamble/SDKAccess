/****************************************************
	文件：OfflineDataEditor.cs
	作者：TravelerTD
	日期：2019/08/18 9:16   	
	功能：离线数据配置的编辑器代码
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OfflineDataEditor {
    /// <summary>
    /// 创建离线数据
    /// </summary>
    /// <param name="obj"></param>
    public static void CreateOfflineData(GameObject obj) {
        OfflineData offlineData = obj.GetComponent<OfflineData>();
        if (offlineData == null) {
            offlineData = obj.AddComponent<OfflineData>();
        }
        offlineData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了 " + obj.name + " prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/离线数据/生成离线数据")]
    public static void AssetCreateOfflineData() {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++) {
            EditorUtility.DisplayProgressBar("添加离线数据，", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateOfflineData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("Assets/离线数据/生成 UI 离线数据")]
    public static void AssetCreateUIData() {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++) {
            EditorUtility.DisplayProgressBar("添加UI离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateUIData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("RealFram/离线数据/生成所有 UI Prefab 离线数据")]
    public static void AllCreateUIData() {
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/UGUI" });
        for (int i = 0; i < allStr.Length; i++) {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("添加 UI 离线数据，", "正在扫描路径：" + prefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null)
                continue;
            CreateUIData(obj);
        }
        Debug.Log("UI 离线数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    public static void CreateUIData(GameObject obj) {
        obj.layer = LayerMask.NameToLayer("UI");
        UIOfflineData uiData = obj.GetComponent<UIOfflineData>();
        if (uiData == null) {
            uiData = obj.AddComponent<UIOfflineData>();
        }
        uiData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了 " + obj.name + " UI prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }

    [MenuItem("Assets/离线数据/生成特效离线数据")]
    public static void AssetCreateEffectData() {
        GameObject[] objects = Selection.gameObjects;
        for (int i = 0; i < objects.Length; i++) {
            EditorUtility.DisplayProgressBar("添加特效离线数据", "正在修改：" + objects[i] + "......", 1.0f / objects.Length * i);
            CreateEffectData(objects[i]);
        }
        EditorUtility.ClearProgressBar();
    }

    [MenuItem("RealFram/离线数据/生成所有 特效 Prefab 离线数据")]
    public static void AllCreateEffectData() {
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/GameData/Prefabs/Effect" });
        for (int i = 0; i < allStr.Length; i++) {
            string prefabPath = AssetDatabase.GUIDToAssetPath(allStr[i]);
            EditorUtility.DisplayProgressBar("添加特效离线数据", "正在扫描路径：" + prefabPath + "......", 1.0f / allStr.Length * i);
            GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (obj == null)
                continue;

            CreateEffectData(obj);
        }
        Debug.Log("特效离线数据全部生成完毕！");
        EditorUtility.ClearProgressBar();
    }

    public static void CreateEffectData(GameObject obj) {
        EffectOfflineData effectData = obj.GetComponent<EffectOfflineData>();
        if (effectData == null) {
            effectData = obj.AddComponent<EffectOfflineData>();
        }

        effectData.BindData();
        EditorUtility.SetDirty(obj);
        Debug.Log("修改了" + obj.name + " 特效 prefab!");
        Resources.UnloadUnusedAssets();
        AssetDatabase.Refresh();
    }
}
