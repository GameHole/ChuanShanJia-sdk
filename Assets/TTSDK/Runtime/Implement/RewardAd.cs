using ByteDance.Union;
using MiniGameSDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace TTSDK
{
    public class RewardAd : MonoBehaviour, IRewardAdAPI,IReloader
    {
        public bool debug;
        public bool isNotUseAd { get => debug; set => debug = value; }
        public bool isRewared { get; set; }

        public int RetryCount => 3;

        public int IdCount => AdHelper.tp.rewardIds.Length;

        public Action<bool> onReloaded { get ; set ; }

        public string userId = "user123";
        public event Action<bool> onClose;
        Action<bool> used_onClose;
        RewardVideoAd rewardAd;
        AdNative adNative;
        TaskCompletionSource<bool> tcs;
        RewardVideoAdListener listener;
        //AdSlot adSlot;
        public IRetryer retryer;
        bool isShowed;
        private void Awake()
        {

            listener = new RewardVideoAdListener(this);
            retryer.Regist(this);
            //LoadRewardAd();
        }
        public void AutoShow(Action<bool> onclose)
        {
            used_onClose = onclose + onClose;
            ShowRewardAd();
        }

        public Task<bool> AutoShowAsync()
        {
            tcs = new TaskCompletionSource<bool>();
            ShowRewardAd();
            return tcs.Task;
        }
        public void Onclose(bool isEnd)
        {
            if (isRewared) return;
            isRewared = true;
            isShowed = false;
            used_onClose?.Invoke(isEnd);
            if (tcs != null && !tcs.Task.IsCompleted)
                tcs?.SetResult(isEnd);
        }
        public bool isReady()
        {
            return rewardAd != null /*&& rewardAd.IsDownloaded*/;
        }
        /// <summary>
        /// Load the reward Ad.
        /// </summary>
        public void LoadRewardAd()
        {
            retryer.Load(this);
        }
        /// <summary>
        /// Show the reward Ad.
        /// </summary>
        public void ShowRewardAd()
        {
            //isShowed = true;//for test
            isRewared = false;
            if (isNotUseAd)
            {
                Onclose(true);
                return;
            }
            if (isReady())
            {
                rewardAd.ShowRewardVideoAd();
            }
        }
        //只有安卓需要解决切出重新加载问题
#if UNITY_ANDROID
        private void OnApplicationPause(bool pause)
        {
            if (isShowed && !pause)
            {
                ReleaseAd();
                LoadRewardAd();
            }
        }
#endif
        void ReleaseAd()
        {
            rewardAd?.Dispose();
            rewardAd = null;
        }
        public void Reload(int id)
        {
            if (PlatfotmHelper.isEditor()) return;
            if (rewardAd != null) return;
            //Debug.Log($"reward id::{AdHelper.tp.rewardIds[id]}");
            var adSlot = new AdSlot.Builder()
                 .SetCodeId(AdHelper.tp.rewardIds[id])
                 .SetSupportDeepLink(true)
                 .SetImageAcceptedSize(1080, 1920)
#if UNITY_ANDROID
                 .SetRewardName("金币") // 奖励的名称

                 .SetRewardAmount(3) // 奖励的数量
#endif
                 .SetUserID(userId) // 用户id,必传参数
                 .SetMediaExtra("media_extra") // 附加参数，可选
                 .SetOrientation(AdHelper.GetCurrentOrientation()) // 必填参数，期望视频的播放方向
#if UNITY_ANDROID
                .SetDownloadType(DownloadType.DownloadTypeNoPopup)
#endif
                 .Build();

            AdHelper.AdNative.LoadRewardVideoAd(adSlot, listener);
        }

        private sealed class RewardVideoAdListener : IRewardVideoAdListener
        {
            private RewardAd reward;
            RewardAdInteractionListener listener;
            //int retryCount = 1;
            public RewardVideoAdListener(RewardAd example)
            {
                this.reward = example;
                listener = new RewardAdInteractionListener(this.reward);
            }

            public void OnError(int code, string message)
            {
                reward.onReloaded?.Invoke(false);
                Debug.LogError("OnRewardError: " + message);
            }
            void GetAd(RewardVideoAd ad)
            {
                reward.onReloaded?.Invoke(true);
                ad.SetRewardAdInteractionListener(listener);
                ad.SetDownloadListener(AdHelper.GetDownListener());
                //retryCount = 1;
                //reward.retryer.Clear(reward.retryId);
                this.reward.rewardAd = ad;
            }
            public void OnRewardVideoAdLoad(RewardVideoAd ad)
            {
                Debug.Log("OnRewardVideoAdLoad");
                GetAd(ad);
            }

            public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
            {
                Debug.Log("OnExpressRewardVideoAdLoad");
            }

            public void OnRewardVideoCached()
            {
                Debug.Log("OnRewardVideoCached");
            }

            public void OnRewardVideoCached(RewardVideoAd ad)
            {
                Debug.Log("OnRewardVideoCached Wirh ad");
                //GetAd(ad);
            }
        }

        private sealed class RewardAdInteractionListener : IRewardAdInteractionListener
        {
            private RewardAd reward;
            bool isReward;
            public RewardAdInteractionListener(RewardAd example)
            {
                this.reward = example;
            }

            public void OnAdShow()
            {
                reward.isShowed = true;
                isReward = false;
                Debug.Log("rewardVideoAd show");
            }

            public void OnAdVideoBarClick()
            {
                Debug.Log("rewardVideoAd bar click");
            }

            public void OnAdClose()
            {
                Debug.Log("rewardVideoAd close");
                this.reward.ReleaseAd();
                this.reward.Onclose(isReward);
                this.reward.LoadRewardAd();
            }

            public void OnVideoComplete()
            {
                Debug.Log("rewardVideoAd complete");
            }

            public void OnVideoError()
            {
                Debug.LogError("rewardVideoAd error");
            }

            public void OnRewardVerify(bool rewardVerify, int rewardAmount, string rewardName)
            {
                isReward = true;
                this.reward.Onclose(isReward);
                Debug.Log("verify:" + rewardVerify + " amount:" + rewardAmount +
                    " name:" + rewardName);
            }

            public void OnVideoSkip()
            {
                Debug.Log("rewardVideoAd OnVideoSkip");
            }
        }
    }
}
