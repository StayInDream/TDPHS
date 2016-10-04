using System;

namespace Spate
{
    public static class ResHost
    {
        public const string RES_VERSION_DEFAULT = "0";

        // 资源来源方式
        public static Source source = Source.FromNetHost;
        // 资源最终url
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
            string platform = "win";
#if UNITY_ANDROID && !UNITY_EDITOR
            platform = "android";
#elif (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR
            platform = "ios";
#endif
            Url = UrlUtil.Combine(Url, platform);
        }

        public enum Source
        {
            /// <summary>
            /// 由服务器派发
            /// </summary>
            FromNetHost,
            /// <summary>
            /// 编辑器
            /// </summary>
            EditorAsset,
            /// <summary>
            /// 自定义url
            /// </summary>
            CustomUrl
        }
    }
}
