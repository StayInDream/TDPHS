using System;
using System.Collections;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spate
{
    public sealed class AssetLoader
    {
        public AssetLoaderState State { get; private set; }
        public bool IsSuccess { get; private set; }
        public Object mainAsset { get; private set; }
        public string Error { get; private set; }
        public AssetRequest Request { get; private set; }

        public void DoWork(AssetRequest req)
        {
            if (req == null)
                throw new ArgumentNullException("req can not be null.");
            Free();
            Request = req;
            State = AssetLoaderState.Work;
            AsyncManager.StartCoroutine(LoadCore());
            LoadCore();
        }

        public void Free()
        {
            if (State == AssetLoaderState.Work)
                AsyncManager.StopCoroutine(LoadCore());
            State = AssetLoaderState.Idle;
            IsSuccess = false;
            mainAsset = null;
            Error = null;
            Request = null;
        }

        private IEnumerator LoadCore()
        {
            yield return 1;
            string path = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, Request.key);
            if (!File.Exists(path))
                path = UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, Request.key);
            // 创建AssetBundlec
            WWW www = new WWW(StringUtil.WrapProtocol(path));
            yield return www;
            if (www.isDone && string.IsNullOrEmpty(www.error))
            {
                // 获取mainAsset
                AssetBundle bundle = www.assetBundle;
                mainAsset = bundle.mainAsset;
                bundle.Unload(false);
                State = AssetLoaderState.Done;
                IsSuccess = true;
                Error = null;
            }
            else
            {
                State = AssetLoaderState.Done;
                IsSuccess = false;
                mainAsset = null;
                Error = string.Format("获取{0} Error:{1}", Request.key, www.error);
            }
            www = null;
        }
    }
}
