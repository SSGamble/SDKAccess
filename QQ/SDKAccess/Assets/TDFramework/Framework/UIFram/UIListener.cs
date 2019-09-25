/****************************************************
    文件：UIListener.cs
	作者：TravelerTD
    日期：2019/08/29 16:12:34
	功能：UI 事件监听，当有物体挂载这个脚本后，只需要对 action 赋值，然后进行根据操作触发相应的函数
*****************************************************/

using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler {

    public object args; // 传递的参数
    public Action<object> onClick;
    public Action<PointerEventData> onClickDown;
    public Action<PointerEventData> onClickUp;
    public Action<PointerEventData> onDrag;

    /// <summary>
    /// 带参数，点击
    /// </summary>
    public void OnPointerClick(PointerEventData eventData) {
        if (onClick != null) {
            onClick(args);
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        if (onClickDown != null) {
            onClickDown(eventData);
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if (onClickUp != null) {
            onClickUp(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData) {
        if (onDrag != null) {
            onDrag(eventData);
        }
    }
}