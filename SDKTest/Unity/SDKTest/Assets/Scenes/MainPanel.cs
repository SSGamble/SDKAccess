using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : MonoBehaviour {
    private AndroidJavaClass _jc;
    private AndroidJavaObject _jo;
    public Text resultLabel;

    void Start() {
        // 获取 Unity 导出的 Activity 对象，固定写法，UnityPlayerActivity 里面对其进行了处理
        _jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); // Unity 要导出的 MainActivity 类
        _jo = _jc.GetStatic<AndroidJavaObject>("currentActivity"); // 获取 MainActivity 的实例对象
    }

    public void Sum() {
        AndroidJavaClass jc = new AndroidJavaClass("com.td.sdkbase.MainActivity"); // 加载自己的类，指定实现了需要调用相应方法的类
        resultLabel.text = "Sum：" + jc.CallStatic<int>("Sum", 1, 5); // 调用 Java 类中的静态方法 Sum，返回值为 int 型，参数 1,5
    }

    public void Toast() {
        _jo.Call("MakeToast", "Unity 调用 Android，显示 Toast");  // 调用 Java 类中的普通方法
    }
}
