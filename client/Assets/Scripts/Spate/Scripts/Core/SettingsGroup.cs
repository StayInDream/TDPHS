using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Spate
{
    public class SettingsGroup
    {
        public string Name { get; private set; }

        private readonly string KEY_CONFIG;

        public SettingsGroup(string name, IList<SettingsPair> defCfgs)
        {
            Name = name;
            KEY_CONFIG = "SettingsGroup_" + name;

            if (defCfgs != null)
            {
                mConfigs.AddRange(defCfgs);
            }
            LoadData();
        }

        public void OnGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(100), GUILayout.Height(80));
            GUILayout.Space(40);
            GUILayout.Label(Name + ":");
            GUILayout.EndVertical();
            GUI.changed = false;
            SelectedIndex = GUILayout.SelectionGrid(SelectedIndex, mAllKeys, 6, GUILayout.Height(80));
            if (GUI.changed)
            {
                UpdateTempKeyAndValue();
                SaveData();
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(20);

            // 显示"添加"功能
            GUILayout.BeginHorizontal();
            if (CheckCurrentConfigCanShow())
            {
                GUILayout.BeginVertical(GUILayout.Width(500), GUILayout.Height(80));
                GUILayout.Space(30);
                GUILayout.BeginHorizontal();
                GUILayout.Label("备注:", GUILayout.Width(40));
                mTempKey = GUILayout.TextField(mTempKey, GUILayout.Width(120), GUILayout.Height(40));
                GUILayout.Space(10);
                GUILayout.Label("地址:", GUILayout.Width(40));
                mTempValue = GUILayout.TextField(mTempValue, GUILayout.Width(300), GUILayout.Height(40));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();

                GUILayout.Space(20);
                if (GUILayout.Button("添加", GUILayout.Width(120), GUILayout.Height(80)))
                {
                    if (!string.IsNullOrEmpty(mTempKey) && !string.IsNullOrEmpty(mTempValue)
                        && GetConfigCountByName(mTempKey) == 0)
                    {
                        // 添加
                        mConfigs.Add(new SettingsPair(mTempKey, mTempValue, false, true));
                        UpdateAllKeys();
                        SelectedIndex = mConfigs.Count - 1;
                        SaveData();
                        UpdateTempKeyAndValue();
                    }
                }
                if (CheckCurrentConfigCanModify())
                {
                    GUILayout.Space(20);
                    if (GUILayout.Button("修改", GUILayout.Width(120), GUILayout.Height(80)))
                    {
                        if (!string.IsNullOrEmpty(mTempKey) && !string.IsNullOrEmpty(mTempValue)
                            && GetConfigCountByName(mTempKey) <= 1)
                        {
                            // 修改
                            mConfigs[SelectedIndex].SetValue(mTempValue);
                            SaveData();
                            UpdateTempKeyAndValue();
                        }
                    }
                }
            }
            if (CheckCurrentConfigCanModify())
            {
                GUILayout.Space(20);
                if (GUILayout.Button("删除", GUILayout.Width(120), GUILayout.Height(80)))
                {
                    // 删除
                    mConfigs.RemoveAt(SelectedIndex);
                    UpdateAllKeys();
                    SelectedIndex -= 1;
                    SaveData();
                    UpdateTempKeyAndValue();
                }
            }
            GUILayout.EndHorizontal();
        }

        // 读取数据
        private void LoadData()
        {
            try
            {
                SelectedIndex = 0;

                string prefs = PlayerPrefsUtil.GetLongString(KEY_CONFIG);
                if (prefs != null)
                {
                    int leftBracket = prefs.IndexOf('[');
                    SelectedIndex = int.Parse(prefs.Substring(0, leftBracket));
                    string[] listPrefs = prefs.Substring(leftBracket + 1).Replace("]", "").Split(';');
                    for (int i = 0; i < listPrefs.Length; i++)
                    {
                        string[] itemArray = listPrefs[i].Split('=');
                        if (itemArray.Length == 2)
                            mConfigs.Add(new SettingsPair(itemArray[0], itemArray[1]));
                    }
                }
            }
            catch
            {
                if (mConfigs.Count == 0)
                    SelectedIndex = -1;
                else
                    SelectedIndex = 0;
            }

            UpdateAllKeys();
            UpdateTempKeyAndValue();
        }
        // 保存数据
        private void SaveData()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(SelectedIndex);
            sb.Append("[");
            for (int i = 0; i < mConfigs.Count; i++)
            {
                if (mConfigs[i].IsFixed)
                    continue;
                sb.AppendFormat("{0}={1}", mConfigs[i].Key, mConfigs[i].Value);
                if (i != (mConfigs.Count - 1))
                    sb.Append(";");
            }
            sb.Append("]");
            PlayerPrefsUtil.SetLongString(KEY_CONFIG, sb.ToString());
        }
        public void ClearData()
        {
            PlayerPrefs.DeleteKey(KEY_CONFIG);
        }

        private bool CheckCurrentConfigCanModify()
        {
            return !mConfigs[SelectedIndex].IsFixed;
        }

        private bool CheckCurrentConfigCanShow()
        {
            return mConfigs[SelectedIndex].ShowKeyAndValue;
        }

        private int GetConfigCountByName(string key)
        {
            int num = 0;
            for (int i = 0; i < mConfigs.Count; i++)
            {
                if (string.Equals(mConfigs[i].Key, key))
                {
                    num++;
                    break;
                }
            }
            return num;
        }

        // 当mConfigs改变时需要更新AllKeys
        private void UpdateAllKeys()
        {
            if (mAllKeys == null || mAllKeys.Length != mConfigs.Count)
                mAllKeys = new string[mConfigs.Count];
            for (int i = 0; i < mAllKeys.Length; i++)
            {
                mAllKeys[i] = mConfigs[i].Key;
            }
        }

        private void UpdateTempKeyAndValue()
        {
            mTempKey = SelectedKey ?? "";
            mTempValue = SelectedValue ?? "";
        }

        // 当前选中的索引
        public int SelectedIndex { get; private set; }
        // 当前选中的Key
        public string SelectedKey
        {
            get
            {
                if (mConfigs == null || SelectedIndex >= mConfigs.Count)
                    return null;
                return mConfigs[SelectedIndex].Key;
            }
        }
        // 当前选中的Value
        public string SelectedValue
        {
            get
            {
                if (mConfigs == null || SelectedIndex >= mConfigs.Count)
                    return null;
                return mConfigs[SelectedIndex].Value;
            }
        }

        private string[] mAllKeys;
        private List<SettingsPair> mConfigs = new List<SettingsPair>();
        private string mTempKey = "";
        private string mTempValue = "";


    }

    public class SettingsPair
    {
        public string Key { get; private set; }
        public string Value { get; private set; }
        // 是否为固定(不可修改和删除)
        public bool IsFixed { get; private set; }
        // 是否显示Key和Value
        public bool ShowKeyAndValue { get; private set; }

        public SettingsPair(string key, string value)
            : this(key, value, false, true)
        {
        }

        public SettingsPair(string key, string value, bool isFixed, bool show)
        {
            Key = key;
            Value = value;
            IsFixed = isFixed;
            ShowKeyAndValue = show;
        }

        public void SetValue(string newValue)
        {
            Value = newValue;
        }
    }
}
