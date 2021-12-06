using ByteDance.Union;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
	public class Pangle_Android
	{
        internal static int NETWORK_STATE_MOBILE = 1;
        internal static int NETWORK_STATE_2G = 2;
        internal static int NETWORK_STATE_3G = 3;
        internal static int NETWORK_STATE_WIFI = 4;
        internal static int NETWORK_STATE_4G = 5;
        private static AndroidJavaObject activity;

        public static void InitializeSDK(Action<bool,string> callback, CustomConfiguration configuration = null)
        {
            Debug.Log("Pangle InitializeSDK start");
            var activity = ActivityGeter.GetActivity();
            if (activity != null)
            {
                var runnable = new AndroidJavaRunnable(() => initTTSdk(callback, configuration));
                activity.Call("runOnUiThread", runnable);
            }
        }

        private static void initTTSdk(Action<bool, string> callback, CustomConfiguration configuration)
        {
            var set = AScriptableObject.Get<TPPama>();
            Debug.Log("Pangle initTTSdk ");
            var sdkInitCallback = new SdkInitCallback(callback);
            AndroidJavaObject adConfigBuilder = new AndroidJavaObject("com.bytedance.sdk.openadsdk.TTAdConfig$Builder");
            Debug.Log("Pangle InitializeSDK 开始设置config");
            adConfigBuilder.Call<AndroidJavaObject>("appId", set.appid)
                .Call<AndroidJavaObject>("useTextureView", true) //使用TextureView控件播放视频,默认为SurfaceView,当有SurfaceView冲突的场景，可以使用TextureView
                .Call<AndroidJavaObject>("appName", "APP测试媒体")
                .Call<AndroidJavaObject>("allowShowNotify", true) //是否允许sdk展示通知栏提示
                .Call<AndroidJavaObject>("debug", set.isDebug) //测试阶段打开，可以通过日志排查问题，上线时去除该调用
                .Call<AndroidJavaObject>("directDownloadNetworkType",
                    new int[] { NETWORK_STATE_WIFI, NETWORK_STATE_3G, NETWORK_STATE_4G }) //允许直接下载的网络状态集合
                .Call<AndroidJavaObject>("themeStatus", 0)//设置主题类型，0：正常模式；1：夜间模式；默认为0；传非法值，按照0处理
                .Call<AndroidJavaObject>("supportMultiProcess", true) //是否支持多进程
                .Call<AndroidJavaObject>("data",
                    "[{\"name\":\"unity_version\",\"value\":\"" + PangleBase.PangleSdkVersion + "\"}]"); //传递unity版本号
            if (configuration != null)
            {
                adConfigBuilder.Call<AndroidJavaObject>("customController", MakeCustomController(configuration));
            }
            AndroidJavaObject adConfig = adConfigBuilder.Call<AndroidJavaObject>("build");
            var jc = new AndroidJavaClass(
                "com.bytedance.sdk.openadsdk.TTAdSdk");
            jc.CallStatic("init",ActivityGeter.GetActivity(), adConfig, sdkInitCallback);
        }

        private sealed class SdkInitCallback : AndroidJavaProxy
        {
            private readonly Action<bool, string>  listener;

            public SdkInitCallback(
                Action<bool, string>  listener)
                : base("com.bytedance.sdk.openadsdk.TTAdSdk$InitCallback")
            {
                this.listener = listener;
            }

            public void fail(int code, string message)
            {
                listener(false, message);
            }

            public void success()
            {
                listener(true, "sdk 初始化成功");
            }
        }


        private static AndroidJavaObject MakeCustomController(CustomConfiguration controller)
        {

            var customController = new AndroidJavaObject("com.bytedance.android.CustomController");
            if (controller != null)
            {
                customController.Call("canUseWifiState", controller.CanUseWifiState);
                customController.Call("setMacAddress", controller.MacAddress);
            }
            return customController;
        }
    }
}
