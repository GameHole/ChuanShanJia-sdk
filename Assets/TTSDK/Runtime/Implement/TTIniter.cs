using MiniGameSDK;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace TTSDK
{
    public class TTIniterCalBack : AndroidJavaProxy
    {
       public Action<int> back;
        public TTIniterCalBack() : base("com.unity.ttsdk.IInitOver")
        {

        }
        public void OnOver()
        {
            back?.Invoke(0);
        }
    }
    public class TTIniter : IAdIniter, IInitializable
    {
        public event Action<int> onInited;

        public void Initialize()
        {
#if !UNITY_EDITOR
            Init();
#endif
            //Debug.Log("INIT_____________________------------------------");
        }
        void Init()
        {
#if UNITY_ANDROID
            AndroidJavaClass init = new AndroidJavaClass("com.unity.ttsdk.AdInit");
            var tp = AScriptableObject.Get<TPPama>();
            init.CallStatic("Init", "mark", ActivityGeter.GetActivity(), tp.appid, "testname", tp.isDebug, tp.enableManualAuthorization, new TTIniterCalBack
            {
                back = (v) =>
                {
                    onInited?.Invoke(v);
                }
            });
#endif
        }
        
    }
}
