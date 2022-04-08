
using System.Collections.Generic;
using UnityEngine;
using MiniGameSDK;
using System;
using ByteDance.Union;

namespace TTSDK
{
    public class DialogPageAd :MonoBehaviour,IDialogPageAd, IReloader
    {
        ExpressAd mExpressInterstitialAd;
#if UNITY_IOS
        ExpressInterstitialAd iExpressInterstitialAd; // for iOS
#endif
        ExpressAdListener listener;
        ExpressAdInteractionListener expressAdInteractionListener;
        IRetryer retryer;
        public int RetryCount => 3;

        public int IdCount => AdHelper.tp.dialogPageIds.Length;

        public Action<bool> onReloaded { get; set ; }

        public event Action<bool> onClose;

        private void Awake()
        {
            listener = new ExpressAdListener(this);
            expressAdInteractionListener = new ExpressAdInteractionListener(this);
            retryer.Regist(this);
            //Debug.Log( GetSize());
        }

        public bool isReady()
        {
            return mExpressInterstitialAd != null;
        }
        Vector2Int GetSize()
        {
            var size = new Vector2Int(350, 0);
#if UNITY_IOS
            size.y = size.x;
            size = size.AdjustScreen();
            size *= 2;
#endif
            return size;
        }
        public void Reload(int id)
        {
            if (PlatfotmHelper.isEditor()) return;
            var size = GetSize();
            var adSlot = new AdSlot.Builder()
                     .SetCodeId(AdHelper.tp.dialogPageIds[id])
                     .SetExpressViewAcceptedSize(size.x, size.y)
                      ////期望模板广告view的size,单位dp，//高度设置为0,则高度会自适应
                     .SetSupportDeepLink(true)
                     .SetAdCount(1)
                     .SetImageAcceptedSize(Screen.width, Screen.height)
                     .SetOrientation(AdHelper.GetCurrentOrientation())
#if UNITY_ANDROID
                     .SetDownloadType(DownloadType.DownloadTypeNoPopup)
#endif
                     .Build();
            AdHelper.AdNative.LoadExpressInterstitialAd(adSlot, listener);
        }
        public void LoadExpressInterstitialAd()
        {
            retryer.Load(this);
        }
        public void ShowExpressInterstitialAd()
        {
#if UNITY_ANDROID
            this.mExpressInterstitialAd.SetDownloadListener(AdHelper.GetDownListener());
            NativeAdManager.Instance().ShowExpressInterstitialAd(ActivityGeter.GetActivity(), mExpressInterstitialAd.handle, expressAdInteractionListener);
#endif
#if UNITY_IOS
            this.iExpressInterstitialAd?.ShowExpressAd(0, 0);
#endif
        }
        public void Show()
        {
            if (PlatfotmHelper.isEditor()) return;
            ShowExpressInterstitialAd();
        }

        private sealed class ExpressAdListener : IExpressAdListener
        {
            private DialogPageAd example;

            public ExpressAdListener(DialogPageAd example)
            {
                this.example = example;
            }
            public void OnError(int code, string message)
            {
                Debug.LogError("onDialogPageAdError: " + message);
                example.onReloaded?.Invoke(false);
            }

            public void OnExpressAdLoad(List<ExpressAd> ads)
            {
                Debug.Log("OnDialogPageAdLoad");
                example.onReloaded?.Invoke(true);
                Debug.Log($"DialogPageAd c::{ads.Count}");
                IEnumerator<ExpressAd> enumerator = ads.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    this.example.mExpressInterstitialAd = enumerator.Current;
                }
            }
#if UNITY_IOS

        public void OnExpressBannerAdLoad(ExpressBannerAd ad)
        {
        }

        public void OnExpressInterstitialAdLoad(ExpressInterstitialAd ad)
        {
                Debug.Log("OnDialogPageAdLoad");
                ad.SetExpressInteractionListener(example.expressAdInteractionListener);
                ad.SetDownloadListener(AdHelper.GetDownListener());
                this.example.iExpressInterstitialAd = ad;
            }
#endif
        }

        private sealed class ExpressAdInteractionListener : IExpressAdInteractionListener
        {
            private DialogPageAd example;

            public ExpressAdInteractionListener(DialogPageAd example)
            {
                this.example = example;
            }
            public void OnAdClicked(ExpressAd ad)
            {
                Debug.Log("DialogPageAd OnAdClicked");
            }

            public void OnAdShow(ExpressAd ad)
            {
                Debug.Log("DialogPageAd OnAdShow");
            }

            public void OnAdViewRenderError(ExpressAd ad, int code, string message)
            {
                Debug.Log("DialogPageAd OnAdViewRenderError");
            }

            public void OnAdViewRenderSucc(ExpressAd ad, float width, float height)
            {
                Debug.Log("DialogPageAd OnAdViewRenderSucc");
            }
            public void OnAdClose(ExpressAd ad)
            {
                Debug.Log("DialogPageAd OnAdClose");
                example.mExpressInterstitialAd = null;
                example.onClose?.Invoke(true);
                example.LoadExpressInterstitialAd();
            }

            public void onAdRemoved(ExpressAd ad)
            {
                Debug.Log("DialogPageAd onAdRemoved");
            }
        }
    }
}
