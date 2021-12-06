using System.Collections.Generic;
using UnityEngine;
using MiniGameSDK;
using System;
using ByteDance.Union;

namespace TTSDK
{
    public class BannerAd :MonoBehaviour, IBannerAd,IReloader
    {
        public enum Gravity
        {
            BOTTOM = 80, CENTER = 17, LEFT = 3, NO_GRAVITY = 0, RIGHT = 5, TOP = 48
        }
        IRetryer retryer;
        ExpressAd mExpressBannerAd;
#if UNITY_IOS
        ExpressBannerAd iExpressBannerAd; // for iOS
#endif
        ExpressAdInteractionListener expressAdInteractionListener;
        ExpressAdDislikeCallback dislikeCallback;
        ExpressAdListener listener;
        public Action<int> onClose { get; set; }

        public int RetryCount => 3;
        public Gravity[] gravities = new Gravity[]
        {
             Gravity.BOTTOM, Gravity.CENTER
        };
        public int IdCount =>AdHelper.tp.bannerIds.Length;

        public Action<bool> onReloaded { get ; set ; }
        public Action onHide { get ; set; }

        public int widthDp=600;
        public int hightDp=90;
        public int intervalTime = 30;
        public event Action onShow;

//#if UNITY_ANDROID
//        AndroidJavaClass mgr;
//#endif

        public void Awake()
        {
            //Debug.Log($"w::{Screen.width},h::{Screen.height},sx::{size.x},sy::{size.y} rx::{size.x * Screen.width}，ry::{size.y * Screen.height}" );
            expressAdInteractionListener = new ExpressAdInteractionListener(1,this);
            dislikeCallback = new ExpressAdDislikeCallback(this, 1);
            onClose += (v) =>
            {
                //StartCountDown();
                LoadExpressBannerAd();
            };
            listener = new ExpressAdListener(this, 1);
#if !UNITY_EDITOR
            retryer.Regist(this);
#endif
        }
        public void Show()
        {
            //Debug.Log("TT banner show");
            ShowExpressBannerAd();
        }
        public void LoadExpressBannerAd()
        {
            retryer.Load(this);
        }
        public void ShowExpressBannerAd()
        {
            if (this.mExpressBannerAd == null)
            {
                return;
            }
#if UNITY_ANDROID
            //if (mgr == null)
            //    mgr = new AndroidJavaClass("com.bytedance.android.NativeAdManager");
            //int g = 0;
            //for (int i = 0; i < gravities.Length; i++)
            //{
            //    g |= (int)gravities[i];
            //}
            //mgr.CallStatic("SetGravity", "mark", g);
            //设置轮播间隔 30s--120s;不设置则不开启轮播
            if (intervalTime > 0)
                this.mExpressBannerAd.SetSlideIntervalTime(intervalTime * 1000);
            this.mExpressBannerAd.SetDownloadListener(AdHelper.GetDownListener());
            NativeAdManager.Instance().ShowExpressBannerAd(ActivityGeter.GetActivity(), mExpressBannerAd.handle, expressAdInteractionListener, dislikeCallback);
#endif
#if UNITY_IOS
            iExpressBannerAd.ShowExpressAd(0, Screen.height - hightDp);
#endif
        }
        public void Hide()
        {
            //StopCountDown();
            onHide?.Invoke();
#if UNITY_ANDROID
            if (this.mExpressBannerAd != null)
            {
                NativeAdManager.Instance().DestoryExpressAd(this.mExpressBannerAd.handle);
                this.mExpressBannerAd = null;
            }
#endif
            LoadExpressBannerAd();
        }

        public void Reload(int id)
        {
            if (widthDp == 0)
            {
                widthDp = Screen.width;
            }
            if (hightDp == 0)
            {
                hightDp = widthDp / 600 * 90;
            }
            var tp = AdHelper.tp.bannerIds[id];
            var adSlot = new AdSlot.Builder()
                    .SetCodeId(tp)
#if UNITY_IOS
                     .SetSlideIntervalTime(intervalTime)
#endif
                    .SetExpressViewAcceptedSize(widthDp, hightDp)
                    .SetSupportDeepLink(true)
                    .SetImageAcceptedSize(Screen.width, Screen.height)
                    .SetAdCount(1)
                    .SetOrientation(AdHelper.GetCurrentOrientation())
#if UNITY_ANDROID
                    .SetDownloadType(DownloadType.DownloadTypeNoPopup)
#endif
                    .Build();
            AdHelper.AdNative.LoadExpressBannerAd(adSlot, listener);
        }

