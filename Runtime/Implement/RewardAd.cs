using ByteDance.Union;
using MiniGameSDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace TTSDK
{
    public class RewardAd : MonoBehaviour, IRewardAdAPI
    {
        public bool debug;
        public bool isNotUseAd { get => debug; set => debug = value; }
        public bool isRewared { get; set; }
        public string userId = "user123";
        public event Action<bool> onClose;
        Action<bool> used_onClose;
        RewardVideoAd rewardAd;
        AdNative adNative;
        TaskCompletionSource<bool> tcs;
        RewardVideoAdListener listener;
        AdSlot adSlot;
        private void Awake()
        {
#if !UNITY_EDITOR
            listener = new RewardVideoAdListener(this);
            adSlot = new AdSlot.Builder()
            .SetCodeId(AdHelper.tp.rewardId)
            .SetSupportDeepLink(true)
                .SetImageAcceptedSize(1080, 1920)
                .SetRewardName("金币") // 奖励的名称
                .SetRewardAmount(3) // 奖励的数量
                .SetUserID(userId) // 用户id,必传参数
                .SetMediaExtra("media_extra") // 附加参数，可选
                .SetOrientation(AdHelper.GetCurrentOrientation()) // 必填参数，期望视频的播放方向
                .Build();
            LoadRewardAd();
#endif
        }
        public void AutoShow(Action<bool> onclose)
        {
            isRewared = false;
            used_onClose = onclose + onClose;
            ShowRewardAd();
        }

        public Task<bool> AutoShowAsync()
        {
            isRewared = false;
            tcs = new TaskCompletionSource<bool>();
            return tcs.Task;
        }
        public void Onclose(bool isEnd)
        {
            if (isRewared) return;
            isRewared = true;
            used_onClose?.Invoke(isEnd);
            tcs?.SetResult(isEnd);
        }
        public bool isReady()
        {
            return rewardAd != null && rewardAd.IsDownloaded;
        }
        /// <summary>
        /// Load the reward Ad.
        /// </summary>
        public void LoadRewardAd()
        {
            if (rewardAd != null) return;
           

            AdHelper.AdNative.LoadRewardVideoAd(
                adSlot, listener);
        }
        /// <summary>
        /// Show the reward Ad.
        /// </summary>
        public void ShowRewardAd()
        {
            if (isReady())
            {
                rewardAd.ShowRewardVideoAd();
            }

        }
        private sealed class RewardVideoAdListener : IRewardVideoAdListener
        {
            private RewardAd reward;
            RewardAdInteractionListener listener;
            int retryCount = 1;
            public RewardVideoAdListener(RewardAd example)
            {
                this.reward = example;
                listener = new RewardAdInteractionListener(this.reward);
            }

            public void OnError(int code, string message)
            {
                Debug.LogError("OnRewardError: " + message);
                reward.Wait(retryCount, () =>
                {
                    reward.LoadRewardAd();
                });
                retryCount <<= 1;
            }

            public void OnRewardVideoAdLoad(RewardVideoAd ad)
            {
                Debug.Log("OnRewardVideoAdLoad");

                ad.SetRewardAdInteractionListener(listener);
                ad.SetDownloadListener(AdHelper.GetDownListener());
                retryCount = 1;
                this.reward.rewardAd = ad;
            }

            public void OnExpressRewardVideoAdLoad(ExpressRewardVideoAd ad)
            {
            }

            public void OnRewardVideoCached()
            {
                Debug.Log("OnRewardVideoCached");

                if (this.reward.rewardAd != null)
                {
                    this.reward.rewardAd.IsDownloaded = true;
                }
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
                this.reward.rewardAd.Dispose();
                this.reward.rewardAd = null;
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
        }
    }
}
