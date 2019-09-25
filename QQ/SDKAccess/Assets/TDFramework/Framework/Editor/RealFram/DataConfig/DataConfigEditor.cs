/****************************************************
	文件：DataEditor.cs
	作者：TravelerTD
	日期：2019/08/19 16:33:37   	
	功能：数据配置表
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Xml;
using OfficeOpenXml;
using System.ComponentModel;

public class DataConfigEditor {
    /// <summary>
    /// 存储 XML 的路径
    /// </summary>
    public static string XmlPath = RealFrameConfig.m_XmlPath;
    /// <summary>
    /// 存储 二进制 的路径
    /// </summary>
    public static string BinaryPath = RealFrameConfig.m_BinaryPath;
    /// <summary>
    /// 存储 脚本 的路径
    /// </summary>
    public static string ScriptsPath = RealFrameConfig.m_ScriptsPath;
    /// <summary>
    /// 存储 表格 的路径
    /// </summary>
    public static string ExcelPath = Application.dataPath + RealFrameConfig.excelPath;
    /// <summary>
    /// 存储 Reg 的路径
    /// </summary>
    public static string RegPath = Application.dataPath + RealFrameConfig.RegPath;

    //[MenuItem("Assets/序列化/类 转 Xml")]
    public static void AssetsClassToXml() {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++) { // 遍历选中的东西
            EditorUtility.DisplayProgressBar("文件下的类转成 xml，", "正在扫描 " + objs[i].name + "... ...", 1.0f / objs.Length * i);
            ClassToXml(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("Assets/序列化/Xml 转 Binary")]
    public static void AssetsXmlToBinary() {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++) {
            EditorUtility.DisplayProgressBar("文件下的 xml 转成 二进制，", "正在扫描" + objs[i].name + "... ...", 1.0f / objs.Length * i);
            XmlToBinary(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("Assets/序列化/Xml 转 Excel")]
    public static void AssetsXmlToExcel() {
        UnityEngine.Object[] objs = Selection.objects;
        for (int i = 0; i < objs.Length; i++) {
            EditorUtility.DisplayProgressBar("文件下的 xml 转成 Excel ", "正在扫描 " + objs[i].name + "... ...", 1.0f / objs.Length * i);
            XmlToExcel(objs[i].name);
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("RealFram/序列化/所有 Xml 转成 Binary")]
    public static void AllXmlToBinary() {
        string path = Application.dataPath.Replace("Assets", "") + XmlPath; // XML 目录文件夹
        string[] filesPath = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories); // XML 文件夹下的所有文件路径
        for (int i = 0; i < filesPath.Length; i++) {
            EditorUtility.DisplayProgressBar("查找文件夹下面的 Xml，", "正在扫描" + filesPath[i] + "... ...", 1.0f / filesPath.Length * i);
            if (filesPath[i].EndsWith(".xml")) { // xml 文件，过滤 meta 文件
                string tempPath = filesPath[i].Substring(filesPath[i].LastIndexOf("/") + 1);
                tempPath = tempPath.Replace(".xml", "");
                XmlToBinary(tempPath);
            }
        }
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    //[MenuItem("RealFram/序列化/所有 Excel 转 Xml")]
    public static void AllExcelToXml() {
        string[] filePaths = Directory.GetFiles(RegPath, "*", SearchOption.AllDirectories);
        for (int i = 0; i < filePaths.Length; i++) {
            if (!filePaths[i].EndsWith(".xml"))
                continue;
            EditorUtility.DisplayProgressBar("查找文件夹下的类", "正在扫描路径" + filePaths[i] + "... ...", 1.0f / filePaths.Length * i);
            string path = filePaths[i].Substring(filePaths[i].LastIndexOf("/") + 1);
            ExcelToXml(path.Replace(".xml", ""));
        }

        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// Excel 转 XML
    /// </summary>
    /// <param name="name"></param>
    private static void ExcelToXml(string name) {
        string className = "";
        string xmlName = "";
        string excelName = "";
        // 1. 读取 Reg 文件，确定类的结构
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className);
        // 2. 读取 Excel 里面的数据
        string excelPath = ExcelPath + excelName;
        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>();
        try {
            using (FileStream stream = new FileStream(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
                using (ExcelPackage package = new ExcelPackage(stream)) {
                    ExcelWorksheets worksheetArray = package.Workbook.Worksheets; // 所有的工作表
                    for (int i = 0; i < worksheetArray.Count; i++) { // 遍历工作表
                        SheetData sheetData = new SheetData();
                        ExcelWorksheet worksheet = worksheetArray[i + 1]; // Excel 都是从 1 开始，而不是从 0 开始
                        SheetClass sheetClass = allSheetClassDic[worksheet.Name];
                        int colMaxCount = worksheet.Dimension.End.Column; // 最大列
                        int rowMaxCount = worksheet.Dimension.End.Row; // 最大行
                        // 列名和该列的数据类型，变量和变量类型
                        for (int n = 0; n < sheetClass.VarList.Count; n++) {
                            sheetData.AllName.Add(sheetClass.VarList[n].Name);
                            sheetData.AllType.Add(sheetClass.VarList[n].Type);
                        }
                        // 遍历行，数据
                        for (int m = 1; m < rowMaxCount; m++) { // 第一行是列名
                            RowData rowData = new RowData();
                            int n = 0;
                            if (string.IsNullOrEmpty(sheetClass.SplitStr) && sheetClass.ParentVar != null
                                && !string.IsNullOrEmpty(sheetClass.ParentVar.Foregin)) {
                                rowData.ParnetVlue = worksheet.Cells[m + 1, 1].Value.ToString().Trim();
                                n = 1;
                            }
                            for (; n < colMaxCount; n++) {
                                ExcelRange range = worksheet.Cells[m + 1, n + 1];
                                string value = "";
                                if (range.Value != null) {
                                    value = range.Value.ToString().Trim();
                                }
                                string colValue = worksheet.Cells[1, n + 1].Value.ToString().Trim(); // 列名
                                rowData.RowDataDic.Add(GetNameFormCol(sheetClass.VarList, colValue), value);
                            }
                            sheetData.AllData.Add(rowData);
                        }
                        sheetDataDic.Add(worksheet.Name, sheetData);
                    }
                }
            }
        }
        catch (Exception e) {
            Debug.LogError(e);
            return;
        }
        // 3. 根据类的结构，创建类，并且给每个变量赋值（从 excel 里读出来的值）
        object objClass = CreateClass(className);
        List<string> outKeyList = new List<string>();
        foreach (string str in allSheetClassDic.Keys) {
            SheetClass sheetClass = allSheetClassDic[str];
            if (sheetClass.Depth == 1) {
                outKeyList.Add(str);
            }
        }
        for (int i = 0; i < outKeyList.Count; i++) {
            ReadDataToClass(objClass, allSheetClassDic[outKeyList[i]], sheetDataDic[outKeyList[i]], allSheetClassDic, sheetDataDic, null);
        }
        BinarySerializeOpt.Xmlserialize(XmlPath + xmlName, objClass);
        //BinarySerializeOpt.BinarySerilize(BinaryPath + className + ".bytes", objClass);
        Debug.Log(excelName + "表导入unity完成！");
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 把数据递归的写到类里面
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="sheetClass"></param>
    /// <param name="sheetData"></param>
    /// <param name="allSheetClassDic"></param>
    /// <param name="sheetDataDic"></param>
    /// <param name="keyValue"></param>
    private static void ReadDataToClass(object objClass, SheetClass sheetClass, SheetData sheetData, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, object keyValue) {
        object item = CreateClass(sheetClass.Name); // 只是为了得到变量类型
        object list = CreateList(item.GetType());
        for (int i = 0; i < sheetData.AllData.Count; i++) { // 给类里面填数据
            if (keyValue != null && !string.IsNullOrEmpty(sheetData.AllData[i].ParnetVlue)) {
                if (sheetData.AllData[i].ParnetVlue != keyValue.ToString())
                    continue;
            }
            object addItem = CreateClass(sheetClass.Name);
            for (int j = 0; j < sheetClass.VarList.Count; j++) {
                VarClass varClass = sheetClass.VarList[j];
                if (varClass.Type == "list" && string.IsNullOrEmpty(varClass.SplitStr)) {
                    ReadDataToClass(addItem, allSheetClassDic[varClass.ListSheetName], sheetDataDic[varClass.ListSheetName], allSheetClassDic, sheetDataDic, GetMemberValue(addItem, sheetClass.MainKey));
                }
                else if (varClass.Type == "list") {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitClass(addItem, allSheetClassDic[varClass.ListSheetName], value);
                }
                else if (varClass.Type == "listStr" || varClass.Type == "listFloat" || varClass.Type == "listInt" || varClass.Type == "listBool") {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    SetSplitBaseClass(addItem, varClass, value);
                }
                else {
                    string value = sheetData.AllData[i].RowDataDic[sheetData.AllName[j]];
                    if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(varClass.DeafultValue)) {
                        value = varClass.DeafultValue;
                    }
                    if (string.IsNullOrEmpty(value)) {
                        Debug.LogError("表格中有空数据，或者 Reg 文件未配置 defaultValue！" + sheetData.AllName[j]);
                        continue;
                    }
                    SetValue(addItem.GetType().GetProperty(sheetData.AllName[j]), addItem, value, sheetData.AllType[j]);
                }
            }
            list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 自定义 List<类> 赋值
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="sheetClass"></param>
    /// <param name="value"></param>
    private static void SetSplitClass(object objClass, SheetClass sheetClass, string value) {
        object item = CreateClass(sheetClass.Name);
        object list = CreateList(item.GetType());
        if (string.IsNullOrEmpty(value)) {
            Debug.Log("excel 里面自定义 list 的列里有空值！" + sheetClass.Name);
            return;
        }
        else {
            string splitStr = sheetClass.ParentVar.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            string[] rowArray = value.Split(new string[] { splitStr }, StringSplitOptions.None);
            for (int i = 0; i < rowArray.Length; i++) {
                object addItem = CreateClass(sheetClass.Name);
                string[] valueList = rowArray[i].Trim().Split(new string[] { sheetClass.SplitStr }, StringSplitOptions.None);
                for (int j = 0; j < valueList.Length; j++) {
                    SetValue(addItem.GetType().GetProperty(sheetClass.VarList[j].Name), addItem, valueList[j].Trim(), sheetClass.VarList[j].Type);
                }
                list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
            }

        }
        objClass.GetType().GetProperty(sheetClass.ParentVar.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 基础 List 赋值，List<基础数据类型>
    /// </summary>
    /// <param name="objClass"></param>
    /// <param name="varClass"></param>
    /// <param name="value"></param>
    private static void SetSplitBaseClass(object objClass, VarClass varClass, string value) {
        Type type = null;
        if (varClass.Type == "listStr") {
            type = typeof(string);
        }
        else if (varClass.Type == "listFloat") {
            type = typeof(float);
        }
        else if (varClass.Type == "listInt") {
            type = typeof(int);
        }
        else if (varClass.Type == "listBool") {
            type = typeof(bool);
        }
        object list = CreateList(type);
        string[] rowArray = value.Split(new string[] { varClass.SplitStr }, StringSplitOptions.None);
        for (int i = 0; i < rowArray.Length; i++) {
            object addItem = rowArray[i].Trim();
            try {
                list.GetType().InvokeMember("Add", BindingFlags.Default | BindingFlags.InvokeMethod, null, list, new object[] { addItem });
            }
            catch {
                Debug.Log(varClass.ListSheetName + "  里 " + varClass.Name + "  列表添加失败！具体数值是：" + addItem);
            }
        }
        objClass.GetType().GetProperty(varClass.Name).SetValue(objClass, list);
    }

    /// <summary>
    /// 根据列名获取变量名
    /// </summary>
    /// <param name="varlist"></param>
    /// <param name="col"></param>
    /// <returns></returns>
    private static string GetNameFormCol(List<VarClass> varlist, string col) {
        foreach (VarClass varClass in varlist) {
            if (varClass.Col == col)
                return varClass.Name;
        }
        return null;
    }

    /// <summary>
    /// XML 转 Excle
    /// </summary>
    /// <param name="name"></param>
    private static void XmlToExcel(string name) {
        string className = "";
        string xmlName = "";
        string excelName = "";
        // 读取，解析 Reg.xml - 表结构，列名
        Dictionary<string, SheetClass> allSheetClassDic = ReadReg(name, ref excelName, ref xmlName, ref className); // 储存所有变量的表
        // 反序列化到类
        object data = GetObjFormXml(className);
        List<SheetClass> outSheetList = new List<SheetClass>(); // 第一层
        foreach (SheetClass sheetClass in allSheetClassDic.Values) {
            if (sheetClass.Depth == 1) {
                outSheetList.Add(sheetClass);
            }
        }
        // 读取，解析 数据.xml，eg：MonsterData.xml
        Dictionary<string, SheetData> sheetDataDic = new Dictionary<string, SheetData>(); // 工作表里面的数据，key：表名，val：工作表
        for (int i = 0; i < outSheetList.Count; i++) {
            ReadData(data, outSheetList[i], allSheetClassDic, sheetDataDic, ""); // 递归读取 SheetClass 里面的数据
        }

        // 判断 Excle 文件是否已经打开
        string xlsxPath = ExcelPath + excelName;
        if (FileIsUsed(xlsxPath)) {
            Debug.LogError("文件被占用，无法修改");
            return;
        }

        // 写入 Excel
        try {
            FileInfo xlsxFile = new FileInfo(xlsxPath);
            if (xlsxFile.Exists) {
                xlsxFile.Delete();
                xlsxFile = new FileInfo(xlsxPath);
            }
            using (ExcelPackage package = new ExcelPackage(xlsxFile)) {
                foreach (string str in sheetDataDic.Keys) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(str);
                    SheetData sheetData = sheetDataDic[str];
                    // 列名
                    for (int i = 0; i < sheetData.AllName.Count; i++) {
                        ExcelRange range = worksheet.Cells[1, i + 1];
                        range.Value = sheetData.AllName[i];
                        range.AutoFitColumns();
                    }
                    // 数据
                    for (int i = 0; i < sheetData.AllData.Count; i++) {
                        RowData rowData = sheetData.AllData[i];
                        for (int j = 0; j < sheetData.AllData[i].RowDataDic.Count; j++) {
                            ExcelRange range = worksheet.Cells[i + 2, j + 1];
                            string vaule = rowData.RowDataDic[sheetData.AllName[j]];
                            range.Value = vaule;
                            range.AutoFitColumns();
                            if (vaule.Contains("\n") || vaule.Contains("\r\n")) {
                                range.Style.WrapText = true;
                            }
                        }
                    }
                    worksheet.Cells.AutoFitColumns();
                }
                package.Save();
            }
        }
        catch (Exception e) {
            Debug.LogError(e);
            return;
        }
        Debug.Log("生成 " + xlsxPath + " 成功！！！");
    }

    /// <summary>
    /// 解析 Reg.xml
    /// </summary>
    /// <param name="name"></param>
    /// <param name="excelName"></param>
    /// <param name="xmlName"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    private static Dictionary<string, SheetClass> ReadReg(string name, ref string excelName, ref string xmlName, ref string className) {
        string regPath = RegPath + name + ".xml";
        if (!File.Exists(regPath)) {
            Debug.LogError("此数据不存在配置变化xml：" + name);
        }
        // <data name ="MonsterData" from="G怪物.xlsx" to = "MonsterData.xml">
        XmlDocument xml = new XmlDocument();
        XmlReader reader = XmlReader.Create(regPath);
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.IgnoreComments = true; // 忽略 xml 里面的注释
        xml.Load(reader);
        XmlNode xn = xml.SelectSingleNode("data");
        XmlElement xe = (XmlElement)xn;
        className = xe.GetAttribute("name");
        xmlName = xe.GetAttribute("to");
        excelName = xe.GetAttribute("from");
        Dictionary<string, SheetClass> allSheetClassDic = new Dictionary<string, SheetClass>(); // 储存所有变量的表
        ReadXmlNode(xe, allSheetClassDic, 0);
        reader.Close();
        return allSheetClassDic;
    }

    /// <summary>
    /// 反序列化 xml 到 类
    /// </summary>
    /// <param name="name">类名</param>
    /// <returns></returns>
    private static object GetObjFormXml(string name) {
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            Type tempType = asm.GetType(name);
            if (tempType != null) {
                type = tempType;
                break;
            }
        }
        if (type != null) {
            string xmlPath = XmlPath + name + ".xml";
            return BinarySerializeOpt.XmlDeserialize(xmlPath, type);
        }

        return null;
    }

    /// <summary>
    /// 递归读取类里面的数据
    /// </summary>
    /// <param name="data">反序列化 XML 得到的类</param>
    /// <param name="sheetClass"></param>
    /// <param name="allSheetClassDic"></param>
    /// <param name="sheetDataDic"></param>
    /// <param name="mainKey">主键值</param>
    private static void ReadData(object data, SheetClass sheetClass, Dictionary<string, SheetClass> allSheetClassDic, Dictionary<string, SheetData> sheetDataDic, string mainKey) {
        List<VarClass> varList = sheetClass.VarList;
        VarClass varClass = sheetClass.ParentVar;
        // 根据 sheetClass 的父级 variable 名，反射，得到有多少个 sheetClass 下 有多少个变量，即 Excle 中有多少列
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));

        SheetData sheetData = new SheetData();
        // 存在外键，会多一列存外键值
        if (!string.IsNullOrEmpty(varClass.Foregin)) {
            sheetData.AllName.Add(varClass.Foregin);
            sheetData.AllType.Add(varClass.Type);
        }
        // 添加列名，Excle 中的列信息
        for (int i = 0; i < varList.Count; i++) {
            if (!string.IsNullOrEmpty(varList[i].Col)) {
                sheetData.AllName.Add(varList[i].Col);
                sheetData.AllType.Add(varList[i].Type);
            }
        }

        // 添加每一行数据
        string tempKey = mainKey;
        for (int i = 0; i < listCount; i++) {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i }); // List<MonsterBase> 里的 MonsterBase
            RowData rowData = new RowData(); // 这一列，每一行的数据
            if (!string.IsNullOrEmpty(varClass.Foregin) && !string.IsNullOrEmpty(tempKey)) { // 添加外键值
                rowData.RowDataDic.Add(varClass.Foregin, tempKey);
            }
            if (!string.IsNullOrEmpty(sheetClass.MainKey)) { // 算出这一层主键的值
                mainKey = GetMemberValue(item, sheetClass.MainKey).ToString();
            }
            for (int j = 0; j < varList.Count; j++) {
                if (varList[j].Type == "list" && string.IsNullOrEmpty(varList[j].SplitStr)) { // list<string>
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    ReadData(item, tempSheetClass, allSheetClassDic, sheetDataDic, mainKey);
                }
                else if (varList[j].Type == "list") { // list<class>
                    SheetClass tempSheetClass = allSheetClassDic[varList[j].ListSheetName];
                    string value = GetSplitStrList(item, varList[j], tempSheetClass);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                // List 基础数据类型，利用 分隔符 进行分割，eg：public List<string> AllString { get; set; } 
                else if (varList[j].Type == "listStr" || varList[j].Type == "listFloat" || varList[j].Type == "listInt" || varList[j].Type == "listBool") {
                    string value = GetSpliteBaseList(item, varList[j]);
                    rowData.RowDataDic.Add(varList[j].Col, value);
                }
                else {
                    object value = GetMemberValue(item, varList[j].Name); // MonsterBase 里指定变量对应的值
                    if (varList != null) {
                        rowData.RowDataDic.Add(varList[j].Col, value.ToString());
                    }
                    else {
                        Debug.LogError(varList[j].Name + " 反射出来为空！");
                    }
                }
            }
            string key = varClass.ListSheetName; // 需保证  SheetName 不能重复
            if (sheetDataDic.ContainsKey(key)) {
                sheetDataDic[key].AllData.Add(rowData);
            }
            else {
                sheetData.AllData.Add(rowData);
                sheetDataDic.Add(key, sheetData);
            }
        }
    }

    /// <summary>
    /// 适用于本身是一个类的列表，但是数据比较少 或 没办法确定父级结构
    /// 获取 List<class> 的所有数据字符串，eg：变量以;分割，类以\n分割
    /// </summary>
    /// <returns></returns>
    private static string GetSplitStrList(object data, VarClass varClass, SheetClass sheetClass) {
        string split = varClass.SplitStr;
        string classSplit = sheetClass.SplitStr;
        string str = "";
        if (string.IsNullOrEmpty(split) || string.IsNullOrEmpty(classSplit)) {
            Debug.LogError("类的列类分隔符 或 变量分隔符为空！！！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));
        for (int i = 0; i < listCount; i++) {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            for (int j = 0; j < sheetClass.VarList.Count; j++) {
                object value = GetMemberValue(item, sheetClass.VarList[j].Name);
                str += value.ToString();
                if (j != sheetClass.VarList.Count - 1) {
                    str += classSplit.Replace("\\n", "\n").Replace("\\r", "\r");
                }
            }
            if (i != listCount - 1) {
                str += split.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

    /// <summary>
    /// 获取基础数据类型 List 里面的所有值
    /// </summary>
    /// <returns></returns>
    private static string GetSpliteBaseList(object data, VarClass varClass) {
        string str = "";
        if (string.IsNullOrEmpty(varClass.SplitStr)) {
            Debug.LogError("基础数据类型 List 的分隔符为空！");
            return str;
        }
        object dataList = GetMemberValue(data, varClass.Name);
        int listCount = System.Convert.ToInt32(dataList.GetType().InvokeMember("get_Count", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { }));
        for (int i = 0; i < listCount; i++) {
            object item = dataList.GetType().InvokeMember("get_Item", BindingFlags.Default | BindingFlags.InvokeMethod, null, dataList, new object[] { i });
            str += item.ToString();
            if (i != listCount - 1) {
                str += varClass.SplitStr.Replace("\\n", "\n").Replace("\\r", "\r");
            }
        }
        return str;
    }

    /// <summary>
    /// 递归读取配置表，Reg.xml
    /// </summary>
    /// <param name="xmlElement">data 节点</param>
    /// <param name="allSheetClassDic">储存所有变量的表</param>
    /// <param name="depth">深度，第几层</param>
    private static void ReadXmlNode(XmlElement xmlElement, Dictionary<string, SheetClass> allSheetClassDic, int depth) {
        depth++;
        foreach (XmlNode node in xmlElement.ChildNodes) { // 遍历 data 下的子节点
            XmlElement xe = (XmlElement)node; // <variable  name="AllMonster" type="list">
            if (xe.GetAttribute("type") == "list") {
                XmlElement listEle = (XmlElement)node.FirstChild;
                VarClass parentVar = new VarClass() { // list 父级的 variable，<variable  name="AllMonster" type="list">
                    Name = xe.GetAttribute("name"),
                    Type = xe.GetAttribute("type"),
                    Col = xe.GetAttribute("col"),
                    DeafultValue = xe.GetAttribute("defaultValue"),
                    Foregin = xe.GetAttribute("foregin"),
                    SplitStr = xe.GetAttribute("split"),
                };
                if (parentVar.Type == "list") {
                    parentVar.ListName = ((XmlElement)xe.FirstChild).GetAttribute("name"); // <list name = "MonsterBase" sheetname="怪物配置" mainKey = "Id">
                    parentVar.ListSheetName = ((XmlElement)xe.FirstChild).GetAttribute("sheetname");
                }
                SheetClass sheetClass = new SheetClass() { // <list name = "MonsterBase" sheetname="怪物配置" mainKey = "Id">
                    Name = listEle.GetAttribute("name"),
                    SheetName = listEle.GetAttribute("sheetname"),
                    SplitStr = listEle.GetAttribute("split"),
                    MainKey = listEle.GetAttribute("mainKey"),
                    ParentVar = parentVar,
                    Depth = depth,
                };
                if (!string.IsNullOrEmpty(sheetClass.SheetName)) {
                    if (!allSheetClassDic.ContainsKey(sheetClass.SheetName)) {
                        // 遍历该节点(sheet/list)下面所有子节点
                        foreach (XmlNode insideNode in listEle.ChildNodes) { // <variable  name="Rare" col = "怪物稀有度" type="int" defaultValue = "0"/>...
                            XmlElement insideXe = (XmlElement)insideNode;
                            VarClass varClass = new VarClass() { // list 下的 variable，<variable  name="Rare" col = "怪物稀有度" type="int" defaultValue = "0"/>
                                Name = insideXe.GetAttribute("name"),
                                Type = insideXe.GetAttribute("type"),
                                Col = insideXe.GetAttribute("col"),
                                DeafultValue = insideXe.GetAttribute("defaultValue"),
                                Foregin = insideXe.GetAttribute("foregin"),
                                SplitStr = insideXe.GetAttribute("split"),
                            };
                            if (varClass.Type == "list") {
                                varClass.ListName = ((XmlElement)insideXe.FirstChild).GetAttribute("name");
                                varClass.ListSheetName = ((XmlElement)insideXe.FirstChild).GetAttribute("sheetname");
                            }
                            sheetClass.VarList.Add(varClass);
                        }
                        allSheetClassDic.Add(sheetClass.SheetName, sheetClass);
                    }
                }
                ReadXmlNode(listEle, allSheetClassDic, depth); // 递归
            }
        }
    }

    /// <summary>
    /// 判断文件是否被占用
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    private static bool FileIsUsed(string path) {
        bool result = false;
        if (!File.Exists(path)) {
            result = false;
        }
        else {
            // 尝试用文件流打开文件，如果出异常说明文件已经被打开了
            FileStream fileStream = null;
            try {
                fileStream = File.Open(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                result = false;
            }
            catch (Exception e) {
                Debug.LogError(e);
                result = true;
            }
            finally {
                if (fileStream != null) {
                    fileStream.Close();
                }
            }
        }
        return result;
    }

    /// <summary>
    /// 反射，new 一個 list
    /// </summary>
    /// <param name="type">List 里 Item 的类型</param>
    /// <returns></returns>
    private static object CreateList(Type type) {
        Type listType = typeof(List<>); // 获取 List 泛型的类型
        Type specType = listType.MakeGenericType(new System.Type[] { type }); // 确定 list<> 里面 T 的类型
        return Activator.CreateInstance(specType, new object[] { }); // new 出来这个 list
    }

    /// <summary>
    /// 反射，变量赋值，主要是封装了类型转换
    /// </summary>
    /// <param name="info"></param>
    /// <param name="var"></param>
    /// <param name="value"></param>
    /// <param name="type"></param>
    private static void SetValue(PropertyInfo info, object var, string value, string type) {
        object val = (object)value;
        if (type == "int") {
            val = System.Convert.ToInt32(val);
        }
        else if (type == "bool") {
            val = System.Convert.ToBoolean(val);
        }
        else if (type == "float") {
            val = System.Convert.ToSingle(val);
        }
        else if (type == "enum") {
            val = TypeDescriptor.GetConverter(info.PropertyType).ConvertFromInvariantString(val.ToString());
        }
        info.SetValue(var, val);
    }

    /// <summary>
    /// 反射类里面的变量的具体数值
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="memeberName"></param>
    /// <param name="bindingFlags"></param>
    /// <returns></returns>
    private static object GetMemberValue(object obj, string memeberName, BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance) {
        Type type = obj.GetType();
        MemberInfo[] members = type.GetMember(memeberName, bindingFlags); // 从 bindingFlags 里面查找叫 memeberName 的值
        switch (members[0].MemberType) {
            case MemberTypes.Field:
                return type.GetField(memeberName, bindingFlags).GetValue(obj);
            case MemberTypes.Property: // 属性
                return type.GetProperty(memeberName, bindingFlags).GetValue(obj);
            default:
                return null;
        }
    }

    /// <summary>
    /// 反射，创建类的实例
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static object CreateClass(string name) {
        object obj = null;
        Type type = null;
        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            Type tempType = asm.GetType(name);
            if (tempType != null) {
                type = tempType;
                break;
            }
        }
        if (type != null) {
            obj = Activator.CreateInstance(type);
        }
        return obj;
    }

    /// <summary>
    /// xml 转 二进制
    /// </summary>
    /// <param name="name"></param>
    private static void XmlToBinary(string name) {
        if (string.IsNullOrEmpty(name))
            return;
        try {
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) {
                Type tempType = asm.GetType(name);
                if (tempType != null) {
                    type = tempType;
                    break;
                }
            }
            if (type != null) {
                string xmlPath = XmlPath + name + ".xml";
                string binaryPath = BinaryPath + name + ".bytes";
                object obj = BinarySerializeOpt.XmlDeserialize(xmlPath, type); // XML 转类
                BinarySerializeOpt.BinarySerilize(binaryPath, obj); // 类转二进制
                Debug.Log(name + "xml 转二进制成功，二进制路径为:" + binaryPath);
            }
        }
        catch {
            Debug.LogError(name + "xml 转二进制失败！");
        }
    }

    /// <summary>
    /// 实际的类转XML，反射
    /// </summary>
    /// <param name="name">根据名字找到类</param>
    private static void ClassToXml(string name) {
        if (string.IsNullOrEmpty(name))
            return;
        try {
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies()) { // 遍历当前的主程序集
                Type tempType = asm.GetType(name); // 根据名字获取类型
                if (tempType != null) {
                    type = tempType;
                    break;
                }
            }
            if (type != null) {
                var temp = Activator.CreateInstance(type); // 利用反射 new 这个类
                if (temp is ExcelBase) {
                    (temp as ExcelBase).Construction(); // 生成基本数据
                }
                string xmlPath = XmlPath + name + ".xml";
                BinarySerializeOpt.Xmlserialize(xmlPath, temp); // 类转 XML
                Debug.Log(name + "类转 xml 成功，xml 路径为：" + xmlPath);
            }
        }
        catch {
            Debug.LogError(name + "类转 xml 失败！");
        }
    }
}

/// <summary>
/// 工作表类，variable 下的 list
/// </summary>
public class SheetClass {
    /// <summary>
    /// 所属父级 Var 变量
    /// </summary>
    public VarClass ParentVar { get; set; }
    /// <summary>
    /// 深度，第几层
    /// </summary>
    public int Depth { get; set; }
    /// <summary>
    /// 类名
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 类对应的 sheet 名 
    /// </summary>
    public string SheetName { get; set; }
    /// <summary>
    /// 主键
    /// </summary>
    public string MainKey { get; set; }
    /// <summary>
    /// 分隔符
    /// </summary>
    public string SplitStr { get; set; }
    /// <summary>
    /// 所包含的变量
    /// </summary>
    public List<VarClass> VarList = new List<VarClass>();
}

/// <summary>
/// 变量类
/// </summary>
public class VarClass {
    /// <summary>
    /// 原类里面变量的名称
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// 变量类型
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// 变量对应的 Excel 里的列
    /// </summary>
    public string Col { get; set; }
    /// <summary>
    /// 变量的默认值
    /// </summary>
    public string DeafultValue { get; set; }
    /// <summary>
    /// 变量是 list 的话，外联部分列，外键
    /// </summary>
    public string Foregin { get; set; }
    /// <summary>
    /// 分隔符
    /// </summary>
    public string SplitStr { get; set; }
    /// <summary>
    /// 如果自己是 List，对应的 list 类名
    /// </summary>
    public string ListName { get; set; }
    /// <summary>
    /// 如果自己是 list，对应的 sheet 名
    /// </summary>
    public string ListSheetName { get; set; }
}

/// <summary>
/// 工作表里具体的值
/// </summary>
public class SheetData {
    /// <summary>
    /// 所有的列名
    /// </summary>
    public List<string> AllName = new List<string>();
    /// <summary>
    /// 所有的类型
    /// </summary>
    public List<string> AllType = new List<string>();
    /// <summary>
    /// 所有的数据，所有行的数据
    /// </summary>
    public List<RowData> AllData = new List<RowData>();
}

/// <summary>
/// 每一行的数据
/// </summary>
public class RowData {
    public string ParnetVlue = "";
    /// <summary>
    /// key：列名，val：对应的数据
    /// </summary>
    public Dictionary<string, string> RowDataDic = new Dictionary<string, string>();
}
