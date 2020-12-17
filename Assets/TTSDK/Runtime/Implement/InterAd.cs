using ByteDance.Union;
using MiniGameSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
    public class InterAd : IInterstitialAdAPI
    {
        public event Action<bool> onClose;
        private FullScreenVideoAd fullScreenVideoAd;
        AdSlot adSlot;
        FullScreenVideoAdListener listener;
        public InterAd()
        {
#if !UNITY_EDITOR
            adSlot = new AdSlot.Builder()
                             .SetCodeId(AdHelper.tp.intersititialId)
                             .SetSupportDeepLink(true)
                                 .SetImageAcceptedSize(1080, 1920)
                                 .SetOrientation(AdHelper.GetCurrentOrientation())
                                 .Build();
            listener = new FullScreenVideoAdListener(this);
            LoadFullScreenVideoAd();
#endif
        }


        public bool isReady()
        {
            return this.fullScreenVideoAd != null && fullScreenVideoAd.IsDownloaded;
        }

        public void Show()
        {
            ShowFullScreenVideoAd();
        }
        public void LoadFullScreenVideoAd()
        {
            if (this.fullScreenVideoAd != null)
            {
                return;
            }
            
            AdHelper.AdNative.LoadFullScreenVideoAd(adSlot, listener);

        }
        /// <summary>
        /// Show the reward Ad.
        /// </summary>
        public void ShowFullScreenVideoAd()
        {
            if (isReady())
            {
                this.fullScreenVideoAd.ShowFullScreenVideoAd();
            }
        }

        private sealed class FullScreenVideoAdListener : IFullScreenVideoAdListener
        {
            private InterAd inter;
            FullScreenAdInteractionListener listener;
            int retryCount = 1;
            public FullScreenVideoAdListener(InterAd example)
            {
                this.inter = example;
                listener = new FullScreenAdInteractionListener(this.inter);
            }

            public void OnError(int code, string message)
            {
                Debug.LogError("OnFullScreenError: " + message);
                MonoEx.Wait(null, retryCount, inter.LoadFullScreenVideoAd);
                retryCount <<= 1;
            }

            public void OnFullScreenVideoAdLoad(FullScreenVideoAd ad)
            {
                Debug.Log("OnFullScreenAdLoad");

                ad.SetFullScreenVideoAdInteractionListener(listener);
                ad.SetDownloadListener(AdHelper.GetDownListener());
                retryCount = 1;
                this.inter.fullScreenVideoAd = ad;
            }

            // iOS
            public void OnExpressFullScreenVideoAdLoad(ExpressFullScreenVideoAd ad)
            {
                // rewrite
            }

            public void OnFullScreenVideoCached()
            {
                Debug.Log("OnFullScreenVideoCached");
                if (this.inter.fullScreenVideoAd != null)
                {
                    this.inter.fullScreenVideoAd.IsDownloaded = true;
                }
            }
        }
        private sealed class FullScreenAdInteractionListener : IFullScreenVideoAdInteractionListener
        {
            private InterAd Inter;

            public FullScreenAdInteractionListener(InterAd example)
            {
                this.Inter = example;
            }

            public void OnAdShow()
            {
                Debug.Log("fullScreenVideoAd show");
            }

            public void OnAdVideoBarClick()
            {
                Debug.Log("fullScreenVideoAd bar click");
            }

            public void OnAdClose()
            {
                Debug.Log("fullScreenVideoAd close");
                this.Inter.fullScreenVideoAd = null;
                this.Inter.onClose?.Invoke(false);
                Inter.LoadFullScreenVideoAd();
            }

            public void OnVideoComplete()
            {
                Debug.Log("fullScreenVideoAd complete");
            }

            public void OnVideoError()
            {
                Debug.Log("fullScreenVideoAd OnVideoError");
            }

            public void OnSkippedVideo()
            {
                Debug.Log("fullScreenVideoAd OnSkippedVideo");
            }
        }
    }
}
