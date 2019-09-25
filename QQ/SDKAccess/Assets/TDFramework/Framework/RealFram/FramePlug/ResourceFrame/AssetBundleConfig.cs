/****************************************************
    文件：AssetBundleConfig.cs
	作者：TravelerTD
    日期：2019/8/9 18:26:49
	功能：包配置表的数据结构
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class AssetBundleConfig {
    [XmlElement("ABList")]
    public List<ABBase> ABList { get; set; }
}

[System.Serializable]
public class ABBase {
    [XmlAttribute("Path")]
    public string Path { get; set; } // 路径
    [XmlAttribute("Crc")]
    public uint Crc { get; set; } // 根据路径生成的 文件唯一标识
    [XmlAttribute("ABName")]
    public string ABName { get; set; } // AB 包名
    [XmlAttribute("AssetName")]
    public string AssetName { get; set; } // 资源名，一个 AB包 下可能会有多个资源
    [XmlElement("ABDependce")]
    public List<string> ABDependce { get; set; } // 一个 AB包 所依赖的其他 AB包，依赖加载
}
