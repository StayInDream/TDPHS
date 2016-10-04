using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spate;
using Jimmy;

public sealed class HttpRequest : IDisposable
{
    private int mSequence;
    private string mMain;
    private string mSub;
    private string mCmd;
    private Action<int, Dictionary<string, object>> mCallback;

    public int ret;
    public Dictionary<string, object> respData;

    // 判定该请求是否为前台请求
    public bool IsFront { get; private set; }

    public HttpRequest(string sessionID, string main, string sub, string cmd, Action<int, Dictionary<string, object>> callback, bool isFront)
    {
        SetSequence(isFront);
        mMain = main;
        mSub = sub;

        string appVersion = SuperSDK.appVersionCode.ToString();
      //  AppinfoSvrData appInfo = DataManager.Get<AppinfoSvrData>();
    //    string resVersion = (appInfo == null ? ResHost.RES_VERSION_DEFAULT : appInfo.resVersion);
        // 考虑有空cmd的情况
        //if (string.IsNullOrEmpty(cmd))
        //    cmd = string.Format("seq={0}&sid={1}&appVersion={2}&resVersion={3}&codeVersion={4}&channelid={5}&channelName={6}&subChannel={7}", mSequence, sessionID, appVersion, resVersion, Settings.CODE_VERSION, SuperSDK.channelId, SuperSDK.channelName, SuperSDK.subChannelId);
        //else
        //    cmd = string.Format("{0}&seq={1}&sid={2}&appVersion={3}&resVersion={4}&codeVersion={5}&channelid={6}&channelName={7}&subChannel={8}", cmd, mSequence, sessionID, appVersion, resVersion, Settings.CODE_VERSION, SuperSDK.channelId, SuperSDK.channelName, SuperSDK.subChannelId);
        mCmd = cmd;
        mCallback = callback;
        IsFront = isFront;
    }

    public void SetSequence(bool flag)
    {
        mSequence = flag ? NetCounter.AutoIncrement() : 0;
    }

    public string ToUrl()
    {
        //    AppinfoSvrData appInfo = DataManager.Get<AppinfoSvrData>();
        //string resVersion = (appInfo == null ? ResHost.RES_VERSION_DEFAULT : appInfo.resVersion);
        //  string url = string.Format("{0}/{1}/{2}?resVersion={3}&codeVersion={4}&channelid={5}&channelName={6}&subChannel={7}&platform={8}", NetHost.Url, mMain, mSub, resVersion, Settings.CODE_VERSION, SuperSDK.channelId, SuperSDK.channelName, SuperSDK.subChannelId, Settings.PLATFORM_NAME);
        return null;//url;
    }

    public byte[] ToData()
    {
        byte[] cmdBuff = Encoding.UTF8.GetBytes(mCmd);
        Encrypter.EncodeXor(cmdBuff);
        return cmdBuff;
    }

    public override string ToString()
    {
        return string.Format("{0} \n{1}", ToUrl(), mCmd);
    }

    public void Callback()
    {
        if (mCallback != null)
        {
            mCallback(ret, respData);
        }
    }

    public void Dispose()
    {
        mMain = null;
        mSub = null;
        mCmd = null;
        mCallback = null;
        if (respData != null) respData.Clear();
        respData = null;
    }
}
