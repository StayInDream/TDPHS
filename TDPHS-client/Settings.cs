using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LP
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

        static Settings()
        {
            Debug = Launcher.isDebug;


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
