using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
    interface ITTInter : IInterface { }
    public class TTIniter : ITTInter, IInitializable
    {
        public void Initialize()
        {
#if !UNITY_EDITOR
            Init();
#endif
            Debug.Log("INIT_____________________------------------------");
        }
        void Init()
        {
#if UNITY_ANDROID
            AndroidJavaClass init = new AndroidJavaClass("com.unity.ttsdk.AdInit");
            var tp = AScriptableObject.Get<TPPama>();
            init.CallStatic("Init", "mark", ActivityGeter.GetActivity(), tp.appid, "testname", tp.isDebug, tp.enableManualAuthorization);
#endif
        }
    }
}
