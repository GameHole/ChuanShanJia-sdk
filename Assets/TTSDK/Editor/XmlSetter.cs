using System.Collections.Generic;
using System.Xml;
using UnityEngine;
namespace TTSDK
{
	public class XmlSetter
	{
        public static void Set()
        {
            var dc = XmlHelper.GetAndroidManifest();

            dc.SetPermission("android.permission.INTERNET");
            dc.SetPermission("android.permission.ACCESS_NETWORK_STATE");
            var param = AScriptableObject.Get<TPPama>();
            if (param.enableManualAuthorization)
            {
                dc.SetPermission("android.permission.READ_PHONE_STATE");
                dc.SetPermission("android.permission.WRITE_EXTERNAL_STORAGE");
                dc.SetPermission("android.permission.ACCESS_WIFI_STATE");
                dc.SetPermission("android.permission.ACCESS_COARSE_LOCATION");
                dc.SetPermission("android.permission.REQUEST_INSTALL_PACKAGES");
                dc.SetPermission("android.permission.GET_TASKS");
                dc.SetPermission("android.permission.ACCESS_FINE_LOCATION");
                dc.SetPermission("android.permission.WAKE_LOCK");
            }
            else
            {
                dc.RemovePermission("android.permission.READ_PHONE_STATE");
                dc.RemovePermission("android.permission.WRITE_EXTERNAL_STORAGE");
                dc.RemovePermission("android.permission.ACCESS_WIFI_STATE");
                dc.RemovePermission("android.permission.ACCESS_COARSE_LOCATION");
                dc.RemovePermission("android.permission.REQUEST_INSTALL_PACKAGES");
                dc.RemovePermission("android.permission.GET_TASKS");
                dc.RemovePermission("android.permission.ACCESS_FINE_LOCATION");
                dc.RemovePermission("android.permission.WAKE_LOCK");
            }

            if (dc.FindNode("/manifest/application/provider", "android:name", "com.bytedance.sdk.openadsdk.TTFileProvider") == null)
            {
                var app = dc.SelectSingleNode("/manifest/application");
                (app as XmlElement).AppendAttribute("name", "com.bytedance.android.UnionApplication");
                var use = dc.CreateElement("uses-library");
                use.AppendAttribute("name", "org.apache.http.legacy");
                use.AppendAttribute("required", "false");
                app.AppendChild(use);
                var pdr = dc.CreateElement("provider");
                pdr.AppendAttribute("name", "com.bytedance.sdk.openadsdk.TTFileProvider")
                    .AppendAttribute("authorities", "${applicationId}.TTFileProvider")
                    .AppendAttribute("exported", "false")
                    .AppendAttribute("grantUriPermissions", "true");
                var meta = dc.CreateElement("meta-data");
                meta.AppendAttribute("name", "android.support.FILE_PROVIDER_PATHS")
                    .AppendAttribute("resource", "@xml/file_paths");
                pdr.AppendChild(meta);
                app.AppendChild(pdr);

                var mppdr = dc.CreateElement("provider");
                mppdr.AppendAttribute("name", "com.bytedance.sdk.openadsdk.multipro.TTMultiProvider")
                    .AppendAttribute("authorities", "${applicationId}.TTMultiProvider")
                    .AppendAttribute("exported", "false");
                app.AppendChild(mppdr);
            }
            
            dc.Save();
        }
	}
}
