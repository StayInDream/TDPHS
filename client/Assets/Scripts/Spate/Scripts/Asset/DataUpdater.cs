using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spate;


namespace Spate
{
    /// <summary>
    /// 数据更新器
    /// </summary>
    public sealed class DataUpdater : BaseBehaviour
    {
        private static DataUpdater _inst = null;

        private Step mStep;
        private Step mPreStep;
        private Step mCurStep;
        private AssetDownloader mDownloader;

        private string mServerHash;
        private string mServerFiles;
        private uint mTotalUpdateSize;
        private uint mUpdatedSize;
        private Queue<string> mUpdateQueue;//待更新的队列


        private string mRetry;
        private string mCurrent;
        private bool mPause;

        private Action<string, float> onProgress;
        private Action<bool> onFinish;

        private Dictionary<string, AssetCfg> mLocalCfgMap = null;

        private bool mHasCodeUpdated = false;

        public const string LOCAL_FILES_NAME = "data-files";

        public static void Start(GameObject go, Action<string, float> _onProgress, Action<bool> _finish)
        {
            if (_inst == null)
            {
                _inst = go.AddComponent<DataUpdater>();
                _inst.onProgress = _onProgress;
                _inst.onFinish = _finish;
            }
        }

        private void Retry(string url, string userdata)
        {
            mPause = false;
            mStep = _inst.mCurStep;
            // XXX: 如果在下载hashmaphash或hashmap的过程中发生超时并且最终真的有更新时,如果这里不处理,那么在update->update中会有hash项出现从而导致空指针
            mRetry = null;
        }

        void Awake()
        {
            if (_inst != null)
            {
                MLogger.LogError("重复挂载AssetUpdater");
                Destroy(this);
                return;
            }
        }

        void Start()
        {
            mDownloader = new AssetDownloader();
            mPreStep = Step.None;
            mStep = Step.HashmapHash;

            // 读取本地的配置列表
            string localFiles = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, LOCAL_FILES_NAME);
            if (File.Exists(localFiles))
            {
                string[] array = File.ReadAllLines(localFiles);
                mLocalCfgMap = new Dictionary<string, AssetCfg>(array.Length);
                for (int i = 0; i < array.Length; i++)
                {
                    string line = array[i];
                    if (string.IsNullOrEmpty(line))
                        continue;
                    string[] cells = line.Split('|');
                    if (cells == null || cells.Length == 0)
                        continue;
                    AssetCfg cfg = new AssetCfg(cells);
                    mLocalCfgMap.Add(cfg.key, cfg);
                }
            }
            else
            {
                mLocalCfgMap = new Dictionary<string, AssetCfg>();
            }
        }

        void OnDestroy()
        {
            _inst = null;
        }

        void Update()
        {
            if (mPause)
                return;
            switch (mStep)
            {
                case Step.HashmapHash:
                    {
                        // 获取服务器上的hashmaphash
                        mPreStep = mStep;
                        mStep = Step.Wait;
                        mCurStep = Step.HashmapHash;
                        mDownloader.DoWork(GetServerUrl("hash"), "hash");
                    }
                    break;
                case Step.Hashmap:
                    {
                        // 获取服务器上的hashmap
                        mPreStep = mStep;
                        mStep = Step.Wait;
                        mCurStep = Step.Hashmap;
                        mDownloader.DoWork(GetServerUrl("files"), "files");
                    }
                    break;
                case Step.Update:
                    {
                        mCurStep = Step.Update;
                        // 从服务器上下载item
                        mPreStep = mStep;
                        mStep = Step.Wait;
                        mCurrent = null;
                        if (mRetry != null)
                        {
                            mCurrent = mRetry;
                            mRetry = null;
                        }
                        else
                        {
                            mCurrent = mUpdateQueue.Dequeue();
                        }
                        mDownloader.OnProgress = OnItemProgress;
                        mDownloader.DoWork(GetServerUrl(mCurrent), mCurrent);
                    }
                    break;
                case Step.Patch:
                    {
                        mPreStep = Step.None;
                        mStep = Step.None;
                        AsyncManager.StartThread(PatchHandleAsync, null);
                    }
                    break;
                case Step.Finish:
                    {
                        mStep = Step.None;
                        OnFinish();
                    }
                    break;
                case Step.Wait:
                    {
                        switch (mDownloader.State)
                        {
                            case AssetDownloaderState.Error:
                                OnError(mDownloader.Url, mDownloader.Error, mDownloader.UserData);
                                mDownloader.Free();
                                break;
                            case AssetDownloaderState.Timeout:
                                OnTimeout(mDownloader.Url, mDownloader.UserData);
                                mDownloader.Free();
                                break;
                            case AssetDownloaderState.LossData:
                                OnLossData(mDownloader.Url, mDownloader.UserData);
                                mDownloader.Free();
                                break;
                            case AssetDownloaderState.Succeed:
                                OnSucceed(mDownloader.Url, mDownloader.Data, mDownloader.UserData);
                                mDownloader.Free();
                                break;
                        }
                    }
                    break;
            }
        }

        // 如果没有需要更新的项则需要调用该方法
        private void SendLocalMapToAssetManager()
        {
            if (mLocalCfgMap == null)
                throw new Exception("仅在无更新的情况下才执行此操作");
            AssetManager.AddCfgs(mLocalCfgMap);
        }

        private void OnItemProgress(int number)
        {
            float percent = (mUpdatedSize + number) * 1.0f / mTotalUpdateSize;
            onProgress(mCurrent, percent);
        }

        private void Finish()
        {
            mStep = Step.Finish;
        }

        private void OnFinish()
        {
            Destroy(this);
            onProgress(mCurrent, 1f);
            onFinish(mHasCodeUpdated);
        }

