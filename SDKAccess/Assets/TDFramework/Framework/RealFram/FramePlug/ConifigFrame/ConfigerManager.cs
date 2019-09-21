/****************************************************
	文件：ConfigerManager.cs
	作者：TravelerTD
	日期：2019/08/19 16:07:07   	
	功能：数据配置表管理
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 数据配置表路径
/// </summary>
public class CFG {
    public const string TABLE_MONSTER = RealFrameConfig.m_BinaryPath + "MonsterData.bytes";
    public const string TABLE_BUFF = RealFrameConfig.m_BinaryPath + "BuffData.bytes";
}

public class ConfigerManager : Singleton<ConfigerManager> {
    /// <summary>
    /// 储存所有已经加载的配置表
    /// </summary>
    protected Dictionary<string, ExcelBase> m_AllExcelData = new Dictionary<string, ExcelBase>();

    /// <summary>
    /// 加载配置表数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path">二进制路径</param>
    /// <returns></returns>
    public T LoadData<T>(string path) where T : ExcelBase {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }
        // 已经加载过
        if (m_AllExcelData.ContainsKey(path)) {
            Debug.LogError("重复加载相同配置文件：" + path);
            return m_AllExcelData[path] as T;
        }
        // 初次加载
        T data = BinarySerializeOpt.BinaryDeserilize<T>(path); // 二进制转类
#if UNITY_EDITOR
        if (data == null) { // 容错，如果二进制文件不存在，尝试从 XML 文件进行读取
            Debug.Log(path + " 不存在，从 xml 加载数据了！");
            string xmlPath = path.Replace("Binary", "Xml").Replace(".bytes", ".xml"); // 根据二进制路径推测出 xml 路径，需要二进制文件和 xml 文件，文件名一置
            data = BinarySerializeOpt.XmlDeserialize<T>(xmlPath); // XML 转类
        }
#endif
        // 加载后，进行初始化
        if (data != null) {
            data.Init();
        }
        m_AllExcelData.Add(path, data);
        return data;
    }

    /// <summary>
    /// 根据路径查找数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="path"></param>
    /// <returns></returns>
    public T FindData<T>(string path) where T : ExcelBase {
        if (string.IsNullOrEmpty(path))
            return null;
        ExcelBase excelBase = null;
        if (m_AllExcelData.TryGetValue(path, out excelBase)) {
            return excelBase as T;
        }
        else {
            excelBase = LoadData<T>(path);
        }
        return (T)excelBase;
    }
}