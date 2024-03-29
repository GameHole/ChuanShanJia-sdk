﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using MiniGameSDK;
namespace TTSDK
{
    //[InitializeOnLoad]
	public class TPHelper:IParamSettng
	{
        //[MenuItem("穿山甲/创建参数")]
        //static void create()
        //{
        //    var tp = AScriptableObject.Get<TPPama>();
        //    if (!tp)
        //    {
        //        var m = AssetHelper.CreateAsset<TPPama>();
        //        Debug.Log($"创建成功 保存至 Resouses/{m.filePath}");
        //        AssetDatabase.Refresh();
        //    }
        //    //GradleHelper.CombineProguard(AssetDatabase.GUIDToAssetPath("6de1926d664a3435e9e357fd75876a60"), "TTSDK");
        //    else
        //    {
        //        Debug.Log($"文件已存在 Resouses/{tp.filePath}");
        //    }
        //}
        ////[MenuItem("穿山甲/应用参数")]
        //public static void apply()
        //{
        //    applyInitFile();
        //    XmlSetter.Set();
        //    AssetDatabase.Refresh();
        //}
        //static void applyInitFile()
        //{
        //    //copyAndReplace("3a19db2af2c49dc4fb0518be7c295493", "Assets/Plugins/Android","java");
        //    //copyAndReplace("aee32d31b23faea45b22e489c6d9a535", "Assets/Plugins/Android/res/xml", "xml");
        //    copyAndReplace("5323257a56ff24f43933c1deb2d335f7", "Assets/Plugins/IOS","mm");
        //}
        //public static void copyAndReplace(string guid,string outPath,string ex)
        //{
        //    var tp = AScriptableObject.Get<TPPama>();
        //    if (tp)
        //    {
        //        string path = outPath;
        //        string file = AssetDatabase.GUIDToAssetPath(guid);
        //        if (File.Exists(file))
        //        {
        //            string fileInit = File.ReadAllText(file);
        //            fileInit = fileInit.Replace("#APPID#", tp.appid);
        //            fileInit = fileInit.Replace("#DEBUG#", tp.isDebug.ToString().ToLower());
        //            fileInit = fileInit.Replace("#AUTHORI#", tp.enableManualAuthorization.ToString().ToLower());
        //            if (!Directory.Exists(path))
        //                Directory.CreateDirectory(path);
        //            File.WriteAllText($"{path}/{Path.GetFileNameWithoutExtension(file)}.{ex}", fileInit);
        //        }
        //    }
        //    else
        //    {
        //        Debug.Log("请先创建广告参数");
        //    }
        //}
        public void SetParam()
        {
#if UNITY_2019_3_OR_NEWER
            PlayerSettings.Android.targetSdkVersion = AndroidSdkVersions.AndroidApiLevel30;
#endif
            AssetHelper.CreateOrGetAsset<TPPama>();
            //XmlSetter.Set();
            var gd = GradleHelper.Open(GradleType.Basic);
            var v = gd.Root.FindValueInDeep("classpath 'com.android.tools.build:gradle:3.4.0'");
            if (v != null)
            {
                v.str = "classpath 'com.android.tools.build:gradle:3.4.3'";
                gd.Save();
            }
            gd = GradleHelper.Open(GradleType.Main);
            gd.Save();
            GooglePlayServices.PlayServicesResolver.Resolve(null,true);
            Google.IOSResolver.AutoInstallCocoapods();
            AssetDatabase.Refresh();
        }
    }
}
