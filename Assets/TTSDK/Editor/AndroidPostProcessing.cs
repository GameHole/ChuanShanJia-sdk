using UnityEditor;
using UnityEditor.Android;
using UnityEditor.Build.Reporting;
using UnityEngine;
namespace TTSDK
{
    public class AndroidPostProcessing: IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 0;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log(path);
            var xml = $"{path}/src/main/AndroidManifest.xml";
            var dc = XmlHelper.Open(xml);
            XmlSetter.SetValues(dc);
            dc.Save(xml);
            GradleHelper.CombineProguard(AssetDatabase.GUIDToAssetPath("6de1926d664a3435e9e357fd75876a60"), "TTSDK", path);
            TPHelper.copyAndReplace("aee32d31b23faea45b22e489c6d9a535", $"{path}/src/main/res/xml", "xml");
        }
    }
}
