/****************************************************
	文件：EditorConfig.cs
	作者：TravelerTD
	日期：2019/08/28 10:09:57   	
	功能：编辑器扩展
*****************************************************/

using UnityEditor;
using UnityEngine;

public class EditorConfig : MonoBehaviour {

    [MenuItem("RealFram/打 AB 包")]
    public static void BuildABEditor() {
        BundleEditor.Build();
    }

    [MenuItem("RealFram/Build 标准包")]
    public static void BuildAppEditor() {
        BuildApp.Build();
    }

    #region 数据配置表相关
    [MenuItem("RealFram/序列化/所有 Xml 转成 Binary")]
    public static void AllXmlToBinary() {
        DataConfigEditor.AllXmlToBinary();
    }

    [MenuItem("RealFram/序列化/所有 Excel 转 Xml")]
    public static void AllExcelToXml() {
        DataConfigEditor.AllExcelToXml();
    }

    [MenuItem("Assets/序列化/类 转 Xml")]
    public static void AssetsClassToXml() {
        DataConfigEditor.AssetsClassToXml();
    }

    [MenuItem("Assets/序列化/Xml 转 Binary")]
    public static void AssetsXmlToBinary() {
        DataConfigEditor.AssetsXmlToBinary();
    }

    [MenuItem("Assets/序列化/Xml 转 Excel")]
    public static void AssetsXmlToExcel() {
        DataConfigEditor.AssetsXmlToExcel();
    }
    #endregion

    
}