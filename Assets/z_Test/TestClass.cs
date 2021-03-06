﻿using System.Collections.Generic;
using UnityEngine;
namespace MiniGameSDK
{
	public class TestClass:MonoBehaviour
	{
        IAdIniter initer;
        ISplashAd splash;
        IInterstitialAdAPI interstitialAd;
        IBannerAd banner;
        IRewardAdAPI rewardAd;
        private void Awake()
        {
            splash.OnClsoe = () => Debug.Log("TestClass::splsh as closed");
            splash.Show();
            initer.onInited += (v) =>
            {
                Debug.Log("initer.onInited--------------");
            };
        }
        public void ShowFull()
        {
            interstitialAd.Show();
        }
        public async void ShowRw()
        {
            Debug.Log(await rewardAd.AutoShowAsync());
        }
        public  void ShowRw1()
        {
            rewardAd.AutoShow((v) =>
            {
                Debug.Log(v);
            });
        }
        public void Show()
        {
            banner.Show();
        }
        public void Hide()
        {
            banner.Hide();
        }
    }
}
