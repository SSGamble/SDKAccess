/****************************************************
    文件：StaticThisExtensions.cs
	作者：TravelerTD
    日期：2019/8/7 19:42:40
	功能：静态 this 扩展
*****************************************************/

using UnityEngine;
namespace TDFramework {
    public static class StaticThisExtension {
#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/18.StaticThisExtension", false, 18)]
#endif
        static void MenuClicked() {
            new object().Test();
            "string".Test();
        }

        // Test 的第⼀个参数是 object 类型的，但是前边有个 this 关键字，有了这个 this 关键字，我们的 object 对象就可以像调⽤⾃⼰的⽅法⼀样调⽤ Test ⽅法。
        static void Test(this object selfObj) {
            Debug.Log(selfObj);
        }
    }
}