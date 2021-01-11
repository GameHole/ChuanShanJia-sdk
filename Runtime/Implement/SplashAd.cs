﻿using ByteDance.Union;
using System;
using System.Collections.Generic;
using UnityEngine;
using MiniGameSDK;
namespace TTSDK
{
	public class SplashAd:ISplashAd,IReloader
    {
        IRetryer retryer;
        private AndroidJavaObject mSplashAdManager;
        public AndroidJavaObject GetSplashAdManager()
        {
            if (mSplashAdManager == null)
            {
                var jc = new AndroidJavaClass("com.bytedance.android.SplashAdManager");
                if (jc != null)
                    mSplashAdManager = jc.CallStatic<AndroidJavaObject>("getSplashAdManager");
            }
            return mSplashAdManager;
        }
        public Action onclose;

        public Action OnClsoe { get => onclose; set => onclose=value; }

        public int RetryCount => 3;

        public int IdCount => AdHelper.tp.splushIds.Length;

        public Action<bool> onReloaded { get ; set ; }

        //public void LoadSplashAd()
        //{
        //    retryer.Load(this);
        //}

        public void Show()
        {
#if !UNITY_EDITOR
            retryer.Regist(this);
#endif
        }

        public void Reload(int id)
        {
            Debug.Log($"load {AdHelper.tp.splushIds[id]}");
            var adSlot = new AdSlot.Builder()
            .SetCodeId(AdHelper.tp.splushIds[id])
            .SetImageAcceptedSize(1080, 1920)
            //.SetExpressViewAcceptedSize(Screen.width, Screen.height)
            .Build();
            AdHelper.AdNative.LoadSplashAd(adSlot, new SplashAdListener(AdHelper.GetActivity(), GetSplashAdManager()) { splash = this });
        }

        private sealed class SplashAdListener : ISplashAdListener, ISplashAdInteractionListener
        {
            public SplashAd splash;
            private AndroidJavaObject activity;
            private AndroidJavaObject splashAdManager;
            private const int INTERACTION_TYPE_DOWNLOAD = 4;

            public SplashAdListener(AndroidJavaObject activity, AndroidJavaObject splashAdManager)
            {
                this.activity = activity;
                this.splashAdManager = splashAdManager;
            }

            public void OnError(int code, string message)
            {
                splash.onReloaded?.Invoke(false);
                Debug.Log("splash load Onerror:" + code + ":" + message);
            }

            public void OnSplashAdLoad(BUSplashAd ad)
            {
                if (ad != null)
                {
                    splash.onReloaded?.Invoke(true);
                    Debug.Log("splash load Onsucc:");
                    ad.SetSplashInteractionListener(this);
                    if (ad.GetInteractionType() == INTERACTION_TYPE_DOWNLOAD)
                    {
                        Debug.Log("splash is download type ");
                        ad.SetDownloadListener(AdHelper.GetDownListener());
                    }
                }
#if UNITY_ANDROID
                if (ad != null && this.splashAdManager != null && this.activity != null)
                {
                    this.splashAdManager.Call("showSplashAd", this.activity, ad.getCurrentSplshAd());
                }
#endif
            }
            private void DestorySplash()
            {
#if UNITY_ANDROID
                if (splashAdManager != null && this.activity != null)
                {
                    splashAdManager.Call("destorySplashView", this.activity);
                }
#endif
                splash.onclose?.Invoke();
            }

            /// <summary>
            /// Invoke when the Ad is clicked.
            /// </summary>
            public void OnAdClicked(int type)
            {
                Debug.Log("splash Ad OnAdClicked type " + type);
#if UNITY_ANDROID
                if (type != INTERACTION_TYPE_DOWNLOAD)
                {
                    DestorySplash();
                }
#endif
            }

            /// <summary>
            /// Invoke when the Ad is shown.
            /// </summary>
            public void OnAdShow(int type)
            {
                Debug.Log("splash Ad OnAdShow");
            }

            /// <summary>
            /// Invoke when the Ad is skipped.
            /// </summary>
            public void OnAdSkip()
            {
                Debug.Log("splash Ad OnAdSkip");
                DestorySplash();
            }

            /// <summary>
            /// Invoke when the Ad time over.
            /// </summary>
            public void OnAdTimeOver()
            {
                Debug.Log("splash Ad OnAdTimeOver");
                DestorySplash();
            }

            public void OnAdClose()
            {
                DestorySplash();
            }
        }
    }
}
