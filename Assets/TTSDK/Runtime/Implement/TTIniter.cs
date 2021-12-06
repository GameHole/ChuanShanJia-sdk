using ByteDance.Union;
using MiniGameSDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace TTSDK
{
    public class TTIniter : IAdIniter, IInitializable,IMenualInitor
    {
        public event Action<int> onInited;
        bool isInited;
        private void callbackmethod(bool success, string message)
        {
            onInited?.Invoke(success ? 1 : 0);
            Debug.Log("`````````````````初始化``````" + success + "-----" + message);
        }
        public void Initialize()
        {
            var set = AScriptableObject.Get<TPPama>();
            if (!set.isLateInit)
            {
                Init();
            }
        }

        public void Init()
        {
            if (isInited) return;
            isInited = true;
#if UNITY_IOS
            var set = AScriptableObject.Get<TPPama>();
            PangleConfiguration configuration = PangleConfiguration.CreateInstance();
            configuration.appID = set.appid;
            Pangle.InitializeSDK(callbackmethod);
#endif
#if UNITY_ANDROID
            Pangle_Android.InitializeSDK(callbackmethod);
#endif
        }
    }
}
