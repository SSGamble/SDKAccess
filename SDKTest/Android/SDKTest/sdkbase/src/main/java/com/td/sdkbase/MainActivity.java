package com.td.sdkbase;

import android.os.Bundle;
import android.widget.Toast;

import com.unity3d.player.*; // 是否能够 import

public class MainActivity extends UnityPlayerActivity {

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
    }

    /**
     * 供 Unity 调用的求和函数（静态）
     */
    public static int Sum(int x, int y) {
        return x + y;
    }

    /**
     * 供 Unity 调用的显示吐司的函数
     */
    public void MakeToast(String str) {
        Toast.makeText(this, str, Toast.LENGTH_LONG).show();
    }
}
