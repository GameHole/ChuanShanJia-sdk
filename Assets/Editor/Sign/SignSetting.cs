using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace MiniGameSDK
{
    [InitializeOnLoad]
    public class SignSetting
	{
        static SignSetting()
        {
            PlayerSettings.Android.useCustomKeystore = true;
            PlayerSettings.Android.keystoreName = "Assets/Editor/Sign/user.keystore";
            PlayerSettings.Android.keystorePass = "aaaaaaaa";
            PlayerSettings.Android.keyaliasName = "aaaaaaaa";
            PlayerSettings.Android.keyaliasPass = "aaaaaaaa";
        }
    }
}
