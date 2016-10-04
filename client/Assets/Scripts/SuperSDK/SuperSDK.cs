using UnityEngine;
using Spate;
using System;
using System.Collections.Generic;
using MiniJson;
#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
using System.Runtime.InteropServices;
#endif

namespace Jimmy
{
    /// <summary>
    /// SuperSDK
    /// </summary>
    public static class SuperSDK
    {
        public const int FUNC_ID_SDK_INIT = 1;
        public const int FUNC_ID_SDK_LOGIN = 2;
        public const int FUNC_ID_SDK_LOGOUT = 3;
        public const int FUNC_ID_SDK_EXIT = 4;
        public const int FUNC_ID_SDK_PAY = 5;
        public const int FUNC_ID_SDK_CUSTOM = 6;

        public const int FUNC_RET_SUCCESS = 0;
        public const int FUNC_RET_UNREALIZED = 1;
        public const int FUNC_RET_CANCEL = 2;
        public const int FUNC_RET_FAILED = 3;

        // 角色-登录
        public const int VALUE_SUBMIT_ROLE_LOGIN = 0;
        // 角色-创建
        public const int VALUE_SUBMIT_ROLE_CREATE = 1;
        // 角色-升级
        public const int VALUE_SUBMIT_ROLE_UPDATE = 2;
        // 角色-退出
        public const int VALUE_SUBMIT_ROLE_EXIT = 3;


        // SDKListener的Login回调到达时是否直接发送给服务器
        public static bool IsLoginReady;
        // sdk登录后引发SDKListener回调,但场景未准备就绪,先缓存
        public static SDKUser CacheUser;

        static SuperSDK()
        {
            // 只需赋值一次
            deviceInfo = GetDeviceInfo();
            channelId = GetChannelId();
            channelName = StringUtil.GetStringUTF8(GetChannelName());
            subChannelId = StringUtil.GetStringUTF8(GetSubChannelId());
            packageName = GetPackageName();
            appVersionCode = GetVersionCode();
            appVersionName = GetVersionName();
            isDebugBuild = IsDebugBuild();
            netHost = GetNetHost();

            sdkVersion = SDK_GetVersion();
            subSdkVersion = SDK_GetSubSDKVersion();
        }

        /// <summary>
        /// 初始化，绑定SDKListener
        /// </summary>
        public static void SetListener(SuperSDKListener listener)
        {
            Listener = listener;
            // 获取Listener的GameObject路径
            List<string> list = new List<string>();
            Transform tf = listener.transform;
            while (tf)
            {
                list.Insert(0, tf.name);
                tf = tf.parent;
            }
            string listenerPath = string.Join("/", list.ToArray());
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_set_listener", listenerPath);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            // UNITY_IOS是U5开始有的，用于替代UNITY_IPHONE(u5开始过时)
            native_set_listener(listenerPath);
#endif

        }


        /// <summary>
        /// 汇报结果，辅助调用SuperSDKListener的OnSuperSDKCallback
        /// </summary>
        /// <param name="funcId">FUNC_ID_SDK_INIT</param>
        /// <param name="ret">FUNC_RET_SUCCESS</param>
        /// <param name="data"></param>
        public static void ReportResult(int funcId, int ret, string data)
        {
            if (Listener != null)
            {
                Dictionary<string, object> map = new Dictionary<string, object>();
                map.Add("funcId", funcId);
                map.Add("ret", ret);
                map.Add("data", data);
                Listener.OnSuperSDKCallback(Json.Serialize(map));
            }
        }

        /// <summary>
        /// 渠道SDK初始化
        /// </summary>
        public static void SDK_Init()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_init");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_init();
#else
            ReportResult(FUNC_ID_SDK_INIT, FUNC_RET_SUCCESS, null);
#endif
        }

        /// <summary>
        /// 渠道SDK登录
        /// </summary>
        public static void SDK_Login()
        {
            if (CacheUser == null)
            {

#if UNITY_ANDROID && !UNITY_EDITOR
             mAndroid.CallStatic("native_sdk_login");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
             native_sdk_login();
#else
                ReportResult(FUNC_ID_SDK_LOGIN, FUNC_RET_UNREALIZED, null);
#endif
            }
            else
            {
                // 从缓存中提取数据
                ReportResult(FUNC_ID_SDK_LOGIN, FUNC_RET_SUCCESS, Json.Serialize(CacheUser));
            }
        }

