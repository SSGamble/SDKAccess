package com.td.qqsdk;

import android.util.Log;
import com.tencent.tauth.IUiListener;
import com.tencent.tauth.UiError;
import org.json.JSONObject;

/**
 * 回调
 */
public class BaseUiListener implements IUiListener {

    /**
     * 操作完成后的回调
     */
    @Override
    public void onComplete(Object o) {
        if (o == null)
            return;
        JSONObject jsonObject = (JSONObject) o;
        if (jsonObject != null && jsonObject.length() == 0) {
            return;
        }
        Log.d(TencentQQ.TAG, "onComplete: 登录成功！");
        TencentQQ.LoginCallBack(jsonObject);
    }

    /**
     * 操作错误的回调
     */
    @Override
    public void onError(UiError uiError) {
        Log.d(TencentQQ.TAG, "onComplete: 登录失败！");
        GameHelper.SendPlatformMessageToUnity(GameHelper.PLATFORM_MSG_QQLOGIN, 1, 0, 0, "", "", "");
    }

    /**
     * 操作取消的回调
     */
    @Override
    public void onCancel() {
        Log.d(TencentQQ.TAG, "onComplete: 取消登录！");
        GameHelper.SendPlatformMessageToUnity(GameHelper.PLATFORM_MSG_QQLOGIN, 2, 0, 0, "", "", "");
    }
}


