/****************************************************
    文件：BundleEditor.cs
	作者：TravelerTD
    日期：2019/8/9 15:59:44
	功能：生成 AB 包
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

public class BundleEditor {
    /// <summary>
    /// 打包配置表路径
    /// </summary>
    private static string ABCONFIGPATH = RealFrameConfig.abConfigPath;
    /// <summary>
    /// 二进制配置表路径
    /// </summary>
    private static string ABBYTEPATH = RealFrameConfig.m_ABBytePath;
    /// <summary>
    /// 打包路径，按当前平台
    /// </summary>
    private static string m_BunleTargetPath = Application.dataPath + "/../AssetBundle/" + EditorUserBuildSettings.activeBuildTarget.ToString();
    /// <summary>
    /// 所有文件夹 ab 包，用于对指定文件夹进行打包，key：ab包名，val：路径
    /// </summary>
    private static Dictionary<string, string> m_AllFileDir = new Dictionary<string, string>();
    /// <summary>
    /// 过滤后的 ab 包，保存的是剔除冗余AB包后的资源路径
    /// </summary>
    private static List<string> m_AllFileAB = new List<string>();
    /// <summary>
    /// 单个 Prefab 的 ab 包，用于对指定文件夹下所有单个文件进行打包，key：ab包名，val：路径
    /// </summary>
    private static Dictionary<string, List<string>> m_AllPrefabDir = new Dictionary<string, List<string>>();
    /// <summary>
    /// 存储所有有效路径，相比较 allFileAB 而言过滤掉了一些不需要动态加载的资源
    /// </summary>
    private static List<string> m_ConfigFil = new List<string>();

    //[MenuItem("RealFram/打包")]
    public static void Build() {
        DataConfigEditor.AllXmlToBinary(); // 将所有的配置表转二进制
        m_ConfigFil.Clear();
        m_AllFileAB.Clear();
        m_AllFileDir.Clear();
        m_AllPrefabDir.Clear();
        ABConfig abConfig = AssetDatabase.LoadAssetAtPath<ABConfig>(ABCONFIGPATH);  // 加载配置表
        if (abConfig == null) {
            Debug.LogError("ABConfig == null，请确认 AB 配置表的路径");
        }
        // 先处理文件夹
        foreach (ABConfig.FileDirABName fileDir in abConfig.m_AllFileDirAB) { // 遍历配置表中的文件夹
            if (m_AllFileDir.ContainsKey(fileDir.ABName)) {
                Debug.LogError("AB 包配置名字重复，请检查！");
            }
            else {
                m_AllFileDir.Add(fileDir.ABName, fileDir.Path);
                m_AllFileAB.Add(fileDir.Path);
                m_ConfigFil.Add(fileDir.Path);
            }
        }
        // 再处理单个文件，Prefab
        string[] allStr = AssetDatabase.FindAssets("t:Prefab", abConfig.m_AllPrefabPath.ToArray()); // 找到所有的 Prefab，返回的是 guid 数组
        for (int i = 0; i < allStr.Length; i++) {  // 遍历 Prefab
            string path = AssetDatabase.GUIDToAssetPath(allStr[i]);  // 根据 guid 获取资源路径
            EditorUtility.DisplayProgressBar("查找 Prefab", "Prefab:" + path, i * 1.0f / allStr.Length);  // 进度条
            m_ConfigFil.Add(path);
            if (!ContainAllFileAB(path)) { // 判断过滤
                // 一个 Prefab 其实包含了很多的东西，如贴图，shader 等，如果 shader 已经打包了，那么这里就不需要打包了
                GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                string[] allDepend = AssetDatabase.GetDependencies(path); // 获取 Prefab 的依赖项，包含 Prefab 自己，还包含脚本
                List<string> allDependPath = new List<string>(); // 一个 Prefab 的所有依赖项路径
                for (int j = 0; j < allDepend.Length; j++) { // 遍历依赖项路径
                    if (!ContainAllFileAB(allDepend[j]) && !allDepend[j].EndsWith(".cs")) {  // 过滤依赖项，剔除脚本
                        m_AllFileAB.Add(allDepend[j]);
                        allDependPath.Add(allDepend[j]);
                    }
                }
                if (m_AllPrefabDir.ContainsKey(obj.name)) {
                    Debug.LogError("存在相同名字的 Prefab：" + obj.name);
                }
                else {
                    m_AllPrefabDir.Add(obj.name, allDependPath);  // 一个 Prefab 过滤后的依赖
                }
            }
        }
        // 设置文件夹打包的 ab名
        foreach (string name in m_AllFileDir.Keys) {
            SetABName(name, m_AllFileDir[name]);
        }
        // 设置单个文件 Prefab 的 ab名
        foreach (string name in m_AllPrefabDir.Keys) {
            SetABName(name, m_AllPrefabDir[name]);
        }
        // 打包
        BunildAssetBundle();
        // 清除所有的 AB包 名字
        string[] oldABNames = AssetDatabase.GetAllAssetBundleNames();  // 设置好的所有 AB包 的名字，eg：attack，shader，sound
        for (int i = 0; i < oldABNames.Length; i++) {
            AssetDatabase.RemoveAssetBundleName(oldABNames[i], true);
            EditorUtility.DisplayProgressBar("清除AB包名", "名字：" + oldABNames[i], i * 1.0f / oldABNames.Length);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.ClearProgressBar();
    }

    /// <summary>
    /// 构建 AB 包
    /// </summary>
    static void BunildAssetBundle() {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames(); // 设置好的所有 AB包 的名字，eg：attack，shader，sound
        Dictionary<string, string> resPathDic = new Dictionary<string, string>();  // 过滤后的资源信息，key: 全路径，val: 包名
        for (int i = 0; i < allBundlesName.Length; i++) {
            string[] allBundlePath = AssetDatabase.GetAssetPathsFromAssetBundle(allBundlesName[i]); // 获取指定包名内所包含资源的全路径
            for (int j = 0; j < allBundlePath.Length; j++) {
                if (allBundlePath[j].EndsWith(".cs")) // 过滤脚本文件
                    continue;
                Debug.Log("此 AB 包：" + allBundlesName[i] + "下面包含的资源文件路径：" + allBundlePath[j]);
                resPathDic.Add(allBundlePath[j], allBundlesName[i]);
            }
        }

        if (!Directory.Exists(m_BunleTargetPath)) {
            Directory.CreateDirectory(m_BunleTargetPath);
        }
        // 删除没用的 AB 包
        DeleteAB();
        // 生成自己的配置表
        WriteData(resPathDic);
        // 如果打 AB 包路径不存在，自行创建文件夹
        if (!Directory.Exists(m_BunleTargetPath)) {
            Directory.CreateDirectory(m_BunleTargetPath);
        }
        // 构建 AssetBundle 包
        AssetBundleManifest abManifest = BuildPipeline.BuildAssetBundles(m_BunleTargetPath, BuildAssetBundleOptions.ChunkBasedCompression, EditorUserBuildSettings.activeBuildTarget);
        if (abManifest == null) {
            Debug.LogError("AssetBundle 打包失败，abManifest == null");
        }
        else {
            Debug.Log("AssetBundle 打包完毕");
        }
    }

    /// <summary>
    /// 生成自己的配置表
    /// </summary>
    /// <param name="resPathDic">过滤后的资源信息，key: 全路径，val: 包名</param>
    private static void WriteData(Dictionary<string, string> resPathDic) {
        // 设置配置表信息
        AssetBundleConfig config = new AssetBundleConfig();
        config.ABList = new List<ABBase>();
        foreach (string path in resPathDic.Keys) {
            if (!ValidPath(path)) // 是否是有效路径
                continue;
            ABBase abBase = new ABBase();
            abBase.Path = path;
            abBase.Crc = Crc32.GetCrc32(path);
            abBase.ABName = resPathDic[path];
            abBase.AssetName = path.Remove(0, path.LastIndexOf("/") + 1);
            abBase.ABDependce = new List<string>();
            string[] resDependce = AssetDatabase.GetDependencies(path);  // 获取依赖项
            for (int i = 0; i < resDependce.Length; i++) { // 过滤，看依赖项在哪一个 ab 里面
                string tempPath = resDependce[i];
                if (tempPath == path || path.EndsWith(".cs")) { // 过滤自己和脚本
                    continue;
                }
                string abName = "";
                if (resPathDic.TryGetValue(tempPath, out abName)) { // 存在在其他 ab 里
                    if (abName == resPathDic[path]) { // 忽略自己
                        continue;
                    }
                    if (!abBase.ABDependce.Contains(abName)) { // 可能一个 prefab 依赖了 一个ab包里面的多个文件，只添加一次依赖就行了
                        abBase.ABDependce.Add(abName);
                    }
                }
            }
            config.ABList.Add(abBase);
        }
        // 写入 xml，方便自己可以看见
        string xmlPath = Application.dataPath + "/AssetbundleConfig.xml";
        if (File.Exists(xmlPath)) File.Delete(xmlPath);
        FileStream fileStream = new FileStream(xmlPath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite); // 文件流
        StreamWriter sw = new StreamWriter(fileStream, System.Text.Encoding.UTF8); // 写入流
        XmlSerializer xs = new XmlSerializer(config.GetType()); // 需要序列化的类型
        xs.Serialize(sw, config); // 将 config 序列化到 sw 去
        sw.Close();
        fileStream.Close();
        // 写入 二进制
        foreach (ABBase abBase in config.ABList) { // 清除掉 path 属性，因为这只是为了在 xml 方便自己查看，是不需要存在在二进制文件里的，CRC 才是唯一标识
            abBase.Path = "";
        }
        FileStream fs = new FileStream(ABBYTEPATH, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite); // 文件流
        fs.Seek(0, SeekOrigin.Begin); // 清空文件流
        fs.SetLength(0);
        BinaryFormatter bf = new BinaryFormatter(); // 二进制流
        bf.Serialize(fs, config);
        fs.Close();
        // 刷新，设置 AB包名
        AssetDatabase.Refresh();
        SetABName("assetbundleconfig", ABBYTEPATH);
    }

    /// <summary>
    /// 设置 ab 包的名字
    /// </summary>
    /// <param name="name"></param>
    /// <param name="path"></param>
    static void SetABName(string name, string path) {
        AssetImporter assetImporter = AssetImporter.GetAtPath(path);
        if (assetImporter == null) {
            Debug.LogError("不存在此路径文件：" + path + "，请检查 ABConfig");
        }
        else {
            assetImporter.assetBundleName = name;
        }
    }

    /// <summary>
    /// 设置 ab 包的名字
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pathList"></param>
    static void SetABName(string name, List<string> pathList) {
        for (int i = 0; i < pathList.Count; i++) {
            SetABName(name, pathList[i]);
        }
    }

    /// <summary>
    /// 删除没用的 AB 包
    /// </summary>
    static void DeleteAB() {
        string[] allBundlesName = AssetDatabase.GetAllAssetBundleNames();  // 设置好的所有 AB包 的名字，eg：attack，shader，sound
        DirectoryInfo direction = new DirectoryInfo(m_BunleTargetPath);
        FileInfo[] files = direction.GetFiles("*", SearchOption.AllDirectories); // 获取文件夹下所有的文件
        for (int i = 0; i < files.Length; i++) {
            if (ConatinABName(files[i].Name, allBundlesName)
                || files[i].Name.EndsWith(".meta")
                || files[i].Name.EndsWith(".manifest")
                || files[i].Name.EndsWith("assetbundleconfig")) { // 如果文件包含在将要打包的列表里，就不用删了，meta 文件也不用管
                continue;
            }
            else {
                Debug.Log("此 AB 包已经被删或者改名了：" + files[i].Name);
                if (File.Exists(files[i].FullName)) {
                    File.Delete(files[i].FullName);
                }
                if (File.Exists(files[i].FullName + ".manifest")) {
                    File.Delete(files[i].FullName + ".manifest");
                }
            }
        }
    }

    /// <summary>
    /// 指定 ab 包是否已存在
    /// </summary>
    /// <param name="name"></param>
    /// <param name="strs"></param>
    /// <returns></returns>
    static bool ConatinABName(string name, string[] strs) {
        // 遍历文件夹里的文件名与设置的所有 AB包 进行检查判断
        for (int i = 0; i < strs.Length; i++) {
            if (name == strs[i])
                return true;
        }
        return false;
    }

    /// <summary>
    /// 指定路径是否已经在过滤的AB包里面，用于剔除冗余 AB 包
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    static bool ContainAllFileAB(string path) {
        for (int i = 0; i < m_AllFileAB.Count; i++) {
            if (path == m_AllFileAB[i] || (path.Contains(m_AllFileAB[i]) && (path.Replace(m_AllFileAB[i], "")[0] == '/')))
                return true;
        }

        return false;
    }

    /// <summary>
    /// 是否是有效路径 - configFile
    /// </summary>
    /// <param name="path"></param>
    static bool ValidPath(string path) {
        for (int i = 0; i < m_ConfigFil.Count; i++) {
            if (path.Contains(m_ConfigFil[i])) {
                return true;
            }
        }
        return false;
    }
}
