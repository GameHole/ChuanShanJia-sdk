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
#if !UNITY_EDITOR
            retryer.Regist(this);
#endif
        }

        public bool isReady()
        {
            return mExpressInterstitialAd != null;
        }

        public void Reload(int id)
        {
            var adSlot = new AdSlot.Builder()
                     .SetCodeId(AdHelper.tp.dialogPageIds[id])
                     .SetExpressViewAcceptedSize(350, 0)
                      ////期望模板广告view的size,单位dp，//高度设置为0,则高度会自适应
                     .SetSupportDeepLink(true)
                     .SetAdCount(1)
                     .SetImageAcceptedSize(Screen.width, Screen.height)
                     .SetOrientation(AdHelper.GetCurrentOrientation())
                     .Build();
            AdHelper.AdNative.LoadExpressInterstitialAd(adSlot, listener);
        }
        public void LoadExpressInterstitialAd()
        {
            retryer.Load(this);
        }
        public void ShowExpressInterstitialAd()
        {
            if (!isReady())
            {
                return;
            }
            this.mExpressInterstitialAd.SetDownloadListener(AdHelper.GetDownListener());
            NativeAdManager.Instance().ShowExpressInterstitialAd(ActivityGeter.GetActivity(), mExpressInterstitialAd.handle, expressAdInteractionListener);
        }
        public void Show()
        {
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
                Debug.Log("onDialogPageAdError: " + message);
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
            //Debug.Log("OnExpressBannerAdLoad");
            //this.example.information.text = "OnExpressBannerAdLoad";
            //ad.SetExpressInteractionListener(
            //    new ExpressAdInteractionListener(this.example,1));
            //ad.SetDownloadListener(
            //    new AppDownloadListener(this.example));
            //this.example.iExpressBannerAd = ad;
        }

        public void OnExpressInterstitialAdLoad(ExpressInterstitialAd ad)
        {
            //Debug.Log("OnExpressInterstitialAdLoad");
            //this.example.information.text = "OnExpressInterstitialAdLoad";
            //ad.SetExpressInteractionListener(
            //    new ExpressAdInteractionListener(this.example, 2));
            //ad.SetDownloadListener(
            //    new AppDownloadListener(this.example));
            //this.example.iExpressInterstitialAd = ad;
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
                example.mExpressInterstitialAd = null;
                //example.onClose?.Invoke(true);
                example.LoadExpressInterstitialAd();
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
                
            }
        }
    }
}
