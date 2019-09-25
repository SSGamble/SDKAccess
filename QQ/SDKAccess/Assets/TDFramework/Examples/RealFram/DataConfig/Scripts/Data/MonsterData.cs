/****************************************************
	文件：MonsterData.cs
	作者：TravelerTD
	日期：2019/08/19 16:24:54   	
	功能：怪物配置表
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class MonsterData : ExcelBase {

#if UNITY_EDITOR
    /// <summary>
    /// 编辑器下，初始类转 xml
    /// </summary>
    public override void Construction() {
        // 随便给了一些数据，用于转 XML 的时候，可以生成一个基本的结构，方便于复制，不用手写 XML 结构
        AllMonster = new List<MonsterBase>();
        for (int i = 0; i < 5; i++) {
            MonsterBase monster = new MonsterBase();
            monster.Id = i + 1;
            monster.Name = i + "sq";
            monster.OutLook = "Assets/GameData/Prefabs/Attack.prefab";
            monster.Rare = 2;
            monster.Height = 2 + i;
            AllMonster.Add(monster);
        }
    }
#endif

    /// <summary>
    /// 数据初始化
    /// </summary>
    public override void Init() {
        m_AllMonsterDic.Clear();
        foreach (MonsterBase monster in AllMonster) {
            if (m_AllMonsterDic.ContainsKey(monster.Id)) {
                Debug.LogError(monster.Name + " 有重复 ID");
            }
            else {
                m_AllMonsterDic.Add(monster.Id, monster);
            }
        }
    }

    /// <summary>
    /// 根据 ID 查找 Monster 数据
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public MonsterBase FinMonsterById(int id) {
        return m_AllMonsterDic[id];
    }

    /// <summary>
    ///  所有的怪物数据，缓存
    /// </summary>
    [XmlIgnore] // 不需要序列化
    public Dictionary<int, MonsterBase> m_AllMonsterDic = new Dictionary<int, MonsterBase>();

    /// <summary>
    /// 所有的怪物数据
    /// </summary>
    [XmlElement("AllMonster")]
    public List<MonsterBase> AllMonster { get; set; }
}

/// <summary>
/// 怪物实体
/// </summary>
[System.Serializable]
public class MonsterBase {
    // ID
    [XmlAttribute("Id")]
    public int Id { get; set; }
    // Name
    [XmlAttribute("Name")]
    public string Name { get; set; }
    // 预制体路径
    [XmlAttribute("OutLook")]
    public string OutLook { get; set; }
    // 怪物等级
    [XmlAttribute("Level")]
    public int Level { get; set; }
    // 怪物稀有度
    [XmlAttribute("Rare")]
    public int Rare { get; set; }
    // 怪物高度
    [XmlAttribute("Height")]
    public float Height { get; set; }
}
