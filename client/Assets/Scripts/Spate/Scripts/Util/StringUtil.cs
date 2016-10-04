using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Spate;
using System.Text;

public static class StringUtil
{
    public static void RemoveClone(GameObject go)
    {
        go.name = go.name.Replace("(Clone)", "");
    }

    public static string WrapProtocol(string url)
    {
        if (url.StartsWith("jar:file://", StringComparison.OrdinalIgnoreCase))
            return url;
        else if (url.StartsWith("/"))
            return "file://" + url;
        return "file:///" + url;
    }

    public static bool CheckString(string effect)
    {
        return !string.IsNullOrEmpty(effect) && !string.Equals(effect.Trim(), "0");
    }

    public static bool IsChineseChar(char ch)
    {
        int code = char.ConvertToUtf32(ch.ToString(), 0);
        return code >= 0x4e00 && code <= 0x9fff;
    }

    public static int StringToInt(string str)
    {
        if (string.IsNullOrEmpty(str))
            return 0;
        else
            return int.Parse(str);
    }


    public static string GetRatioOrValueText(float ratio)
    {
        string str = "";
        if (ratio < 1)
        {
            str = ratio * 100 + "%";
        }
        else
        {
            str = ratio.ToString();
        }
        return str;
    }

   
    public static int GetStringLength(string str)
    {
        string temp = str;
        int j = 0;
        for (int i = 0; i < temp.Length; i++)
        {
            if (Regex.IsMatch(temp.Substring(i, 1), @"[\u4e00-\u9fa5]+"))
            {
                j += 2;
            }
            else
            {
                j += 1;
            }

        }
        return j;
    }
    public static string GetStringUTF8(string name)
    {
        StringBuilder sb = new StringBuilder();
        byte[] buffer = Encoding.UTF8.GetBytes(name);
        foreach (byte b in buffer)
        {
            sb.Append(string.Format("%{0:X2}", b));
        }
        return sb.ToString();
    }
    //
    public static DateTime TransStringToDateTimeSecond(string str, bool utc2beijing = false)
    {
        if (String.IsNullOrEmpty(str) || str.Trim().Equals("0"))
        {
            //Log.Log_hjx("str :" + str + ":");
            return DateTime.MinValue;
        }
        //Log.Log_hjx("str " + str);
        int year = int.Parse(str.Substring(0, 4));
        int month = int.Parse(str.Substring(4, 2));
        int day = int.Parse(str.Substring(6, 2));
        int hours = int.Parse(str.Substring(8, 2));
        int minute = int.Parse(str.Substring(10, 2));
        int second = int.Parse(str.Substring(12, 2));
        //Log.Log_hjx("year " + year + "month " + month + "day " + day + " hours " + hours + " minute " + minute + "second " + second);
        DateTime d = new DateTime(year, month, day, hours, minute, second, DateTimeKind.Utc);
        if (utc2beijing)
            d = d.AddHours(-8);
        return d;
    }

