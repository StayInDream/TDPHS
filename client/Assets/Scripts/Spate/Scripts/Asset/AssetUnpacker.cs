
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spate
{

    public class AssetUnpacker : BaseBehaviour
    {
        private const string KEY = "Supreme_Cfg_AssetUnpacked";

        private static AssetUnpacker _inst = null;

        private Action onFinish;
        private Action<string, float> onProgress;

        private bool mFinished;
        //private string mSourceFolder;

        private uint mTotalSize;
        private uint mUnpackedSize;

        public static void Start(GameObject go, Action<string, float> onProgress, Action onFinish)
        {
            if (_inst == null)
            {
                _inst = go.AddComponent<AssetUnpacker>();
                _inst.onFinish = onFinish;
                _inst.onProgress = onProgress;
            }
        }

        void Awake()
        {
            if (_inst != null)
            {
                Finish();
                return;
            }
        }

        void Start()
        {
            // 如果已经Unpack过就不需要再Unpack
            if (PlayerPrefs.HasKey(KEY))
            {
                MLogger.Log("Unpacked,exit Unpack");
                Finish();
            }
            else
            {
                StartCoroutine(UnpackFilesAsync(() =>
                {
                    // 仅android平台可用
                    if (RuntimePlatform.Android == Application.platform)
                    {
                        mTotalSize = mUnpackedSize = 0u;
                        NotifyProgress(null);

                        StartCoroutine(UnpackAsync());
                    }
                    else
                    {
                        Finish();
                    }
                }));
            }
        }

        void OnDestroy()
        {
            _inst = null;
            onFinish();
        }

        private void Finish()
        {
            PlayerPrefs.SetInt(KEY, 1);
            PlayerPrefs.Save();
            Destroy(this);
        }

        private IEnumerator UnpackFilesAsync(Action callback)
        {
            // unpack data-files
            WWW www = new WWW(StringUtil.WrapProtocol(UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, DataUpdater.LOCAL_FILES_NAME)));
            yield return www;
            byte[] buffer = www.bytes;
            File.WriteAllBytes(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, DataUpdater.LOCAL_FILES_NAME), buffer);
            www = null;

            www = new WWW(StringUtil.WrapProtocol(UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, AssetUpdater.LOCAL_FILES_NAME)));
            yield return www;
            buffer = www.bytes;
            File.WriteAllBytes(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, AssetUpdater.LOCAL_FILES_NAME), buffer);
            www = null;

            callback();
        }

        private IEnumerator UnpackAsync()
        {
            List<string> listLines = new List<string>(200);
            // 第一步，尝试获取data-files
            string dataFiles = File.ReadAllText(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, DataUpdater.LOCAL_FILES_NAME));
            listLines.AddRange(dataFiles.Replace("\r\n", "\n").Split('\n'));
            // 第一步，尝试获取asset-files
            string assetFiles = File.ReadAllText(UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, AssetUpdater.LOCAL_FILES_NAME)); ;
            listLines.AddRange(assetFiles.Replace("\r\n", "\n").Split('\n'));
            // 分析并逐个更新
            List<UnpackItem> list = new List<UnpackItem>(listLines.Count);
            foreach (string line in listLines)
            {
                string[] cells = line.Split('|');
                string shortName = cells[0];
                // 只unpack出effect、UIEffect
                if (shortName.StartsWith("effect/", StringComparison.OrdinalIgnoreCase) ||
                    shortName.StartsWith("uiEffect/", StringComparison.OrdinalIgnoreCase))
                {
                    string md5Hash = cells[1];
                    uint size = Convert.ToUInt32(cells[2]);
                    list.Add(new UnpackItem(shortName, md5Hash, size));
                    mTotalSize += size;
                }
            }


            if (list.Count > 0)
            {
                // 逐个解压缩
                for (int i = 0; i != list.Count; i++)
                {
                    yield return StartCoroutine(UnpackFile(list[i]));
                }
            }
            Finish();
        }

        private IEnumerator UnpackFile(UnpackItem item)
        {
            string srcUrl = UrlUtil.Combine(Settings.UNITY_RAW_READONLY_FOLDER, item.shortName);
            string dstUrl = UrlUtil.Combine(Settings.UNITY_RAW_FOLDER, item.shortName);
            FileUtil.EnsureFileParent(dstUrl);

            WWW www = new WWW(StringUtil.WrapProtocol(srcUrl));
            yield return www;
            uint size = 0u;
            if (string.IsNullOrEmpty(www.error))
            {
                size = (uint)www.bytes.Length;
                File.WriteAllBytes(dstUrl, www.bytes);
            }
            else
            {
                MLogger.LogError("AssertUnpacker-->UnpackFile Error:" + www.error);
            }
            www = null;

            mUnpackedSize += size;
            NotifyProgress(item.shortName);
        }

        private struct UnpackItem
        {
            public readonly string shortName;
            public readonly string hash;
            public readonly uint size;

            public UnpackItem(string name, string md5, uint s)
            {
                shortName = name;
                hash = md5;
                size = s;
            }
        }

        private void NotifyProgress(string name)
        {
            if (onProgress != null)
            {
                float percent = 0f;
                if (mTotalSize > 0f)
                    percent = mUnpackedSize * 1.0f / mTotalSize;
                onProgress(name, percent);
            }
        }
    }
}