        private string GetServerUrl(string key)
        {
            return UrlUtil.Combine(DataHost.Url, key);
        }

        // code写入到persistentDataPath中便于程序卸载时自动删除,否则如果安装了同名包的app，code会错乱
        private string GetRawPath(string key, bool ensure)
        {
            string path = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, key);
            if (ensure)
                FileUtil.EnsureFileParent(path);
            return path;
        }

        private void OnError(string url, string error, object userData)
        {
            mRetry = userData.ToString();
            mPause = true;
            mStep = mPreStep;
            Notifier.Notify<Action<string, string>, string, string>(AssetDownloadCode.Error, Retry, url, error);
        }

        private void OnTimeout(string url, object userData)
        {
            mRetry = userData.ToString();
            mPause = true;
            mStep = mPreStep;
            Notifier.Notify<Action<string, string>, string, string>(AssetDownloadCode.Timeout, Retry, url, "");
        }

        private void OnLossData(string url, object userData)
        {
            mRetry = userData.ToString();
            mPause = true;
            mStep = mPreStep;
            Notifier.Notify<Action<string, string>, string, string>(AssetDownloadCode.LossData, Retry, url, "");
        }

        private void OnSucceed(string url, byte[] raw, object userData)
        {
            switch (mPreStep)
            {
                case Step.HashmapHash:
                    {
                        // 和本地的HashmapHash进行对比
                        mServerHash = new UTF8Encoding(false).GetString(raw);
                        string local_hashmaphash = GetLocalHashmapHash();
                        if (string.Equals(local_hashmaphash, mServerHash))
                        {
                            SendLocalMapToAssetManager();
                            Finish();
                        }
                        else
                            mStep = Step.Hashmap;
                    }
                    break;
                case Step.Hashmap:
                    {
                        mServerFiles = new UTF8Encoding(false).GetString(raw);
                        string[] array = mServerFiles.Replace("\r\n", "\n").Split('\n');
                        mUpdateQueue = new Queue<string>(array.Length);// 筛选出待更新的项
                        mTotalUpdateSize = 0u;
                        List<string> codes = new List<string>();
                        for (int i = 0; i < array.Length; i++)
                        {
                            string line = array[i];
                            if (string.IsNullOrEmpty(line))
                                continue;
                            string[] cells = line.Split('|');
                            if (cells == null || cells.Length == 0)
                                continue;
                            string key = cells[0];

                            AssetCfg item = new AssetCfg(cells, GetLocalItemHash(key));
                            if (item.isCode)
                                codes.Add(key);
                            // AssetManager使用服务器上的item
                            AssetManager.AddCfg(item);
                            if (item.needUpdate)
                            {
                                mUpdateQueue.Enqueue(key);
                                mTotalUpdateSize += item.size;
                                // 检测是否有code更新
                                if (!mHasCodeUpdated && item.isCode)
                                    mHasCodeUpdated = true;
                            }
                        }

                        // 同步本地的codes,删除无效的dll
                        {
                            string dir = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, "code");
                            if (Directory.Exists(dir))
                            {
                                foreach (string f in Directory.GetFiles(dir))
                                {
                                    if (!codes.Contains(Path.GetFileName(f)))
                                    {
                                        // 删除
                                        File.Delete(f);
                                    }
                                }
                            }
                        }

                        mUpdateQueue.TrimExcess();
                        // 选择Patch或更新
                        if (mUpdateQueue.Count == 0)
                        {
                            //SendLocalMapToAssetManager();
                            mStep = Step.Patch;
                            onProgress(null, 1f);
                        }
                        else
                        {
                            mStep = Step.Update;
                            mUpdatedSize = 0u;
                            onProgress(null, 0f);
                        }
                    }
                    break;
                case Step.Update:
                    {
                        // 写入到Raw中
                        string key = userData.ToString();
                        string rawPath = GetRawPath(key, true);
                        File.WriteAllBytes(rawPath, raw);
                        // 检查队列
                        if (mUpdateQueue.Count > 0)
                            mStep = Step.Update;
                        else
                            mStep = Step.Patch;
                        // 更新进度
                        AssetCfg item = AssetManager.GetCfg(key);
                        mUpdatedSize += item.size;
                        float percent = (mUpdatedSize * 1.0f / mTotalUpdateSize);
                        onProgress(key, percent);
                    }
                    break;
            }
        }

        private string GetLocalHashmapHash()
        {
            // 获取本地的files
            string localFilesPath = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, LOCAL_FILES_NAME);
            if (!File.Exists(localFilesPath))
                localFilesPath = UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, LOCAL_FILES_NAME);
            if (!File.Exists(localFilesPath))
                return string.Empty;
            return Hasher.CalcFileHash(localFilesPath);
        }

        private string GetLocalItemHash(string key)
        {
            AssetCfg cfg = null;
            mLocalCfgMap.TryGetValue(key, out cfg);
            if (cfg == null)
                return string.Empty;
            // 对于code需要每次都检测
            if (key.StartsWith("code/"))
            {
                string localFilesPath = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, key);
                if (!File.Exists(localFilesPath))
                    return string.Empty;
                return Hasher.CalcFileHash(localFilesPath);
            }
            return cfg.hash;
        }

        private object PatchHandleAsync(object args)
        {
            // 写files
            string path = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, LOCAL_FILES_NAME);
            File.WriteAllBytes(path, new UTF8Encoding(false).GetBytes(mServerFiles));
            GC.Collect();
            // 完成Patch
            Finish();
            return args;
        }

        private enum Step
        {
            None,
            Wait,
            HashmapHash,
            Hashmap,
            Update,
            Patch,
            Finish
        }
    }
}