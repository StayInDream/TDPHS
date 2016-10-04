using System;

namespace Spate
{
    public static class DataHost
    {
        // 数据来源方式
        public static Source source = Source.FromNetHost;
        // 数据表最终url
        public static string Url { get; private set; }

        public static void SetUrl(string url)
        {
            if (url.EndsWith("/"))
                url.Substring(0, url.Length - 1);
            Url = url;
        }

        // 构建url
        public static void BuildUrl()
        {

        }

        public enum Source
        {
            /// <summary>
            /// 由服务器派发
            /// </summary>
            FromNetHost,
            /// <summary>
            /// 本地CSV
            /// </summary>
            LocalCSV,
            /// <summary>
            /// 自定义url
            /// </summary>
            CustomUrl
        }
    }
}
