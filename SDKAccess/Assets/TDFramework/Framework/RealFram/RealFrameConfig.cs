/****************************************************
	文件：RealFrameConfig.cs
	作者：TravelerTD
	日期：2019/08/27 19:16:49   	
	功能：底层资源加载框架的配置信息
*****************************************************/

using UnityEngine;

public class RealFrameConfig {

    #region 需根据自己项目重设
    /// <summary>
    /// 是否从 AB 加载，true：从 AB 加载，false：从编辑器加载
    /// </summary>
    public const bool loadFormAB = false;

    #region AB 包
    /// <summary>
    /// 打包配置表路径
    /// </summary>
    public const string abConfigPath = "Assets/TDFramework/Framework/Editor/RealFram/Resource/ABConfig.asset";
    /// <summary>
    /// 打包时生成 AB 包配置表的二进制路径
    /// </summary>
    public const string m_ABBytePath = "Assets/TDFramework/GameData/Data/ABData/AssetBundleConfig.bytes";
    #endregion

    #region 数据配置表
    /// <summary>
    /// 存储 Reg 的路径
    /// </summary>
    public const string RegPath = "/../Data/Reg/";
    /// <summary>
    /// 存储 表格 的路径
    /// </summary>
    public const string excelPath = "/../Data/Excel/";
    /// <summary>
    /// xml 文件夹路径
    /// </summary>
    public const string m_XmlPath = "Assets/TDFramework/GameData/Data/Xml/";
    /// <summary>
    /// 二进制文件夹路径
    /// </summary>
    public const string m_BinaryPath = "Assets/TDFramework/GameData/Data/Binary/";
    /// <summary>
    /// 数据信息脚本文件夹路径
    /// </summary>
    public const string m_ScriptsPath = "Assets/TDFramework/Examples/RealFram/DataConfig/Scripts/Data/";
    #endregion
    #endregion

    /// <summary>
    /// ab 包的名字
    /// </summary>
    public const string ABCONFIG_ABNAME = "assetbundleconfig";
    /// <summary>
    /// Build 工程的目标路径
    /// </summary>
    public const string buildPath = "/../BuildTarget/";
}