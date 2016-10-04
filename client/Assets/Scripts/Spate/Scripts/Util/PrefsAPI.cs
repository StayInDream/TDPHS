using System;
using System.Collections.Generic;
using UnityEngine;

/*本地数据存储
*/
public class PrefsAPI
{
    private const string DEFAULT_SERVER = "DefaultServer"; //设置登陆默认服务器
    //
    public const string USER_NAME = "username";  //用户名
    public const string PWD = "pwd";//用户密码
    //
    public const string LAYER_UI = "UI";
    //
    private const string BGM_VOl = "bgmVol";
    private const string SE_VOl = "seVol";
    private const string VOICE_VOL = "voiceVol";

    private const string KEY_SP_VIT_SWITCH = "KEY_SP_VIT_SWITCH";
    private const string KEY_SP_VIT_GET12_SWITCH = "KEY_SP_VIT_GET12_SWITCH";
    private const string KEY_SP_VIT_GET18_SWITCH = "KEY_SP_VIT_GET18_SWITCH";
    private const string KEEP_CAMERA_RIGHT_POS = "KEEP_CAMERA_RIGHT_POS";
    private const string KEEP_CAMERA_LEFT_POS = "KEEP_CAMERA_RIGHT_POS";

    public const int APTIME = 480;//ap回复时间，暂定8分钟

    public const string MAIN_CAMERA_POS = "Main_Camera_Pos";
    public const string MAP_CAMERA_POS = "Map_Camera_Pos";
    public const string STAGE_CHAPTERID = "Stage_Chapterid";
    public const string PVP_GIVEUP_SIGN = "Pvp_Giveup_Sign";
    //引导自动战斗
    public const string GUIDE_BATTLE_AUTO = "Guide_Battle_auto";
    public const string GUIDE_BATTLE_FAST = "Guide_Battle_fast";

    #region 战斗数据
    private const string KEY_BATTLE_DATA_SPEED = "KEY_BATTLE_DATA_SPEED";
    private const string KEY_BATTLE_DATA_MODE = "KEY_BATTLE_DATA_MODE";

    public static void SaveBattleSpeed(float val)
    {
        PlayerPrefs.SetFloat(KEY_BATTLE_DATA_SPEED, val);
    }

    public static float GetBattleSpeed(float defaultValue)
    {
        return PlayerPrefs.GetFloat(KEY_BATTLE_DATA_SPEED, defaultValue);
    }

  
    #endregion

    #region DefaultServer
    //public static int GetDefaultServer()
    //{
    //    return PlayerPrefs.GetInt(DEFAULT_SERVER, 0);
    //}

    //public static void SetDefaultServer(int defaultServer)
    //{
    //    PlayerPrefs.SetInt(DEFAULT_SERVER, defaultServer);
    //}
    #endregion

    #region RoleInfo
    public static string GetUserName()
    {
        return PlayerPrefs.GetString(USER_NAME, string.Empty);
    }

    public static void SetUserName(string username)
    {
        PlayerPrefs.SetString(USER_NAME, username);
    }

    public static string GetPassWord()
    {
        return PlayerPrefs.GetString(PWD, string.Empty);
    }

    public static void SetPassWord(string pwd)
    {
        PlayerPrefs.SetString(PWD, pwd);
    }
    #endregion

    #region 音量
    public static float GetBgmVolume()
    {
        return PlayerPrefs.GetFloat(BGM_VOl, 1f);
    }

    public static void SetBgmVolume(float vol)
    {
        PlayerPrefs.SetFloat(BGM_VOl, vol);
    }

    public static float GetSeVolume()
    {
        return PlayerPrefs.GetFloat(SE_VOl, 1f);
    }

    public static void SetSeVolume(float vol)
    {
        PlayerPrefs.SetFloat(SE_VOl, vol);
    }

    public static float GetVoiceVolume()
    {
        return PlayerPrefs.GetFloat(VOICE_VOL, 1f);
    }

    public static void SetVoiceVolume(float vol)
    {
        PlayerPrefs.SetFloat(VOICE_VOL, vol);
    }
    #endregion

    #region 通知
    public static void SetSpVitSwitch(bool state)
    {
        PlayerPrefs.SetInt(KEY_SP_VIT_SWITCH, state ? 1 : 0);
    }
    public static void SetSpVitGet12Switch(bool state)
    {
        PlayerPrefs.SetInt(KEY_SP_VIT_GET12_SWITCH, state ? 1 : 0);
    }
    public static void SetSpVitGet18Switch(bool state)
    {
        PlayerPrefs.SetInt(KEY_SP_VIT_GET18_SWITCH, state ? 1 : 0);
    }

    /// <summary>
    /// 0为继续提示 1为不提示
    /// </summary>
    public static void SetPvpGiveUpSign(int state)
    {
        PlayerPrefs.SetInt(PVP_GIVEUP_SIGN, state);
    }

    public static bool GetSpVitSwitch()
    {
        return PlayerPrefs.GetInt(KEY_SP_VIT_SWITCH, 1) == 1;
    }

    public static bool GetSpVitGet12Switch()
    {
        return PlayerPrefs.GetInt(KEY_SP_VIT_GET12_SWITCH, 1) == 1;
    }

    public static bool GetSpVitGet18Switch()
    {
        return PlayerPrefs.GetInt(KEY_SP_VIT_GET18_SWITCH, 1) == 1;
    }

    /// <summary>
    /// 0为继续提示 1为不提示
    /// </summary>
    public static int GetPvpGiveUpSign()
    {
        return PlayerPrefs.GetInt(PVP_GIVEUP_SIGN, 0);
    }
    #endregion

    public static void SetMainCameraPos(float Pos_x)
    {
        PlayerPrefs.SetFloat(MAIN_CAMERA_POS, Pos_x);
    }

    public static float GetMainCameraPos()
    {
        return PlayerPrefs.GetFloat(MAIN_CAMERA_POS, 0.0f);
    }

    public static void DeletePlayPrefs(string str)
    {
        PrefsAPI.DeletePlayPrefs(str);
    }

    public static void SetMapCameraPos(float Pos_x)
    {
        PlayerPrefs.SetFloat(MAP_CAMERA_POS, Pos_x);
    }
    public static float GetMapCameraPos()
    {
        return PlayerPrefs.GetFloat(MAP_CAMERA_POS, 135.5f);
    }
  
    /// <summary>
    /// true 为已经引导过了，0为还没有被引导
    /// </summary>
    public static bool GetBattleAutoState()
    {
        return PlayerPrefs.GetInt(GUIDE_BATTLE_AUTO, 0) == 1;
    }

    public static void SetBattleAutoState(bool guide)
    {
        PlayerPrefs.SetInt(GUIDE_BATTLE_AUTO, guide ? 1 : 0);
        PlayerPrefs.Save();
    }

    public static bool GetBattleFastState()
    {
        return PlayerPrefs.GetInt(GUIDE_BATTLE_FAST, 0) == 1;
    }

    public static void SetBattleFastState(bool guide)
    {
        PlayerPrefs.SetInt(GUIDE_BATTLE_FAST, guide ? 1 : 0);
        PlayerPrefs.Save();
    }
}

