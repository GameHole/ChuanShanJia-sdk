﻿using ByteDance.Union;
using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
    public static class AdHelper
    {
        public static Vector2Int AdjustScreen(this Vector2Int v)
        {
            float scale = Screen.width / 1920f;
            return new Vector2Int((int)(v.x * scale), (int)(v.y * scale));
        }
        private static AdNative adNative;
        public static AdNative AdNative
        {
            get
            {
                if (adNative == null)
                {
                    adNative = SDK.CreateAdNative();
                }
                return adNative;
            }
        }
        static TPPama _tp;
        public static TPPama tp
        {
            get
            {
                return _tp ?? (_tp = AScriptableObject.Get<TPPama>());
            }
        }
        //private static AndroidJavaObject mNativeAdManager;
        //public static AndroidJavaObject GetNativeAdManager()
        //{
        //    if (mNativeAdManager == null)
        //    {
        //        var jc = new AndroidJavaClass(
        //                  "com.bytedance.android.NativeAdManager");
        //        if (jc != null)
        //            mNativeAdManager = jc.CallStatic<AndroidJavaObject>("getNativeAdManager");
        //    }
        //    return mNativeAdManager;
        //}
        public static IAppDownloadListener GetDownListener()
        {
            return new AppDownloadListener();
        }
        public static AdOrientation GetCurrentOrientation()
        {
            if (Screen.width >= Screen.height)
                return AdOrientation.Horizontal;
            return AdOrientation.Vertical;
        }
        sealed class AppDownloadListener : IAppDownloadListener
        {

            public void OnIdle()
            {
            }

            public void OnDownloadActive(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.Log("下载中，点击下载区域暂停");
            }

            public void OnDownloadPaused(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.Log("下载暂停，点击下载区域继续");
            }

            public void OnDownloadFailed(
                long totalBytes, long currBytes, string fileName, string appName)
            {
                Debug.LogError("下载失败，点击下载区域重新下载");
            }

            public void OnDownloadFinished(
                long totalBytes, string fileName, string appName)
            {
                Debug.Log("下载完成，点击下载区域重新下载");
            }

            public void OnInstalled(string fileName, string appName)
            {
                Debug.Log("安装完成，点击下载区域打开");
            }
        }
    }
}
