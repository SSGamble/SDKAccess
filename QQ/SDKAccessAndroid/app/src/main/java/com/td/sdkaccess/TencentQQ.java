package com.td.sdkaccess;

import android.app.Activity;
import android.content.Intent;
import android.text.TextUtils;
import android.util.Log;
import com.tencent.connect.common.Constants;
import com.tencent.tauth.Tencent;
import org.json.JSONObject;
import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.net.HttpURLConnection;
import java.net.URL;

public class TencentQQ {

    private static Tencent mTenecnt; // Tencent 是 SDK 的功能入口，所有的接口调用都得通过 Tencent 进行调用
    private static Activity mMainActivity;
    public static String TAG = "TencentQQ";

    private static String APP_ID = "1056898435"; // OpenAPI 分配给第三方应用的 appid，类型为 String。
    private static BaseUiListener mLoginCallBack = new BaseUiListener(); // 登录回调

    public static void Init(Activity activity) {
        Log.d(TAG, "TencentQQ Init");
        mTenecnt = Tencent.createInstance(APP_ID, activity.getApplicationContext()); // 创建一个 Tencent 实例
        mMainActivity = activity;
    }

    /**
     * 处理低端机内存紧张导致可能无回调，在某些低端机上调用登录后，由于内存紧张导致 APP 被系统回收，登录成功后无法成功回传数据。
     */
    public static void onActivityResult(int requestCode, int resultCode, Intent data) {
        if (requestCode == Constants.REQUEST_API) {
            if (resultCode == Constants.REQUEST_LOGIN) {
                mTenecnt.handleResultData(data, mLoginCallBack);
            }
            mTenecnt.onActivityResultData(requestCode, resultCode, data, mLoginCallBack);
        }
    }

    /**
     * 登录函数
     */
    public static void Login() {
        Log.d(TAG, "QQLogin");
        mTenecnt.login(mMainActivity, "all", mLoginCallBack); // 调用者 activity，应用需要获得哪些 API 的权限（all 所有），回调
    }

    /**
     * 注销函数
     */
    public static void Logout() {
        Log.d(TAG, "QQLogout");
        mTenecnt.logout(mMainActivity);
    }

    /**
     * 检测授权是否过期
     */
    public static boolean CheckAutorVaild() {
        return mTenecnt.checkSessionValid(APP_ID);
    }

    /**
     * 刷新票据，自动登录
     */
    public static JSONObject RefreshSession() {
        JSONObject jsonObject = mTenecnt.loadSession(APP_ID);
        if (jsonObject == null) {
            Login();
        } else {
            mTenecnt.initSessionCache(jsonObject);
        }
        Log.d(TAG, "RefreshSession" + jsonObject.toString());
        return SetSelfData(jsonObject);
    }

    /**
     * 登录成功回调
     * @param jsonObject 回调 JSONObject
     */
    public static void LoginCallBack(final JSONObject jsonObject) {
        Log.d(TAG, "LoginCallBack:" + jsonObject.toString());
        new Thread(new Runnable() { // 获取 unionid 的时候有 http 请求
            @Override
            public void run() {
                initOpenidAndToken(jsonObject);
                JSONObject data = SetSelfData(jsonObject);
                GameHelper.SendPlatformMessageToUnity(GameHelper.PLATFORM_MSG_QQLOGINCALLBACK, 0, 0, 0, data.toString(), "", "");
            }
        }).start();
    }

    /**
     * 初始化 Tencent 中的 token 信息缓存
     * @param jsonObject
     */
    public static void initOpenidAndToken(JSONObject jsonObject) {
        try {
            String token = jsonObject.getString(Constants.PARAM_ACCESS_TOKEN);
            String expires = jsonObject.getString(Constants.PARAM_EXPIRES_IN); // 到期时间
            String openId = jsonObject.getString(Constants.PARAM_OPEN_ID);
            if (!TextUtils.isEmpty(token) && !TextUtils.isEmpty(expires) && !TextUtils.isEmpty(openId)) {
                mTenecnt.setAccessToken(token, expires);
                mTenecnt.setOpenId(openId);
            }
        } catch (Exception e) {
            Log.e(TAG, "initOpenidAndToken: " + e.toString());
        }
    }

    /**
     * 设置自己的 Json 格式，将返回的 Json 信息格式化成自己习惯的格式
     * @param data
     * @return
     */
    public static JSONObject SetSelfData(JSONObject data) {
        JSONObject jsonObject = new JSONObject();
        try {
            String token = data.getString(Constants.PARAM_ACCESS_TOKEN);
            String expires = data.getString(Constants.PARAM_EXPIRES_IN);
            String openId = data.getString(Constants.PARAM_OPEN_ID);
            String paytoken = data.getString("pay_token");
            String pf = data.getString("pf");
            String pfkey = data.getString("pfkey");
            String expiresTime = data.getString(Constants.PARAM_EXPIRES_TIME);
            String unionid = getUnionid(token);

            jsonObject.put("openid", openId);
            jsonObject.put("token", token);
            jsonObject.put("unionid", unionid);
            jsonObject.put("refreshtoken", "");
            jsonObject.put("expires", expires);
            jsonObject.put("paytoken", paytoken);
            jsonObject.put("pf", pf);
            jsonObject.put("pfkey", pfkey);
            jsonObject.put("expirestime", expiresTime);
        } catch (Exception e) {
            Log.e(TAG, "SetSelfData: 设置统一 QQ 票据失败！");
        }
        return jsonObject;
    }

    /**
     * 获取 unionid，唯一标识，http 请求
     */
    public static String getUnionid(String token) {
        String unionid = "";
        try {
            URL url = new URL("https://graph.qq.com/oauth2.0/me?access_token=" + token + "&unionid=1");
            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("GET");
            connection.setConnectTimeout(5000);
            connection.setReadTimeout(5000);

            int code = connection.getResponseCode();
            if (code == HttpURLConnection.HTTP_OK) {
                InputStream is = connection.getInputStream();
                byte[] data = readStream(is);
                String json = new String(data);
                json = json.replace("(", "").replace(")", "").replace("callback", "");
                JSONObject jsonObject = new JSONObject(json);
                unionid = jsonObject.getString("unionid");
            }
        } catch (Exception e) {
            Log.e(TAG, "getUniodid: 获取 unionid 错误：" + e.toString());
        }
        return unionid;
    }

    /**
     * 流转字节
     */
    private static byte[] readStream(InputStream inputStream) throws Exception {
        ByteArrayOutputStream bout = new ByteArrayOutputStream();
        byte[] buffer = new byte[1024];
        int len = 0;
        while ((len = inputStream.read(buffer)) != -1) {
            bout.write(buffer, 0, len);
        }
        bout.close();
        inputStream.close();
        return bout.toByteArray();
    }
}
