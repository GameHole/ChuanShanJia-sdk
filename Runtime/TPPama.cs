using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
    public class TPPama : AScriptableObject
    {
        public override string filePath => "穿山甲参数";
        public bool isDebug;
        public string appid= "5001121";
        public string[] rewardIds = { "901121430" };
        public string[] intersititialIds = { "901121184" };
        public string[] bannerIds= { "901121246" };
        public string[] splushIds = { "887341406" };
    }
}
