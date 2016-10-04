using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Spate
{
    public sealed class AssetPreloader : BaseBehaviour
    {
        private static AssetPreloader _inst = null;
        private Action<string, float> onProgress;
        private Action onFinish;

        private Dictionary<string, uint> mKeySizeMap;
        private uint mTotalSize;
        private uint mLoadedSize;
        private string mCurrent;

        public static void Start(GameObject go, Action<string, float> _onProgress, Action _onFinish)
        {
            if (_inst == null)
            {
                _inst = go.AddComponent<AssetPreloader>();
                _inst.onProgress = _onProgress;
                _inst.onFinish = _onFinish;
            }
        }

        void Awake()
        {
            if (_inst != null)
            {
                Destroy(this);
                return;
            }
        }

        void Start()
        {
            List<string> listPreloads = GetPreLoads();
            if (mTotalSize == 0f)
            {
                mCurrent = null;
                Finish();
            }
            else
            {
                StartCoroutine(LoadAsync(listPreloads));
            }
        }

        void OnDestroy()
        {
            _inst = null;
        }

        private IEnumerator LoadAsync(List<string> list)
        {
            using (List<string>.Enumerator erator = list.GetEnumerator())
            {
                while (erator.MoveNext())
                {
                    string key = erator.Current;
                    if (key.EndsWith(".cb"))
                    {
                        if (DataHost.source == DataHost.Source.LocalCSV)
                        {
                            OnCsvHandle(key, File.ReadAllBytes(UrlUtil.Combine(Settings.CSV_FOLDER, Path.GetFileNameWithoutExtension(key) + ".csv")));
                            yield return 1;
                        }
                        else
                        {
                            //AssetManager.Load(key, false, OnDataHandle, OnLoadCallback);
                            string path = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, key);
                            if (!File.Exists(path))
                                path = UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, key);
                            WWW www = new WWW(StringUtil.WrapProtocol(path));
                            yield return www;
                            byte[] data = www.bytes;
                            www = null;
                            OnDataHandle(key, data);
                            OnLoadCallback(key);
                        }
                    }
                    else
                    {
                        AssetManager.Load(key, false, null, OnLoadCallback);
                    }
                }
            }
        }

        private List<string> GetPreLoads()
        {
            mLoadedSize = mTotalSize = 0u;

            List<string> listAll = new List<string>(200);
            listAll.AddRange(File.ReadAllLines(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, DataUpdater.LOCAL_FILES_NAME)));
            listAll.AddRange(File.ReadAllLines(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, AssetUpdater.LOCAL_FILES_NAME)));

            List<string> list = new List<string>(50);
            mKeySizeMap = new Dictionary<string, uint>(20);
            for (int i = 0; i < listAll.Count; i++)
            {
                string line = listAll[i];
                if (string.IsNullOrEmpty(line))
                    continue;
                string[] arr = line.Split('|');
                string key = arr[0];
                bool preLoad = string.Equals("1", arr[3]);
                if (preLoad)
                {
                    list.Add(key);
                    uint size = Convert.ToUInt32(arr[2]);
                    mKeySizeMap.Add(key, size);
                    mTotalSize += size;
                }
            }
            list.TrimExcess();
            return list;
        }

        private void OnCsvHandle(string key, byte[] data)
        {
            string text = Encoding.Default.GetString(data);
            string[] allLines = text.Replace("\r\n", "\n").Split('\n');
          //  DataManager.FillCsvData(Path.GetFileNameWithoutExtension(key), allLines);

            OnLoadCallback(key);
        }

        private byte[] OnDataHandle(string key, byte[] data)
        {
            // 解密
            Encrypter.EncodeXor(data);
            // 解压缩
            byte[] newData = GZipUtil.Uncompression(data);
            // 还原成明文
            string text = Encoding.UTF8.GetString(newData);
            string[] allLines = text.Replace("\r\n", "\n").Split('\n');
          //  DataManager.FillCsvData(Path.GetFileNameWithoutExtension(key), allLines);
            return null;
        }

        private void OnLoadCallback(string key)
        {
            mCurrent = key;
            mLoadedSize += mKeySizeMap[key];
            float percent = mLoadedSize * 1.0f / mTotalSize;
            if (percent == 1f)
                Finish();
            else
                onProgress(key, percent);
        }

        private void Finish()
        {
            Destroy(this);
            GCManager.GC();
            onProgress(mCurrent, 1f);
            onFinish();
        }
    }
}