    public static DateTime TransStringToDateTimeMinute(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Trim().Equals("0"))
        {
            //Log.Log_hjx("str :" + str + ":");
            return DateTime.MinValue;
        }
        int year = int.Parse(str.Substring(0, 4));
        int month = int.Parse(str.Substring(4, 2));
        int day = int.Parse(str.Substring(6, 2));
        int hours = int.Parse(str.Substring(8, 2));
        int minute = int.Parse(str.Substring(10, 2));
        //Log.Log_hjx("year " + year + "month " + month + "day " + day + " hours " + hours + " minute " + minute);
        DateTime d = new DateTime(year, month, day, hours, minute, 0, DateTimeKind.Utc);
        return d;
    }

    public static string TransDateTimeSecondToString(DateTime time)
    {
        string str = "";
        //if(time != null)
        //{
        str = time.Year + "";
        str += time.Month < 10 ? "0" + time.Month : time.Month + "";
        str += time.Day < 10 ? "0" + time.Day : time.Day + "";
        str += time.Hour < 10 ? "0" + time.Hour : time.Hour + "";
        str += time.Minute < 10 ? "0" + time.Minute : time.Minute + "";
        str += time.Second < 10 ? "0" + time.Second : time.Second + "";
        //}
        return str;
    }

    public static string TransStringToTimeSecondString(string str)
    {
        if (string.IsNullOrEmpty(str) || str.Trim().Equals("0"))
        {
            //Log.Log_hjx("str :" + str + ":");
            return "";
        }
        string year = str.Substring(0, 4);
        string month = str.Substring(4, 2);
        string day = str.Substring(6, 2);
        string hours = string.Format("{0:D2}", str.Substring(8, 2));
        string minute = string.Format("{0:D2}", str.Substring(10, 2));
        string second = string.Format("{0:D2}", str.Substring(12, 2));
        //Log.Log_hjx("year " + year + "month " + month + "day " + day + " hours " + hours + " minute " + minute);
        String timestr = year + "." + month + "." + day + "  " + hours + ":" + minute + ":" + second;
        return timestr;
    }
    public static string TransStringToHourMinuSecondString(string str)
    {
        if (str.Trim().Equals("0"))
        {
            return "";
        }
        string hours = string.Format("{0:D2}", str.Substring(8, 2));
        string minute = string.Format("{0:D2}", str.Substring(10, 2));
        string second = string.Format("{0:D2}", str.Substring(12, 2));
        String timestr = hours + ":" + minute + ":" + second;
        return timestr;
    }

    /// <summary>
    /// str 时钟和分钟
    /// </summary>
    public static DateTime TransStringToHourMinuSecond(string str)
    {
        if (str.Trim().Equals("0"))
        {
            return DateTime.MinValue;
        }
        DateTime curTime = AlarmManager.GetCurrentDateTime();
        string time = curTime.ToString("yyyyMMdd") + str + "00";
        return TransStringToDateTimeSecond(time);
    }

    public static string TransSecondToStringTime(long t, bool showhour = false)
    {
        if (t > 0)
        {
            int hour = 0;
            int minu = 0;
            int second = 0;
            hour = (int)(t / 3600);
            minu = (int)((t % 3600) / 60);
            second = (int)(t % 60);
            string HOUR = hour > 9 ? hour.ToString() : ("0" + hour);
            string MINU = minu > 9 ? minu.ToString() : ("0" + minu);
            string SEC = second > 9 ? second.ToString() : ("0" + second);
            if (showhour)
            {
                return HOUR + ":" + MINU + ":" + SEC;
            }
            else
            {
                return MINU + ":" + SEC;
            }
        }
        else
        {
            return "";
        }
    }
    public static string GetRoleCreateTimeUnix(string rolecreatetime)
    {
        if (!string.IsNullOrEmpty(rolecreatetime))
        {
            DateTime dt = StringUtil.TransStringToDateTimeSecond(rolecreatetime, true);
            return ((dt.Ticks - 621355968000000000) * 0.0001 * 0.001).ToString();
        }
        else
            return "";
    }

    public static string ShowNoMountedPrompt(int index)
    {
        string str = string.Empty;
        switch (index)
        {
            case 1:
                str = "装备强化到5级时开启";
                break;
            case 2:
                str = "装备强化到10级时开启";
                break;
            case 3:
                str = "装备强化到15级时开启";
                break;
            case 4:
                str = "装备强化到20级时开启";
                break;
            case 5:
                str = "装备强化到25级时开启";
                break;
        }
        return str;

    }

 

  

    public static string GetLayerTitle(int layer)
    {
        string str = string.Empty;
        switch (layer)
        {
            case 1:
                str = "第一层:";
                break;
            case 2:
                str = "第二层:";
                break;
            case 3:
                str = "第三层:";
                break;
            case 4:
                str = "第四层:";
                break;
            case 5:
                str = "第五层:";
                break;
            case 6:
                str = "第六层:";
                break;
            case 7:
                str = "第七层:";
                break;
            case 8:
                str = "第八层:";
                break;
            case 9:
                str = "第九层:";
                break;
        }
        return str;
    }

    public static void ParseArgs<T>(string args, ref T val1)
    {
        string[] array = args.Split(';');
        val1 = (T)Convert.ChangeType(array[0], typeof(T));
    }
    public static void ParseArgs<T, U>(string args, ref T val1, ref U val2)
    {
        string[] array = args.Split(';');
        val1 = (T)Convert.ChangeType(array[0], typeof(T));
        val2 = (U)Convert.ChangeType(array[1], typeof(U));
    }
    public static void ParseArgs<T, U, V>(string args, ref T val1, ref U val2, ref V val3)
    {
        string[] array = args.Split(';');
        val1 = (T)Convert.ChangeType(array[0], typeof(T));
        val2 = (U)Convert.ChangeType(array[1], typeof(U));
        val3 = (V)Convert.ChangeType(array[2], typeof(V));
    }
    public static void ParseArgs<T, U, V, Q>(string args, ref T val1, ref U val2, ref V val3, ref Q val4)
    {
        string[] array = args.Split(';');
        val1 = (T)Convert.ChangeType(array[0], typeof(T));
        val2 = (U)Convert.ChangeType(array[1], typeof(U));
        val3 = (V)Convert.ChangeType(array[2], typeof(V));
        val4 = (Q)Convert.ChangeType(array[3], typeof(Q));
    }
    public static void ParseArgs<T, U, V, Q, W>(string args, ref T val1, ref U val2, ref V val3, ref Q val4, ref W val5)
    {
        string[] array = args.Split(';');
        val1 = (T)Convert.ChangeType(array[0], typeof(T));
        val2 = (U)Convert.ChangeType(array[1], typeof(U));
        val3 = (V)Convert.ChangeType(array[2], typeof(V));
        val4 = (Q)Convert.ChangeType(array[3], typeof(Q));
        val5 = (W)Convert.ChangeType(array[4], typeof(W));
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="evaIndex">评分等级</param>
    /// <returns></returns>
    public static string GetEquipEvaluateSpname(int evaIndex)
    {
        string spriteName = string.Empty;
        switch (evaIndex)
        {
            case 1:
                spriteName = "equip_evaluate_D";
                break;
            case 2:
                spriteName = "equip_evaluate_C";
                break;
            case 3:
                spriteName = "equip_evaluate_B";
                break;
            case 4:
                spriteName = "equip_evaluate_A";
                break;
            case 5:
                spriteName = "equip_evaluate_S";
                break;
            case 6:
                spriteName = "equip_evaluate_SS";
                break;
            case 7:
                spriteName = "equip_evaluate_SSS";
                break;
        }

        return spriteName;
    }


    public static string GetHeroJobName(int job)
    {
        string str = "";
        switch (job)
        {
            case 1:
                str = "hero_big_Near";
                break;
            case 2:
                str = "hero_big_Remote";
                break;
            case 4:
                str = "hero_big_Assist";
                break;
            default:
                str = "";
                break;
        }
        return str;
    }
    public static string GetHeroSmallJobName(int job)
    {
        string str = "";
        switch (job)
        {
            case 1:
                str = "hero_small_Near";
                break;
            case 2:
                str = "hero_small_Remote";
                break;
            case 4:
                str = "hero_small_Assist";
                break;
            default:
                str = "";
                break;
        }
        return str;
    }

  
    public static string GetHeroQualityName(int quality, int star)
    {
        string str = "";
        switch (quality)
        {
            case 1:
                str = "蓝色";
                break;
            case 2:
                str = "紫色";
                break;
            case 3:
                str = "橙色";
                break;
            default:
                str = "色色";
                break;
        }
        str += star + "星";
        return str;
    }

    ///// <summary>
    ///// 武道会特殊处理添加Level
    ///// </summary>
    //public static string GetWindowName(eModuleType type, int level = 0)
    //{
    //    string name = string.Empty;
    //    switch (type)
    //    {
    //        case eModuleType.Stayend:
    //            name = "StayendWindow";
    //            break;
    //        case eModuleType.Budokai:
    //            if (level == 0)
    //                name = "ArenalandWindow";
    //            else if (level == 1)
    //                name = "BudokaiWindow";
    //            else if (level == 2)
    //                name = "LadderMatchWindow";
    //            else
    //                Logger.LogError(string.Format("类型为{0}子模块为{1}的玩法", type, level));
    //            break;
    //        case eModuleType.Copy:
    //            name = "MoneyCopyWindow";
    //            break;
    //        case eModuleType.Tower:
    //            name = "DemontowerWindow";
    //            break;
    //        case eModuleType.Dreamland:
    //            name = "DreamlandWindow";
    //            break;
    //        case eModuleType.Recurit:
    //            name = "RecruitWindow";
    //            break;
    //        case eModuleType.Equip:
    //            name = "EquipWindow";
    //            break;
    //        case eModuleType.TreasureBank:
    //            name = "RareTreasureWindow";
    //            break;
    //        case eModuleType.Compose:
    //            name = "ComposeWindow";
    //            break;
    //        case eModuleType.Guild:
    //            if (DataAPI.IsHadGuild())
    //                name = "GuildWindow";
    //            else
    //                name = "GuildleftWindow";
    //            break;
    //        case eModuleType.RareTreasure:
    //            name = "ShoppingWindow";
    //            break;
    //        case eModuleType.VarietyStore:
    //            name = "ShoppingWindow";
    //            break;
    //        case eModuleType.Hero:
    //            name = "HerodetailWindow";
    //            break;
    //        case eModuleType.Team:
    //            name = "TeamdetailWindow";
    //            break;
    //        case eModuleType.Package:
    //            name = "PackageWindow";
    //            break;
    //        case eModuleType.Task:
    //            name = "TaskWindow";
    //            break;
    //        case eModuleType.KaifuActivity:
    //            name = "KaifuActivityWindow";
    //            break;
    //        case eModuleType.PayActivity:
    //            name = "PayActivityWindow";
    //            break;
    //        case eModuleType.GuideLines:
    //            name = "GuidelinesWindow";
    //            break;
    //        case eModuleType.Friend:
    //            name = "RelationListWindow";
    //            break;
    //        case eModuleType.Email:
    //            name = "EmailBoxWindow";
    //            break;
    //        case eModuleType.Soul:
    //            name = "GeneralsoulWindow";
    //            break;
    //        case eModuleType.Ranking:
    //            name = "RankinglistWindow";
    //            break;
    //        case eModuleType.Mall:
    //            name = "MallWindow";
    //            break;
    //        case eModuleType.Mine:
    //            name = "CoinLootWindow";
    //            break;
    //        default:
    //            name = string.Empty;
    //            Logger.Log(type + "无此类型的窗口");
    //            break;
    //    }
    //    return name;
    //}

 
}
