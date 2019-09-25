/****************************************************
    文件：TransformSimplify.cs
	作者：TravelerTD
    日期：2019/8/6 10:35:8
	功能：Transform 优化
*****************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TDFramework {

    public static partial class TransformExtension {

        #region Transform 赋值优化
        #region 测试
        //#if UNITY_EDITOR
        //        [MenuItem("TDFramework/9.Transform 赋值优化")]
        //#endif

        //private static void GenerateUnityPackageName() {
        //    var transform = new GameObject("transform").transform;
        //    SetLocalPosX(transform, 5.0f);
        //    SetLocalPosY(transform, 5.0f);
        //    SetLocalPosZ(transform, 5.0f);
        //}
        #endregion

        public static void SetLocalPosX(this Transform transform, float x) {
            var localPos = transform.localPosition;
            localPos.x = x;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosY(this Transform transform, float y) {
            var localPos = transform.localPosition;
            localPos.y = y;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosZ(this Transform transform, float z) {
            var localPos = transform.localPosition;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        // 在对 XY、 XZ 和 YZ 赋值时，可以直接调⽤ SetPositionX、 SetPositionY 、 SetPositionZ，好处是代码能够复⽤，但每次进⾏⼀次调⽤，其实是⼀次值类型的复制操作，从性能的⻆度来讲不推荐。所以这里直接重新实现逻辑全部

        public static void SetLocalPosXY(this Transform transform, float x, float y) {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.y = y;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosXZ(this Transform transform, float x, float z) {
            var localPos = transform.localPosition;
            localPos.x = x;
            localPos.z = z;
            transform.localPosition = localPos;
        }

        public static void SetLocalPosYZ(this Transform transform, float y, float z) {
            var localPos = transform.localPosition;
            localPos.y = y;
            localPos.z = z;
            transform.localPosition = localPos;
        }
        #endregion

        /// <summary>
        /// 重置 Transform
        /// </summary>
        public static void Identity(this Transform transform) {
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = Quaternion.identity;
        }

        /// <summary>
        /// 重置操作，MonoBehaviour this 扩展
        /// </summary>
        public static void Identity(this MonoBehaviour monoBehaviour) {
            monoBehaviour.transform.Identity();
        }

        /// <summary>
        /// 添加子物体
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childTrans"></param>
        public static void AddChild(this Transform transform, Transform childTrans) {
            childTrans.SetParent(transform);
        }
    }
}