/****************************************************
	文件：ExcelBase.cs
	作者：TravelerTD
	日期：2019/08/19 16:08:20   	
	功能：配置表基类
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExcelBase {
#if UNITY_EDITOR
    /// <summary>
    /// 生成基本数据，用于转 XML 的时候，可以生成一个基本的结构，方便于复制，不用手写 XML 结构
    /// </summary>
    public virtual void Construction() { }
#endif

    public virtual void Init() { }
}
