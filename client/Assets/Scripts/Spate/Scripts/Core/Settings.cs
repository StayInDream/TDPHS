using System;
using System.IO;
using UnityEngine;

namespace Spate
{
    public static class Settings
    {
        // 代码版本
        public const uint CODE_VERSION = 11;

        // 当前是否为Debug模式
        public static readonly bool Debug = false;
        // 当前是否为手机平台
        public static readonly bool isMobilePlatform;

        // 存放自定义数据的根目录,例如mpq，日志等
        public static readonly string UNITY_FOLDER;
        // 存放更新过程中的临时文件
        public static string UNITY_RAW_FOLDER;
        // 存放随包资源的文件
        public static string UNITY_RAW_READONLY_FOLDER;
        // 存放日志文件
        public static string UNITY_LOG_FOLDER;
        // 当前平台的名称(win,android,ios)
        public static readonly string PLATFORM_NAME;
   
        /// <summary>
        /// UIPackage window在Resources文件夹下的存放路径
        /// </summary>
        public static readonly string UIPACKAGE_WIN_FOLDER = "UI/";

        // 本地CSV的目录
        public static readonly string CSV_FOLDER;
        // 是否显示调试信息
        public static bool ShowDebugInfo
        {
            get; set;
        }

        public static bool IsKipGuide
        {
            get; set;
        }   

        // 是否开启彩蛋
        public static bool Eggshell
        {
            get; private set;
        }

        // 开启支付功能
        public static bool PaymentEnable
        {
            get; private set;
        }


        public static string JpushId;

        static Settings()
        {
            Debug = Jimmy.SuperSDK.isDebugBuild;


            UNITY_RAW_READONLY_FOLDER = UrlUtil.Combine( Application.streamingAssetsPath, "raw" );
            switch(Application.platform)
            {
                case RuntimePlatform.Android:
                    isMobilePlatform = true;
                    PLATFORM_NAME = "android";
                    UNITY_FOLDER = Application.persistentDataPath;
                    break;
                case RuntimePlatform.IPhonePlayer:
                    isMobilePlatform = true;
                    PLATFORM_NAME = "ios";
                    UNITY_FOLDER = Application.persistentDataPath;
                    break;
                default:
                    isMobilePlatform = false;
                    PLATFORM_NAME = "win";
                    UNITY_FOLDER = UrlUtil.Combine( Application.dataPath, "../unity" );
                    CSV_FOLDER = Application.dataPath + "/../../gamedata/assets/csv/";
                   // UIPACKAGE_WIN_FOLDER = Application.dataPath + "/../../Asset/UI/Window/";
                    break;
            }

            UNITY_FOLDER = UNITY_FOLDER.Replace( "\\", "/" );
            FileUtil.EnsureFolder( UNITY_FOLDER );

            UNITY_RAW_FOLDER = UrlUtil.Combine( UNITY_FOLDER, "raw" );
            FileUtil.EnsureFolder( UNITY_RAW_FOLDER );
            UNITY_LOG_FOLDER = UrlUtil.Combine( UNITY_FOLDER, "log" );
            FileUtil.EnsureFolder( UNITY_LOG_FOLDER );


            // 通过指定文件开启debug
            if(!Debug)
            {
                try
                {
                    string p = Path.Combine( UNITY_FOLDER, "把这个文件放在SDCard中开启debug功能.txt" );
                    if(!File.Exists( p ))
                        p = Path.Combine( Application.persistentDataPath, "把这个文件放在SDCard中开启debug功能.txt" );
                    if(File.Exists( p ))
                    {
                        string hash = Hasher.CalcHash( File.OpenRead( p ) );
                        if(string.Equals( "BD96397339913DD2F06CFDEAD3AF0D2C", hash, StringComparison.OrdinalIgnoreCase ))
                            Debug = true;
                    }
                }
                catch { }
            }

            // 是否开启彩蛋,暂定条件
            if(Debug)
            {
                try
                {
                    string p = Path.Combine( UNITY_FOLDER, "把这个文件放在SDCard中开启彩蛋功能.txt" );
                    if(!File.Exists( p ))
                        p = Path.Combine( Application.persistentDataPath, "把这个文件放在SDCard中开启彩蛋功能.txt" );
                    if(File.Exists( p ))
                    {
                        string hash = Hasher.CalcHash( File.OpenRead( p ) );
                        if(string.Equals( "DF9246B01F1EC45C51D2FD223D9A6818", hash, StringComparison.OrdinalIgnoreCase ))
                            Eggshell = true;
                    }
                }
                catch { }
            }

            PaymentEnable = true;
            if(isMobilePlatform)
            {
                if(Jimmy.SuperSDK.channelId <= 0)
                {
                    PaymentEnable = false;
                }
                //PaymentEnable = false;
            }
        }
    }
}
