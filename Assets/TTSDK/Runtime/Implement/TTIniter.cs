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
        IRetryerCtrl ctrl;
        private void callbackmethod(bool success, string message)
        {
            onInited?.Invoke(success ? 0 : -1);
            ctrl.IsRun = true;
            Debug.Log("`````````````````初始化``````" + success + "-----" + message);
        }
        public void Initialize()
        {
            ctrl.IsRun = false;
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
            SDK.RequestPermissionIfNecessary();
            Pangle_Android.InitializeSDK(callbackmethod);
#endif
        }
    }
}
