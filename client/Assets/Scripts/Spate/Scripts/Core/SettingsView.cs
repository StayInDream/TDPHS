using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

namespace Spate
{
    public sealed class SettingsView : MonoBehaviour
    {
        /// <summary>
        /// 显示Settings界面,等待界面关闭的回调
        /// </summary>
        public static void ShowView(GameObject go, Action callback)
        {
            if (Settings.Debug)
            {
                SettingsView inst = go.GetComponent<SettingsView>();
                if (inst == null)
                    inst = go.AddComponent<SettingsView>();
                inst.mCallback = callback;
            }
            else
            {
                NetHost.SetDefault();
                DataHost.source = DataHost.Source.FromNetHost;
                ResHost.source = ResHost.Source.FromNetHost;

                Settings.ShowDebugInfo = false;
                Settings.IsKipGuide = false;
                //Settings.ResCheckable = true;

                callback();
            }
        }

        private const string KEY_CONFIG_SHOWDEBUG = "Settings_Config_ShowDebug";
        private const string KEY_CONFIG_SKIPGUIDE = "Settings_Config_SkipGuide";
        private const string KEY_CONFIG_CHECKRES = "Settings_Config_CHECKRES";

        private Vector2 mScrollPosition;
        private Action mCallback;

        private SettingsGroup mNetSettings;
        private SettingsGroup mDataSettings;
        private SettingsGroup mAssetSettings;

        void Start()
        {
            List<SettingsPair> list = new List<SettingsPair>();

            list.Clear();
            list.Add(new SettingsPair("内网", "http://192.168.1.110:9000", true, true));
            list.Add(new SettingsPair("外网测试", "http://120.25.227.65:9003", true, true));
            list.Add(new SettingsPair("外网版署", "http://120.25.227.65:9000", true, true));
            list.Add(new SettingsPair("外网", NetHost.HOST_DEFAULT, true, true));
            mNetSettings = new SettingsGroup("游戏逻辑", list);


            list.Clear();
            list.Add(new SettingsPair("自动", "auto://", true, false));
#if UNITY_EDITOR
            list.Add(new SettingsPair("本地CSV", "csv://", true, false));
#endif
            list.Add(new SettingsPair("内网", "http://192.168.1.200:888/svn/supreme_hfs_data/trunk/0", true, true));
            list.Add(new SettingsPair("外网", "http://wsres.you2game.com:888/svn/supreme_hfs_data/trunk/0", true, true));
            mDataSettings = new SettingsGroup("数据资源", list);


            list.Clear();
            list.Add(new SettingsPair("自动", "auto://", true, false));
#if UNITY_EDITOR
            list.Add(new SettingsPair("本地EDITOR", "editor://", true, false));
#endif
            list.Add(new SettingsPair("内网", "http://192.168.1.200:888/svn/supreme_hfs_asset/trunk/0", true, true));
#if !UNITY_EDITOR
            list.Add(new SettingsPair("外网", "http://wsres.you2game.com:888/svn/supreme_hfs_asset/trunk/0", true, true));
#endif
            mAssetSettings = new SettingsGroup("美术资源", list);

            // ShowDebug
            Settings.ShowDebugInfo = (PlayerPrefs.GetInt(KEY_CONFIG_SHOWDEBUG, 0) == 1);
            Settings.IsKipGuide = (PlayerPrefs.GetInt(KEY_CONFIG_SKIPGUIDE, 0) == 1);
            //Settings.ResCheckable = (PlayerPrefs.GetInt(KEY_CONFIG_CHECKRES, 0) == 1);
        }

        /// <summary>
        /// 绘制出配置界面
        /// </summary>
        void OnGUI()
        {
            mScrollPosition = GUILayout.BeginScrollView(mScrollPosition);

            GUILayout.Space(20);
            GUILayout.BeginHorizontal();
            GUILayout.Space(30);
            GUILayout.BeginVertical(GUILayout.Width(Screen.width - 30), GUILayout.Height(Screen.height - 20));

            mNetSettings.OnGUI();
            GUILayout.Space(20);
            mDataSettings.OnGUI();
            GUILayout.Space(20);
            mAssetSettings.OnGUI();
            GUILayout.Space(20);
            OnGUI_Other();
            GUILayout.Space(20);
            OnGUI_Control();

            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.EndScrollView();
        }


        void OnDestroy()
        {
            // 获取数据
            NetHost.SetUrl(mNetSettings.SelectedValue);
            ///////
            {
                string data = mDataSettings.SelectedValue;
                if ("csv://".Equals(data))
                {
                    DataHost.source = DataHost.Source.LocalCSV;
                }
                else if ("auto://".Equals(data))
                {
                    DataHost.source = DataHost.Source.FromNetHost;
                }
                else
                {
                    DataHost.source = DataHost.Source.CustomUrl;
                    DataHost.SetUrl(data);
                    DataHost.BuildUrl();
                }
            }
            ///////
            {
                string asset = mAssetSettings.SelectedValue;
                if ("editor://".Equals(asset))
                {
                    ResHost.source = ResHost.Source.EditorAsset;
                }
                else if ("auto://".Equals(asset))
                {
                    ResHost.source = ResHost.Source.FromNetHost;
                }
                else
                {
                    ResHost.source = ResHost.Source.CustomUrl;
                    ResHost.SetUrl(asset);
                    ResHost.BuildUrl();
                }
            }

            if (mCallback != null)
                mCallback();
        }

        private void OnGUI_Other()
        {
            GUILayout.BeginHorizontal();

            // 显示调试信息
            GUI.changed = false;
            Settings.ShowDebugInfo = GUILayout.Toggle(Settings.ShowDebugInfo, "显示调试信息", GUILayout.Width(120), GUILayout.Height(60));
            if (GUI.changed)
            {
                PlayerPrefs.SetInt(KEY_CONFIG_SHOWDEBUG, Settings.ShowDebugInfo ? 1 : 0);
                PlayerPrefs.Save();
            }

            GUILayout.Space(10);
            // 新手引导
            GUI.changed = false;
            Settings.IsKipGuide = GUILayout.Toggle(Settings.IsKipGuide, "跳过新手引导", GUILayout.Width(120), GUILayout.Height(60));
            if (GUI.changed)
            {
                PlayerPrefs.SetInt(KEY_CONFIG_SKIPGUIDE, Settings.IsKipGuide ? 1 : 0);
                PlayerPrefs.Save();
            }
            GUILayout.Space(10);
            // 资源检测
            //GUI.changed = false;
            //Settings.ResCheckable = GUILayout.Toggle(Settings.ResCheckable, "检测资源更新", GUILayout.Width(120), GUILayout.Height(60));
            //if (GUI.changed)
            //{
            //    PlayerPrefs.SetInt(KEY_CONFIG_CHECKRES, Settings.ResCheckable ? 1 : 0);
            //    PlayerPrefs.Save();
            //}
            GUILayout.EndHorizontal();
        }


        private void OnGUI_Control()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("清理", GUILayout.Width(120), GUILayout.Height(80)))
            {
                // 清理自定义数据
                mNetSettings.ClearData();
                mDataSettings.ClearData();
                mAssetSettings.ClearData();
                PlayerPrefs.Save();
                // 重新加载数据
                Start();
            }

            GUILayout.Space(100);

            if (GUILayout.Button("完成", GUILayout.Width(120), GUILayout.Height(80)))
            {
                Destroy(this);
            }
            GUILayout.EndHorizontal();
        }
    }
}