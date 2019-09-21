/****************************************************
	文件：EffectOfflineData.cs
	作者：TravelerTD
	日期：2019/08/18 9:05   	
	功能：特效相关的离线数据
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOfflineData : OfflineData {
    /// <summary>
    /// 粒子系统
    /// </summary>
    public ParticleSystem[] m_Particle;
    /// <summary>
    /// 拖尾
    /// </summary>
    public TrailRenderer[] m_TrailRe;

    /// <summary>
    /// 还原属性
    /// </summary>
    public override void ResetProp() {
        base.ResetProp();
        foreach (ParticleSystem particle in m_Particle) {
            particle.Clear(true);
            particle.Play();
        }
        foreach (TrailRenderer trail in m_TrailRe) {
            trail.Clear();
        }
    }

    /// <summary>
    /// 编辑器下保存的初始数据
    /// </summary>
    public override void BindData() {
        base.BindData();
        m_Particle = gameObject.GetComponentsInChildren<ParticleSystem>(true);
        m_TrailRe = gameObject.GetComponentsInChildren<TrailRenderer>(true);
    }
}