        /// <summary>
        /// 渠道SDK登出
        /// </summary>
        public static void SDK_Logout()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_logout");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_logout();
#else
            ReportResult(FUNC_ID_SDK_LOGOUT, FUNC_RET_SUCCESS, null);
#endif
        }

        /// <summary>
        /// 渠道SDK退出
        /// </summary>
        public static void SDK_Exit()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_exit");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_exit();
#else
            ReportResult(FUNC_ID_SDK_EXIT, FUNC_RET_UNREALIZED, null);
#endif
        }

        /// <summary>
        /// 渠道SDK支付
        /// </summary>
        public static void SDK_Pay(SDKOrder order)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_pay", Json.Serialize(order));
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_pay(Json.Serialize(order));
#else
            ReportResult(FUNC_ID_SDK_PAY, FUNC_RET_UNREALIZED, Json.Serialize(order.CreatePay()));
#endif
        }

        /// <summary>
        /// 提交角色信息给SDK
        /// </summary>
        /// <param name="role">角色信息</param>
        /// <param name="sceneValue">上报的场景ID,参考VALUE_SUBMIT_ROLE_LOGIN</param>
        public static void SDK_SubmitRole(SDKRole role, int sceneValue)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_submitRole", Json.Serialize(role), sceneValue);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_submitRole(Json.Serialize(role), sceneValue);
#endif
        }

        /// <summary>
        /// 提交附加信息给SDK
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="data">数据</param>
        public static void SDK_SubmitExtra(string type, string data)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_submitExtra", type, data);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_submitExtra(type, data);
#endif
        }

        /// <summary>
        /// 工具条控制
        /// </summary>
        public static void SDK_ShowToolBar(SDKToolBarPlace place)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_sdk_showToolBar", (int)place);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_sdk_showToolBar((int)place);
#endif
        }


        /// <summary>
        /// 获取电源锁
        /// </summary>
        public static void AcquirePowerLock()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_acquirePowerLock");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_acquirePowerLock();
#endif
        }
        /// <summary>
        /// 释放电源锁
        /// </summary>
        public static void ReleasePowerLock()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_releasePowerLock");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_releasePowerLock();
#endif
        }

        /// <summary>
        /// 调用系统Toast(目前仅支持android)
        /// </summary>
        public static void Toast(string text, int duration)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_toast", text, duration);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_toast(text, duration);
#else
            Debug.Log(text);
#endif
        }

        /// <summary>
        /// 调用没返回值的方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="funcArgs">函数参数</param>
        public static void CallFunctionWithVoidReturn(string funcName, string funcArgs)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            mAndroid.CallStatic("native_callFunctionWithVoidReturn", funcName, funcArgs);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            native_callFunctionWithVoidReturn(funcName, funcArgs);
#endif
        }

        /// <summary>
        /// 调用返回int的方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="funcArgs">函数参数</param>
        public static int CallFunctionWithIntReturn(string funcName, string funcArgs)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<int>("native_callFunctionWithIntReturn", funcName, funcArgs);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_callFunctionWithIntReturn(funcName, funcArgs);
#else
            return 0;
#endif
        }

        /// <summary>
        /// 调用没返回float的方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="funcArgs">函数参数</param>
        public static float CallFunctionWithFloatReturn(string funcName, string funcArgs)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<float>("native_callFunctionWithFloatReturn", funcName, funcArgs);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_callFunctionWithFloatReturn(funcName, funcArgs);
#else
            return 0f;
#endif
        }

        /// <summary>
        /// 调用没返回string的方法
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="funcArgs">函数参数</param>
        public static string CallFunctionWithStringReturn(string funcName, string funcArgs)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_callFunctionWithStringReturn", funcName, funcArgs);
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_callFunctionWithStringReturn(funcName, funcArgs);
#else
            return "";
#endif
        }





        // 渠道sdk版本
        public static readonly string sdkVersion;
        // 子渠道sdk版本
        public static readonly string subSdkVersion;

        /// <summary>
        /// 设备信息
        /// </summary>
        public static readonly string deviceInfo;
        /// <summary>
        /// 渠道ID
        /// </summary>
        public static readonly int channelId;
        /// <summary>
        /// 渠道名
        /// </summary>
        public static readonly string channelName;
        /// <summary>
        /// 子渠道ID
        /// </summary>
        public static readonly string subChannelId;
        /// <summary>
        /// 包名
        /// </summary>
        public static readonly string packageName;
        /// <summary>
        /// app VersionCode
        /// </summary>
        public static readonly int appVersionCode;
        /// <summary>
        /// app VersionName
        /// </summary>
        public static readonly string appVersionName;
        /// <summary>
        /// is app is debug build.
        /// </summary>
        public static readonly bool isDebugBuild;
        /// <summary>
        /// Net Host,if null
        /// </summary>
        public static readonly string netHost;
        /// <summary>
        /// 登录后用户的信息
        /// </summary>
        public static SDKUser userInfo { get; internal set; }


        // 获取SDK版本
        private static string SDK_GetVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_sdk_getVersion");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_sdk_getVersion();
