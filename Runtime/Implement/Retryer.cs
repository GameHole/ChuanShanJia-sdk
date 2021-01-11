using System;
using System.Collections.Generic;
using UnityEngine;
namespace TTSDK
{
    public interface IReloader
    {
        int RetryCount { get; }
        int IdCount { get; }
        void Reload(int id);
        Action<bool> onReloaded { get; set; }
    }
    public class Retryer :MonoBehaviour,IRetryer
    {
        List<RetryValue> values = new List<RetryValue>();
        NetworkReachability netState;
        public void Regist(IReloader action)
        {
            var v = new RetryValue() { action = action };
            v.Load();
            values.Add(v);
        }
        //private void Awake()
        //{
        //    netState = Application.internetReachability;
        //}
        private void Start()
        {
            netState = Application.internetReachability;
        }
        private void Update()
        {
            foreach (var item in values)
            {
                if (item.isStart)
                {
                    item.add += Time.deltaTime;
                    if (item.add >= item.time)
                    {
                        item.add = 0;
                        item.isStart = false;
                        item.Load();
                    }
                }
            }
            if (Application.internetReachability != netState)
            {
                Debug.Log($"net change from {netState} to {Application.internetReachability}");
                if (netState == NetworkReachability.NotReachable)
                {
                    foreach (var item in values)
                    {
                        if (!item.isLoaded)
                        {
                            item.Load();
                            item.isLoaded = true;
                        }
                    }
                }
                netState = Application.internetReachability;
            }
        }

        public void Load(IReloader action)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].action == action)
                {
                    values[i].Load();
                    break;
                }
            }
        }

        public class RetryValue
        {
            public bool isLoaded = true;
            public float add;
            public float time;
            public bool isStart;
            int _id;
            int retryCount;
            internal int loadId
            {
                get
                {
                    return _id++ % action.IdCount;
                }
            }
            internal IReloader action;
            public void Load()
            {
                if (action.onReloaded == null)
                {
                    action.onReloaded = (v) =>
                    {
                        isLoaded = v;
                        if (!v && retryCount < action.RetryCount)
                        {
                            Restart();
                        }
                        else
                        {
                            Clear();
                        }
                    };
                }
                action.Reload(loadId);
            }
            void Restart()
            {
                retryCount++;
                add = 0;
                time += 1;
                isStart = true;
            }
            void Clear()
            {
                retryCount = 0;
                add = 0;
                time = 0;
                isStart = false;
            }
        }
    }
}
