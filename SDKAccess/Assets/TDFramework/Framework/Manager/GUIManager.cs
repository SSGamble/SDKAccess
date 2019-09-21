/****************************************************
    文件：GUIManager.cs
	作者：TravelerTD
    日期：2019/8/7 11:54:3
	功能：Nothing
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TDFramework {
    public enum UILayer {
        Bg,
        Common,
        Top
    }
    public class GUIManager : MonoBehaviour {

        private static GameObject mPrivateUIRoot;
        // 单例模式，懒加载
        public static GameObject UIRoot {
            get {
                if (mPrivateUIRoot == null) {
                    var uirootPrefab = Resources.Load<GameObject>("UIRoot");
                    mPrivateUIRoot = GameObject.Instantiate(uirootPrefab);
                    mPrivateUIRoot.name = "UIRoot";
                }
                return mPrivateUIRoot;
            }
        }

        // 加载过的界⾯
        private static Dictionary<string, GameObject> mPanelsDict = new Dictionary<string, GameObject>();

        /// <summary>
        /// 加载 UIPanel
        /// </summary>
        /// <param name="panelName"></param>
        /// <param name="layer"></param>
        /// <returns></returns>
        public static GameObject LoadPanel(string panelName, UILayer layer) {
            var panelPrefab = Resources.Load<GameObject>(panelName);
            var panel = Instantiate(panelPrefab);
            panel.name = panelName;
            mPanelsDict.Add(panel.name, panel);
            switch (layer) {
                case UILayer.Bg:
                    panel.transform.SetParent(UIRoot.transform.Find("Bg"));
                    break;
                case UILayer.Common:
                    panel.transform.SetParent(UIRoot.transform.Find("Common"));
                    break;
                case UILayer.Top:
                    panel.transform.SetParent(UIRoot.transform.Find("Top"));
                    break;
            }
            var panelRectTrans = panel.transform as RectTransform;
            panelRectTrans.offsetMin = Vector2.zero;
            panelRectTrans.offsetMax = Vector2.zero;
            panelRectTrans.anchoredPosition3D = Vector3.zero;
            panelRectTrans.anchorMin = Vector2.zero;
            panelRectTrans.anchorMax = Vector2.one;
            return panel;
        }

        /// <summary>
        /// 卸载 UIPanel
        /// </summary>
        /// <param name="panelName"></param>
        public static void UnLoadPanel(string panelName) {
            if (mPanelsDict.ContainsKey(panelName)) {
                Destroy(mPanelsDict[panelName]);
            }
        }

        /// <summary>
        /// 屏幕适配
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="matchWidthOrHeight"></param>
        public static void SetResolution(float width, float height, float matchWidthOrHeight) {
            var canvasScaler = UIRoot.GetComponent<CanvasScaler>();
            canvasScaler.referenceResolution = new Vector2(width, height);
            canvasScaler.matchWidthOrHeight = matchWidthOrHeight;
        }
    }
}