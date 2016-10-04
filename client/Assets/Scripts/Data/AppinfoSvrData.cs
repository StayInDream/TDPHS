using System;
using Spate;

public class AppinfoSvrData : BaseNoKeyData
{
    [PrimaryKey]
    // 数据的版本
    public string resVersion;
    // 数据的host
    public string resUrl;
    // 资源的host
    public string assetUrl;

    // app更新地址,如果该字段不为空,则表示客户端需要强制更新,并跳转到该页面下载
    public string download;
    // 重定向host,如果该字段不为空则表示客户端需要连接到新的host
    public string redirectUrl;

    public string appVersion;
    public string minVersion;

    public string desc;
    public string desc2;

    public string payUrl;
}
