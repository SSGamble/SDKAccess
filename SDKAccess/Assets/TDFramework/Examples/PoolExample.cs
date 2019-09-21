/****************************************************
    文件：PoolExample.cs
	作者：TravelerTD
    日期：2019/8/7 16:59:9
	功能：对象池示例
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class PoolExample {

        private class Fish { }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/14.PoolManager", false, 13)]
        private static void MenuClicked() {
            // new 了 100 条鱼到对象池
            var fishPool = new SimpleObjectPool<Fish>(() => new Fish(), null, 100);
            Debug.LogFormat("fishPool.CurCount:{0}", fishPool.CurCount);
            // 抓出了一条
            var fishOne = fishPool.Allocate();
            Debug.LogFormat("fishPool.CurCount:{0}", fishPool.CurCount);
            // 又放回去一条
            fishPool.Recycle(fishOne);
            Debug.LogFormat("fishPool.CurCount:{0}", fishPool.CurCount);
            // 抓了 10 条
            for (var i = 0; i < 10; i++) {
                fishPool.Allocate();
            }
            Debug.LogFormat("fishPool.CurCount:{0}", fishPool.CurCount); 
        }
#endif
    }
}