/****************************************************
	文件：BuffData.cs
	作者：TravelerTD
	日期：2019/08/21 10:17:16   	
	功能：Buff 配置表
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class BuffData : ExcelBase {

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下，初始类转 xml
    /// </summary>
    public override void Construction() {
        AllBuffList = new List<BuffBase>();
        for (int i = 0; i < 10; i++) {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全 BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5F, 10);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            buff.AllString.Add("ceshiq" + i);
            buff.AllBuffList = new List<BuffTest>();
            // 插入 List<BuffTest> AllBuffList
            int count = Random.Range(0, 4);
            for (int j = 0; j < count; j++) {
                BuffTest test = new BuffTest();
                test.Id = j + Random.Range(0, 5);
                test.Name = "name" + j;
                buff.AllBuffList.Add(test);
            }
            AllBuffList.Add(buff);
        }
        MonsterBuffList = new List<BuffBase>();
        for (int i = 0; i < 5; i++) {
            BuffBase buff = new BuffBase();
            buff.Id = i + 1;
            buff.Name = "全BUFF" + i;
            buff.OutLook = "Assets/GameData/..." + i;
            buff.Time = Random.Range(0.5F, 10);
            buff.BuffType = (BuffEnum)Random.Range(0, 4);
            buff.AllString = new List<string>();
            buff.AllString.Add("ceshi" + i);
            buff.AllBuffList = new List<BuffTest>();
            int count = Random.Range(0, 4);
            for (int j = 0; j < count; j++) {
                BuffTest test = new BuffTest();
                test.Id = j + Random.Range(0, 5);
                test.Name = "name" + j;
                buff.AllBuffList.Add(test);
            }
            MonsterBuffList.Add(buff);
        }
    }
#endif
    /// <summary>
    /// 数据初始化
    /// </summary>
    public override void Init() {
        AllBuffDic.Clear();
        for (int i = 0; i < AllBuffList.Count; i++) {
            AllBuffDic.Add(AllBuffList[i].Id, AllBuffList[i]);
        }
    }

    /// <summary>
    /// 根据 ID 查找 buff
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public BuffBase FinBuffById(int id) {
        return AllBuffDic[id];
    }

    /// <summary>
    ///  所有的 Buff 数据，缓存
    /// </summary>
    [XmlIgnore] // 不需要序列化
    public Dictionary<int, BuffBase> AllBuffDic = new Dictionary<int, BuffBase>();

    /// <summary>
    /// 所有 Buff 数据
    /// </summary>
    [XmlElement("AllBuffList")]
    public List<BuffBase> AllBuffList { get; set; }

    /// <summary>
    /// 所有怪物的 Buff 数据
    /// </summary>
    [XmlElement("MonsterBuffList")]
    public List<BuffBase> MonsterBuffList { get; set; }
}

/// <summary>
/// Buff 类型
/// </summary>
public enum BuffEnum {
    None = 0,
    Ranshao = 1,
    Bingdong = 2,
    Du = 3,
}

/// <summary>
/// Buff 实体
/// </summary>
[System.Serializable]
public class BuffBase {
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }

    [XmlAttribute("OutLook")]
    public string OutLook { get; set; }

    [XmlAttribute("Time")]
    public float Time { get; set; }

    [XmlAttribute("BuffType")]
    public BuffEnum BuffType { get; set; }

    /// <summary>
    /// 用分隔符进行分割
    /// </summary>
    [XmlElement("AllString")]
    public List<string> AllString { get; set; }

    /// <summary>
    /// 新插入工作表
    /// </summary>
    [XmlElement("AllBuffList")]
    public List<BuffTest> AllBuffList { get; set; }
}

[System.Serializable]
public class BuffTest {
    [XmlAttribute("Id")]
    public int Id { get; set; }

    [XmlAttribute("Name")]
    public string Name { get; set; }
}
