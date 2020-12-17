//#define UNITY_IOS
using System.Collections.Generic;
using UnityEngine;
using MiniGameSDK;
using System;
using ByteDance.Union;

namespace TTSDK
{
    public class BannerAd :MonoBehaviour, IBannerAd
    {
        ExpressAd mExpressBannerAd;
        ExpressAdInteractionListener expressAdInteractionListener = new ExpressAdInteractionListener(1);
        ExpressAdDislikeCallback dislikeCallback;
        public Action<int> onClose { get; set; }
        AdSlot adSlot;
        public Vector2 size = new Vector2(0.2f, 0.1f);
        public float reShowTime = 30;
        public bool useReShow;
        float add;
        bool isRun;
        public void Awake()
        {
            //Debug.Log($"w::{Screen.width},h::{Screen.height},sx::{size.x},sy::{size.y} rx::{size.x * Screen.width}，ry::{size.y * Screen.height}" );
            var tp = AdHelper.tp.bannerId;
            adSlot = new AdSlot.Builder()
                    .SetCodeId(tp)
                    ////期望模板广告view的size,单位dp，//高度按照实际rit对应宽高传入
                    .SetExpressViewAcceptedSize(size.x*0.5f * Screen.width, size.y * Screen.height)
                    .SetSupportDeepLink(true)
                    .SetImageAcceptedSize(1080, 1920)
                    .SetAdCount(1)
                    .SetOrientation(AdHelper.GetCurrentOrientation())
                    .Build();
            dislikeCallback = new ExpressAdDislikeCallback(this, 1);
            onClose += (v) =>
            {
                StartCountDown();
                LoadExpressBannerAd();
            };
#if !UNITY_EDITOR
            LoadExpressBannerAd();
#endif
        }
        private void Update()
        {
            if (isRun)
            {
                add += Time.deltaTime;
                if (add >= reShowTime)
                {
                    add = 0;
                    isRun = false;
                    if (useReShow)
                    {
                        Show();
                    }
                }
            }
        }
        public void Show()
        {
            ShowExpressBannerAd();
        }
        public void LoadExpressBannerAd()
        {
            AdHelper.AdNative.LoadExpressBannerAd(adSlot, new ExpressAdListener(this, 1));
        }
        public void ShowExpressBannerAd()
        {
            if (this.mExpressBannerAd == null)
            {
                return;
            }
            //设置轮播间隔 30s--120s;不设置则不开启轮播
            this.mExpressBannerAd.SetSlideIntervalTime(30 * 1000);
            this.mExpressBannerAd.SetDownloadListener(AdHelper.GetDownListener());
            NativeAdManager.Instance().ShowExpressBannerAd(AdHelper.GetActivity(), mExpressBannerAd.handle, expressAdInteractionListener, dislikeCallback);
        }
        public void StartCountDown()
        {
            add = 0;
            isRun = useReShow;
        }
        public void StopCountDown()
        {
            isRun = false;
        }
        public void Hide()
        {
            StopCountDown();
            if (this.mExpressBannerAd != null)
            {
                NativeAdManager.Instance().DestoryExpressAd(this.mExpressBannerAd.handle);
                this.mExpressBannerAd = null;
            }
            LoadExpressBannerAd();
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
                example.onClose?.Invoke(0);
                Debug.LogError("express dislike OnCancel");
            }

            public void OnRefuse()
            {
                example.onClose?.Invoke(1);
                Debug.LogError("express dislike onRefuse");
            }

            public void OnSelected(int var1, string var2)
            {
                example.onClose?.Invoke(2);
                Debug.LogError("express dislike OnSelected:" + var2);
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
            }
        }
        private sealed class ExpressAdListener : IExpressAdListener
        {
            private BannerAd example;
            private int type;//0:feed   1:banner  2:interstitial
            int retry = 1;
            public ExpressAdListener(BannerAd example, int type)
            {
                this.example = example;
                this.type = type;
            }
            public void OnError(int code, string message)
            {
                Debug.LogError("onExpressAdError: " + message);
                MonoEx.Wait(null, retry,  example.LoadExpressBannerAd);
                retry <<= 1;
            }

            public void OnExpressAdLoad(List<ExpressAd> ads)
            {
                retry = 1;
                Debug.LogError("OnExpressAdLoad");
                IEnumerator<ExpressAd> enumerator = ads.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    switch (type)
                    {
                        case 1:
                            this.example.mExpressBannerAd = enumerator.Current;
                            break;
                    }
                }
            }
        }
        private sealed class ExpressAdInteractionListener : IExpressAdInteractionListener
        {
            int type;//0:feed   1:banner  2:interstitial

            public ExpressAdInteractionListener(int type)
            {
                this.type = type;
            }
            public void OnAdClicked(ExpressAd ad)
            {
                Debug.LogError("express OnAdClicked,type:" + type);
            }

            public void OnAdShow(ExpressAd ad)
            {
                Debug.LogError("express OnAdShow,type:" + type);
            }

            public void OnAdViewRenderError(ExpressAd ad, int code, string message)
            {
                Debug.LogError("express OnAdViewRenderError,type:" + type);
            }

            public void OnAdViewRenderSucc(ExpressAd ad, float width, float height)
            {
                Debug.LogError("express OnAdViewRenderSucc,type:" + type);
            }
            public void OnAdClose(ExpressAd ad)
            {
                Debug.LogError("express OnAdClose,type:" + type);
            }
        }
    }
}
