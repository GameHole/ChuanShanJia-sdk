package com.unity.ttsdk;

import android.app.Activity;
import android.os.HandlerThread;
import android.util.Log;

import com.bytedance.sdk.openadsdk.TTAdConfig;
import com.bytedance.sdk.openadsdk.TTAdConstant;
import com.bytedance.sdk.openadsdk.TTAdSdk;
import com.bytedance.sdk.openadsdk.TTCustomController;
import com.bytedance.sdk.openadsdk.TTLocation;

public class AdInit{
    public  static  void  Init(String mark, final Activity activity, final String appid, final String appname, final boolean debug, final boolean enable,final  IInitOver ac){
        Thread thread=new Thread(new Runnable() {
            @Override
            public void run() {
                TTAdConfig config = new TTAdConfig.Builder()
                        .appId(appid)
                        .useTextureView(false) //使用TextureView控件播放视频,默认为SurfaceView,当有SurfaceView冲突的场景，可以使用TextureView
                        .appName(appname)
                        .titleBarTheme(TTAdConstant.TITLE_BAR_THEME_DARK)
                        .allowShowNotify(true) //是否允许sdk展示通知栏提示
                        .allowShowPageWhenScreenLock(true) //是否在锁屏场景支持展示广告落地页
                        .debug(debug) //测试阶段打开，可以通过日志排查问题，上线时去除该调用
                        .directDownloadNetworkType(TTAdConstant.NETWORK_STATE_WIFI, TTAdConstant.NETWORK_STATE_3G,TTAdConstant.NETWORK_STATE_4G,TTAdConstant.NETWORK_STATE_MOBILE) //允许直接下载的网络状态集合
                        .supportMultiProcess(true) //是否支持多进程，true支持
                        .customController(getController(enable))//控制隐私数据
                        .build();
                TTAdSdk.init(activity, config);
                Log.v("Unity","aaaaaaa");
                if(ac!= null) {
                    ac.OnOver();
                }
            }
        });
        thread.start();
    }
    private static TTCustomController getController(boolean enable) {
        MyTTCustomController customController = new MyTTCustomController();
        customController.enable=enable;
        return customController;
    }

    private static class MyTTCustomController extends TTCustomController {
      public  boolean enable;

        @Override
        public boolean isCanUseLocation() {
            return enable&&super.isCanUseLocation();
        }

        @Override
        public TTLocation getTTLocation() {
            return super.getTTLocation();
        }

        @Override
        public boolean alist() {
            return enable&&super.alist();
        }

        @Override
        public boolean isCanUsePhoneState() {
            return enable&&super.isCanUsePhoneState();
        }

        @Override
        public String getDevImei() {
            return super.getDevImei();
        }

        @Override
        public boolean isCanUseWifiState() {
            return enable&&super.isCanUseWifiState();
        }

        @Override
        public boolean isCanUseWriteExternal() {
            return enable&&super.isCanUseWriteExternal();
        }
    }
}
