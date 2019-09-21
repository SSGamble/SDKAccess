/****************************************************
    文件：ClassObjectPool.cs
	作者：TravelerTD
    日期：2019/8/11 15:1:23
	功能：类对象池
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassObjectPool<T> where T : class, new() {
    protected Stack<T> m_Pool = new Stack<T>(); // 池
    protected int m_MaxCount = 0; // 最大对象个数，<=0 表示不限个数
    protected int m_NoRecycleCount = 0; // 没有回收的个数

    public ClassObjectPool(int maxcount) {
        m_MaxCount = maxcount;
        for (int i = 0; i < maxcount; i++) {
            m_Pool.Push(new T());
        }
    }

    /// <summary>
    /// 从池里面取对象
    /// </summary>
    /// <param name="createIfPoolEmpty">如果池里没有对象了，需不需要 new 一个对象</param>
    /// <returns></returns>
    public T Spawn(bool createIfPoolEmpty) {
        if (m_Pool.Count > 0) {
            T rtn = m_Pool.Pop();
            if (rtn == null) {
                if (createIfPoolEmpty) {
                    rtn = new T();
                }
            }
            m_NoRecycleCount++;
            return rtn;
        }
        else {
            if (createIfPoolEmpty) {
                T rtn = new T();
                m_NoRecycleCount++;
                return rtn;
            }
        }
        return null;
    }

    /// <summary>
    /// 回收类对象
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Recycle(T obj) {
        if (obj == null) {
            return false;
        }
        m_NoRecycleCount--;
        if (m_Pool.Count >= m_MaxCount && m_MaxCount > 0) { // 不属于池里的（直接 new 出来的），直接置为 null，之后会被 GC
            obj = null;
            return false;
        }
        m_Pool.Push(obj);
        return true;
    }
}
