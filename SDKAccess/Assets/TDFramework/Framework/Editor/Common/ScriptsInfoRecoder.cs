/****************************************************
	文件：ScriptsInfoRecoder.cs
	作者：TravelerTD
	日期：2019/08/21 09:53:12   	
	功能：记录脚本信息，解析代码模板的参数
*****************************************************/

using System;
using System.Globalization;
using System.IO;

public class ScriptsInfoRecoder : UnityEditor.AssetModificationProcessor {

    private static void OnWillCreateAsset(string path) {
        path = path.Replace(".meta", "");
        if (path.EndsWith(".cs")) {
            string str = File.ReadAllText(path);
            str = str.Replace("#CreateAuthor#", Environment.UserName).Replace("#CreateTime#", string.Concat(TDFramework.DateUtil.DateTimeNow()));
            File.WriteAllText(path, str);
        }
    }
}