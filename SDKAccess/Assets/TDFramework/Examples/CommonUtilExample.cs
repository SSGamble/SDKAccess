/****************************************************
    文件：CommonUtilExample.cs
	作者：TravelerTD
    日期：2019/8/7 9:52:50
	功能：CommonUtil 示例代码
*****************************************************/

using UnityEngine;

namespace TDFramework {
    public class CommonUtilExample : MonoBehaviour {

#if UNITY_EDITOR
        [UnityEditor.MenuItem("TDFramework/Example/1.复制⽂本到剪切板", false, 2)]
#endif
        private static void MenuClicked2() {
            CommonUtil.CopyText("要复制的关键字");
        }
    }
}