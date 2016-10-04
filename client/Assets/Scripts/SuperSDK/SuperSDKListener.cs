using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spate;
using MiniJson;

namespace Jimmy
{
    public abstract class SuperSDKListener : MonoBehaviour
    {
        /// <summary>
        /// SDK初始化成功
        /// </summary>
        protected abstract void OnSDKInitSuccess();
        /// <summary>
        /// SDK初始化失败
        /// </summary>
        protected abstract void OnSDKInitFailed(string error);
        /// <summary>
        /// SDK登录成功
        /// </summary>
        protected abstract void OnSDKLoginSuccess(SDKUser user);
        /// <summary>
        /// SDK登录失败
        /// </summary>
        protected abstract void OnSDKLoginFailed(string error);
        /// <summary>
        /// SDK登录未实现,采用游戏登录
        /// </summary>
        protected abstract void OnSDKLoginUnrealized();
        /// <summary>
        /// SDK登出成功
        /// </summary>
        protected abstract void OnSDKLogoutSuccess();
        /// <summary>
        /// SDK登出失败
        /// </summary>
        protected abstract void OnSDKLogoutFailed(string error);
        /// <summary>
        /// SDK退出成功
        /// </summary>
        protected abstract void OnSDKExitSuccess();
        /// <summary>
        /// SDK退出失败
        /// </summary>
        protected abstract void OnSDKExitFailed(string error);
        /// <summary>
        /// 取消SDK退出
        /// </summary>
        protected abstract void OnSDKExitCancel();
        /// <summary>
        /// SDK退出未实现
        /// </summary>
        protected abstract void OnSDKExitUnrealized();
        /// <summary>
        /// SDK支付成功
        /// </summary>
        protected abstract void OnSDKPaySuccess(SDKPay pay);
        /// <summary>
        /// SDK支付失败
        /// </summary>
        protected abstract void OnSDKPayFailed(SDKPay pay, string error);
        /// <summary>
        /// 取消SDK支付
        /// </summary>
        protected abstract void OnSDKPayCancel(SDKPay pay);
        /// <summary>
        /// SDK支付未实现
        /// </summary>
        protected abstract void OnSDKPayUnrealized(SDKPay pay);

        /// <summary>
        /// 自定义事件回调
        /// </summary>
        protected abstract void OnCustomCallback(int ret, string key, string value);



        // 来自UnityPlayer.UnitySendMessage呼叫
        public void OnSuperSDKCallback(string json)
        {
            Spate.MLogger.Log("OnSuperSDKCallback===>" + json);
            Dictionary<string, object> map = (Dictionary<string, object>)Json.Deserialize(json);
            // 1-init,2-login,3-logout,4-exit,5-pay,6-custom
            int funcId = int.Parse(map["funcId"].ToString());
            // 0成功，1未实现，2取消，3失败
            int ret = int.Parse(map["ret"].ToString());
            string data = "" + map["data"]; //别用.ToString()防止map["data"]为null

            switch (funcId)
            {
                case SuperSDK.FUNC_ID_SDK_INIT:
                    {
                        // sdk init
                        if (ret == SuperSDK.FUNC_RET_SUCCESS)
                            OnSDKInitSuccess();
                        else
                            OnSDKInitFailed(data);
                    }
                    break;
                case SuperSDK.FUNC_ID_SDK_LOGIN:
                    {
                        // sdk login
                        if (ret == SuperSDK.FUNC_RET_SUCCESS)
                            OnSDKLoginSuccess(Json.Deserialize<SDKUser>(data));
                        else if (ret == SuperSDK.FUNC_RET_UNREALIZED)
                            OnSDKLoginUnrealized();
                        else
                            OnSDKLoginFailed(data);
                    }
                    break;
                case SuperSDK.FUNC_ID_SDK_LOGOUT:
                    {
                        // sdk logout
                        if (ret == SuperSDK.FUNC_RET_SUCCESS)
                            OnSDKLogoutSuccess();
                        else
                            OnSDKLogoutFailed(data);
                    }
                    break;
                case SuperSDK.FUNC_ID_SDK_EXIT:
                    {
                        // sdk exit
                        if (ret == SuperSDK.FUNC_RET_SUCCESS)
                            OnSDKExitSuccess();
                        else if (ret == SuperSDK.FUNC_RET_UNREALIZED)
                            OnSDKExitUnrealized();
                        else if (ret == SuperSDK.FUNC_RET_CANCEL)
                            OnSDKExitCancel();
                        else
                            OnSDKExitFailed(data);
                    }
                    break;
                case SuperSDK.FUNC_ID_SDK_PAY:
                    {
                        // sdk pay
                        if (ret == SuperSDK.FUNC_RET_SUCCESS)
                            OnSDKPaySuccess(Json.Deserialize<SDKPay>(data));
                        else if (ret == SuperSDK.FUNC_RET_UNREALIZED)
                            OnSDKPayUnrealized(Json.Deserialize<SDKPay>(data));
                        else if (ret == SuperSDK.FUNC_RET_CANCEL)
                            OnSDKPayCancel(Json.Deserialize<SDKPay>(data));
                        else
                        {
                            Dictionary<string, object> tm = (Dictionary<string, object>)Json.Deserialize(data);
                            string error = tm["error"].ToString();
                            string payJson = tm["pay"].ToString();
                            OnSDKPayFailed(Json.Deserialize<SDKPay>(payJson), error);
                        }
                    }
                    break;
                case SuperSDK.FUNC_ID_SDK_CUSTOM:
                    {
                        // sdk custom
                        Dictionary<string, object> tm = (Dictionary<string, object>)Json.Deserialize(data);
                        string key = tm["key"].ToString();
                        string value = tm["value"].ToString();
                        OnCustomCallback(ret, key, value);
                    }
                    break;
            }
        }
    }
}
