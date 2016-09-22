using System;
using System.IO;
using UnityEngine;

namespace LP
{
    public static class Settings
    {
        /// <summary>
        /// 代码版本
        /// </summary>
        public const uint CODE_VERSION = 11;
        // 
        /// <summary>
        /// 当前是否为Debug模式
        /// </summary>
        public static readonly bool Debug = false;

        // 
        /// <summary>
        /// 当前是否为手机平台
        /// </summary>
        public static readonly bool isMobilePlatform;
        // 
        /// <summary>
        /// 存放自定义数据的根目录,例如mpq，日志等
        /// </summary>
        public static readonly string UNITY_FOLDER;
        //
        /// <summary>
        ///  存放更新过程中的临时文件
        /// </summary>
        public static string UNITY_RAW_FOLDER;
        // 
        /// <summary>
        /// 存放随包资源的文件
        /// </summary>
        public static string UNITY_RAW_READONLY_FOLDER;
        /// <summary>
        /// 存放日志的文件夹
        /// </summary>
        public static string UNITY_LOG_FOLDER;

        /// <summary>
        /// 正常日志文件
        /// </summary>
        public static string UNITY_LOG_FILE;

        /// <summary>
        /// 异常日志文件
        /// </summary>
        public static string UNITY_LOG_ERROR;

        /// <summary>
        /// 当前平台的名称(win,android,ios)
        /// </summary>
        public static readonly string PLATFORM_NAME;

        static Settings()
        {
            Debug = Launcher.IsDebug;
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
                    break;
            }

            UNITY_FOLDER = UNITY_FOLDER.Replace( "\\", "/" );
            FileUtil.EnsureFolder( UNITY_FOLDER );

            UNITY_LOG_FOLDER = UrlUtil.Combine( UNITY_FOLDER, "log" );
            FileUtil.EnsureFolder( UNITY_LOG_FOLDER );

            UNITY_RAW_FOLDER = UrlUtil.Combine( UNITY_FOLDER, "raw" );
            FileUtil.EnsureFolder( UNITY_RAW_FOLDER );

            
            if(!Debug)
            {
                try
                {
                    FileUtil.EnsureFileParent( UNITY_LOG_FOLDER ,"log.txt" );
                    UNITY_LOG_FILE = UrlUtil.Combine( UNITY_LOG_FOLDER,"log.txt" );

                    FileUtil.EnsureFileParent(UNITY_LOG_FOLDER,"logError.txt");
                    UNITY_LOG_ERROR = UrlUtil.Combine( UNITY_LOG_FOLDER, "logError.txt" );
                }
                catch { }
            }
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
    }
}