        private sealed class ExpressAdDislikeCallback : IDislikeInteractionListener
        {
            private BannerAd example;
            int type;//0:feed   1:banner
            public ExpressAdDislikeCallback(BannerAd example, int type)
            {
                this.example = example;
                this.type = type;
            }
            public void OnCancel()
            {
                //example.onClose?.Invoke(0);
                Debug.Log("express dislike OnCancel");
            }

            public void OnRefuse()
            {
                //example.onClose?.Invoke(1);
                Debug.Log("express dislike onRefuse");
            }

            public void OnSelected(int var1, string var2, bool enforce)
            {
                example.onClose?.Invoke(2);
                Debug.Log("express dislike OnSelected:" + var2);
#if UNITY_ANDROID
                //释放广告资源
                switch (type)
                {
                    case 1:
                        if (this.example.mExpressBannerAd != null)
                        {
                            NativeAdManager.Instance().DestoryExpressAd(this.example.mExpressBannerAd.handle);
                            this.example.mExpressBannerAd = null;
                        }
                        break;
                }
#endif
            }

            public void OnShow()
            {
                Debug.Log("express dislike OnShow");
            }
        }
        private sealed class ExpressAdListener : IExpressAdListener
        {
            private BannerAd example;
            private int type;//0:feed   1:banner  2:interstitial
            //int retry = 1;
            public ExpressAdListener(BannerAd example, int type)
            {
                this.example = example;
                this.type = type;
            }
            public void OnError(int code, string message)
            {
                Debug.LogError("onExpressBannerAdError: " + message);
                example.onReloaded?.Invoke(false);
            }

            public void OnExpressAdLoad(List<ExpressAd> ads)
            {
                example.onReloaded?.Invoke(true);
                //retry = 1;
                Debug.Log("OnExpressBannerAdLoad");
                IEnumerator<ExpressAd> enumerator = ads.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    switch (type)
                    {
                        case 1:
                            this.example.mExpressBannerAd = enumerator.Current;
                            this.example.mExpressBannerAd.SetDownloadListener(AdHelper.GetDownListener());
                            break;
                    }
                }
            }
#if UNITY_IOS
            public void OnExpressBannerAdLoad(ExpressBannerAd ad)
            {
                ad.SetExpressInteractionListener(example.expressAdInteractionListener);
                ad.SetDownloadListener(AdHelper.GetDownListener());
                example.iExpressBannerAd = ad;
            }

            public void OnExpressInterstitialAdLoad(ExpressInterstitialAd ad)
            {
                
            }
#endif
        }
        private sealed class ExpressAdInteractionListener : IExpressAdInteractionListener
        {
            int type;//0:feed   1:banner  2:interstitial
            BannerAd banner;
            public ExpressAdInteractionListener(int type,BannerAd banner)
            {
                this.banner = banner;
                this.type = type;
            }
            public void OnAdClicked(ExpressAd ad)
            {
                Debug.Log("express OnAdClicked,type:" + type);
            }

            public void OnAdShow(ExpressAd ad)
            {
                banner.onShow?.Invoke();
                Debug.Log("express OnAdShow,type:" + type);
            }

            public void OnAdViewRenderError(ExpressAd ad, int code, string message)
            {
                Debug.Log("express OnAdViewRenderError,type:" + type);
            }

            public void OnAdViewRenderSucc(ExpressAd ad, float width, float height)
            {
                Debug.Log("express OnAdViewRenderSucc,type:" + type);
            }
            public void OnAdClose(ExpressAd ad)
            {
                banner.onHide?.Invoke();
                Debug.Log("express OnAdClose,type:" + type);
            }

            public void onAdRemoved(ExpressAd ad)
            {
                Debug.Log("express onAdRemoved,type:" + type);
            }
        }
    }
}
