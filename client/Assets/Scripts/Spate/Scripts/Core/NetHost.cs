using System;

namespace Spate
{
    public static class NetHost
    {
        // 外网正式服:http://ws.you2game.com
        public const string HOST_DEFAULT = "http://ws.you2game.com";

        // 外网测试服:http://120.25.227.65:9003
        // 版署审核服:http://120.25.227.65:9000

        public static string Url { get; private set; }

        public static void SetUrl(string url)
        {
            if (url.EndsWith("/"))
                url.Substring(0, url.Length - 1);
            Url = url;
        }

        public static void SetDefault()
        {
            string host = Jimmy.SuperSDK.netHost;
            if (string.IsNullOrEmpty(host))
                host = HOST_DEFAULT;
            if (host.EndsWith("/"))
                host.Substring(0, host.Length - 1);
            SetUrl(host);
        }

        public static string PayBackUrl;
    }
}
