/****************************************************
    文件：DictionaryExtension.cs
	作者：TravelerTD
    日期：2019/08/29 13:56:40
	功能：字典的扩展方法
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DictionaryExtension {

    /// <summary>
    /// 尝试根据 key 得到 value，得到则返回 value，否则返回 null
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    /// <param name="dict">这个字典表示我们要获取值的字典</param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Tvalue TryGet<Tkey, Tvalue>(this Dictionary<Tkey, Tvalue> dict, Tkey key) {
        Tvalue value;
        dict.TryGetValue(key, out value);
        return value;
    }
}