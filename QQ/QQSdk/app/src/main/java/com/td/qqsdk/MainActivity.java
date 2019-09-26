package com.td.qqsdk;

import android.content.Intent;
import android.os.Bundle;
import com.unity3d.player.*;

public class MainActivity extends UnityPlayerActivity {
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        GameHelper.Init(this); // 初始化 GameHelper
        TencentQQ.Init(this); // 初始化 TencentQQ
    }

    /**
     * 处理低端机内存紧张导致可能无回调
     */
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) {
        super.onActivityResult(requestCode,resultCode,data);
        TencentQQ.onActivityResult(requestCode, resultCode, data);
    }
}
