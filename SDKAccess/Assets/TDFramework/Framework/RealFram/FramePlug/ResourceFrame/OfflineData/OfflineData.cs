/****************************************************
	文件：OfflineData.cs
	作者：TravelerTD
	日期：2019/08/18 9:04   	
	功能：离线数据的基类
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineData : MonoBehaviour {
    // 可能用到的常用信息
    public Rigidbody m_Rigidbody;
    public Collider m_Collider;
    /// <summary>
    /// 一个 Prefab 下的所有节点
    /// </summary>
    public Transform[] m_AllPoint;
    /// <summary>
    /// 每一个节点下的子节点个数，类似于保存一个树形结构
    /// </summary>
    public int[] m_AllPointChildCount;
    /// <summary>
    /// 每一个节点是否激活
    /// </summary>
    public bool[] m_AllPointActive;
    /// <summary>
    /// 每一个节点的位置信息
    /// </summary>
    public Vector3[] m_Pos;
    /// <summary>
    /// 每一个节点的缩放信息
    /// </summary>
    public Vector3[] m_Scale;
    /// <summary>
    /// 每一个节点的旋转信息
    /// </summary>
    public Quaternion[] m_Rot;

    /// <summary>
    /// 还原属性
    /// </summary>
    public virtual void ResetProp() {
        int allPointCount = m_AllPoint.Length;
        for (int i = 0; i < allPointCount; i++) {
            Transform tempTrs = m_AllPoint[i];
            if (tempTrs != null) {
                tempTrs.localPosition = m_Pos[i];
                tempTrs.localRotation = m_Rot[i];
                tempTrs.localScale = m_Scale[i];
                if (m_AllPointActive[i]) {
                    if (!tempTrs.gameObject.activeSelf) {
                        tempTrs.gameObject.SetActive(true);
                    }
                }
                else {
                    if (tempTrs.gameObject.activeSelf) {
                        tempTrs.gameObject.SetActive(false);
                    }
                }
                if (tempTrs.childCount > m_AllPointChildCount[i]) {
                    int childCount = tempTrs.childCount;
                    for (int j = m_AllPointChildCount[i]; j < childCount; j++) {
                        GameObject tempObj = tempTrs.GetChild(j).gameObject;
                        if (!ObjectManager.Instance.IsObjectManagerCreate(tempObj)) {
                            GameObject.Destroy(tempObj);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// 编辑器下保存的初始数据
    /// </summary>
    public virtual void BindData() {
        m_Collider = gameObject.GetComponentInChildren<Collider>(true);
        m_Rigidbody = gameObject.GetComponentInChildren<Rigidbody>(true);
        m_AllPoint = gameObject.GetComponentsInChildren<Transform>(true);
        int allPointCount = m_AllPoint.Length;
        m_AllPointChildCount = new int[allPointCount];
        m_AllPointActive = new bool[allPointCount];
        m_Pos = new Vector3[allPointCount];
        m_Rot = new Quaternion[allPointCount];
        m_Scale = new Vector3[allPointCount];
        for (int i = 0; i < allPointCount; i++) {
            Transform temp = m_AllPoint[i] as Transform;
            m_AllPointChildCount[i] = temp.childCount;
            m_AllPointActive[i] = temp.gameObject.activeSelf;
            m_Pos[i] = temp.localPosition;
            m_Rot[i] = temp.localRotation;
            m_Scale[i] = temp.localScale;
        }
    }
}
