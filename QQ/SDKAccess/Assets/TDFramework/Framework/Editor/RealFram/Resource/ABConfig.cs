/****************************************************
    文件：ABConfig.cs
	作者：TravelerTD
    日期：2019/8/9 15:48:16
	功能：自定义打包配置表
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ABConfig", menuName = "CreatABConfig", order = 0)]
public class ABConfig : ScriptableObject {
    /// <summary>
    /// 1. 对指定文件夹下所有单个文件进行打包: 单个文件所在的文件夹路径，会遍历这个文件夹下面所有的 Prefab，所以必须保证 Prefab 名字的唯一性
    /// </summary>
    public List<string> m_AllPrefabPath = new List<string>();
    /// <summary>
    /// 2. 对指定文件夹进行打包
    /// </summary>
    public List<FileDirABName> m_AllFileDirAB = new List<FileDirABName>();

    /// <summary>
    /// 因为字典不能序列化所以用了结构体
    /// </summary>
    [System.Serializable]
    public struct FileDirABName {
        public string ABName;
        public string Path;
    }
}
