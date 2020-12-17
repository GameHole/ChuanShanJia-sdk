using System.Collections.Generic;
using UnityEngine;
namespace MiniGameSDK
{
	public class TestClass:MonoBehaviour
	{
        ISplashAd splash;
        IInterstitialAdAPI interstitialAd;
        IBannerAd banner;
        private void Awake()
        {
            splash.OnClsoe = () => Debug.Log("TestClass::splsh as closed");
            splash.Show();
        }
        public void ShowFull()
        {
            interstitialAd.Show();
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
