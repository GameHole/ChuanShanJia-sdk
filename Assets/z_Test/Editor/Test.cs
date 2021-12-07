using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using MiniGameSDK;

namespace Default
{
	public class Test
	{
        [MenuItem("Assets/Test")]
        static void AAA()
        {
            string m = "123456789";
            //Debug.Log(m.Remove(1, 2));
            Debug.Log(m.IndexOf("2345"));
          
        }
        [PostProcessBuildAttribute(99)]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            if (target == BuildTarget.iOS)
            {
                IOHelper.ComprassDirToRar(path, @"E:\WorkSpace\Webs\APK\2.rar");
            }
        }
    }
}
