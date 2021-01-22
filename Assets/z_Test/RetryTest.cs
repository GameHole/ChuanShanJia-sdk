using MiniGameSDK;
using System;
using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
	public class RetryTest:MonoBehaviour,IReloader
	{
        IRetryer retryer;

        public int IdCount => 2;

        public Action<bool> onReloaded { get; set ; }

        public int RetryCount => 3;

        int id = 0;
        public void Reload(int id)
        {
            Debug.Log($"reload {id}");
            //if (id++ > 2)
            //    onReloaded?.Invoke(true);
            //else
                onReloaded?.Invoke(false);
        }

        //int id;
        private void Awake()
        {
            retryer.Regist(this);
            
        }
        private void Update()
        {
            //if (Input.GetKeyDown(KeyCode.A))
            //{
            //    Debug.Log("start retry");
            //    retryer.Restart(id);
            //}
            //if (Input.GetKeyDown(KeyCode.S))
            //{
            //    Debug.Log("end retry");
            //    retryer.Clear(id);
            //}
        }
    }
}