#else
            return "0";
#endif
        }
        // 获取子SDK版本
        private static string SDK_GetSubSDKVersion()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_sdk_getSubSdkVersion");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_sdk_getSubSdkVersion();
#else
            return "0";
#endif
        }


        // 获取设备信息
        private static string GetDeviceInfo()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getDeviceInfo");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getDeviceInfo();
#else
            return Json.Serialize(new SDKDevice());
#endif
        }
        // 获取渠道id
        private static int GetChannelId()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<int>("native_getChannelId");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getChannelId();
#else
            return 0;
#endif
        }
        // 获取渠道名
        private static string GetChannelName()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getChannelName");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getChannelName();
#else
            return "";
#endif
        }
        // 获取子渠道Id
        private static string GetSubChannelId()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getSubChannelId");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getSubChannelId();
#else
            return "0";
#endif
        }
        // 获取package-name
        private static string GetPackageName()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getPackageName");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getPackageName();
#else
            return "";
#endif
        }
        // 获取app versionCode
        private static int GetVersionCode()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<int>("native_getVersionCode");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getVersionCode();
#else
            return 1;
#endif
        }
        // 获取app versionName
        private static string GetVersionName()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getVersionName");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getVersionName();
#else
            return "0.0.0";
#endif
        }
        // 获取app是否为debug build
        private static bool IsDebugBuild()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<bool>("native_isDebugBuild");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_isDebugBuild();
#else
            return true;
#endif
        }
        // 获取app是否为debug build
        private static string GetNetHost()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return mAndroid.CallStatic<string>("native_getNetHost");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            return native_getNetHost();
#else
            return "";
#endif
        }






        public static SuperSDKListener Listener { get; private set; }
#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaClass mAndroid = new AndroidJavaClass("com.jimmy.supersdk.SuperSdk");
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
        [DllImport("__Internal")]
		private static extern void native_set_listener(string listenerPath);
        [DllImport("__Internal")]
		private static extern void native_sdk_init();
        [DllImport("__Internal")]
		private static extern void native_sdk_login();
        [DllImport("__Internal")]
		private static extern void native_sdk_logout();
        [DllImport("__Internal")]
		private static extern void native_sdk_exit();
        [DllImport("__Internal")]
		private static extern void native_sdk_pay(string json);
        [DllImport("__Internal")]
		private static extern void native_sdk_submitRole(string json, int sceneValue);
        [DllImport("__Internal")]
		private static extern void native_sdk_submitExtra(string type, string json);
        [DllImport("__Internal")]
		private static extern void native_sdk_showToolBar(int place);


        [DllImport("__Internal")]
		private static extern void native_acquirePowerLock();
        [DllImport("__Internal")]
		private static extern void native_releasePowerLock();
        [DllImport("__Internal")]
		private static extern void native_toast(string text, int duration);
        [DllImport("__Internal")]
		private static extern void native_callFunctionWithVoidReturn(string funcName, string funcArgs);
        [DllImport("__Internal")]
		private static extern int native_callFunctionWithIntReturn(string funcName, string funcArgs);
        [DllImport("__Internal")]
		private static extern float native_callFunctionWithFloatReturn(string funcName, string funcArgs);
        [DllImport("__Internal")]
		private static extern string native_callFunctionWithStringReturn(string funcName, string funcArgs);
        


        [DllImport("__Internal")]
		private static extern string native_sdk_getVersion();
        [DllImport("__Internal")]
		private static extern string native_sdk_getSubSdkVersion();

        [DllImport("__Internal")]
		private static extern string native_getDeviceInfo();
        [DllImport("__Internal")]
		private static extern int native_getChannelId();
        [DllImport("__Internal")]
		private static extern string native_getChannelName();
        [DllImport("__Internal")]
		private static extern string native_getSubChannelId();
        [DllImport("__Internal")]
		private static extern string native_getPackageName();
        [DllImport("__Internal")]
		private static extern int native_getVersionCode();
        [DllImport("__Internal")]
		private static extern string native_getVersionName();
        [DllImport("__Internal")]
		private static extern bool native_isDebugBuild();
        [DllImport("__Internal")]
		private static extern string native_getNetHost();
#endif
    }
}