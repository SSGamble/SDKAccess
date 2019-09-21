/****************************************************
    文件：GameObjectSimplify.cs
	作者：TravelerTD
    日期：2019/8/6 11:17:43
	功能：将 SetActive(true/false) 更改为 show，hide
*****************************************************/

#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace TDFramework {

    public static partial class GameObjectExtension {

        public static void Show(this GameObject gameObj) {
            gameObj.SetActive(true);
        }

        public static void Hide(this GameObject gameObj) {
            gameObj.SetActive(false);
        }

        public static void Show(this Transform transform) {
            transform.gameObject.SetActive(true);
        }

        public static void Hide(this Transform transform) {
            transform.gameObject.SetActive(false);
        }


        #region 静态 this 扩展
        public static void Show(this MonoBehaviour monoBehaviour) {
            monoBehaviour.gameObject.SetActive(true);
        }

        public static void Hide(this MonoBehaviour monoBehaviour) {
            monoBehaviour.gameObject.SetActive(true);
        }
        #endregion
    }
}