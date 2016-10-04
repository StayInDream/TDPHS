namespace Jimmy
{
    public sealed class SDKDevice
    {
        // 设备型号
        public string device;
        // 系统版本
        public string osVersion;
        // 手机串号
        public string imei;
        // 国际移动用户号
        public string imsi;
        // 网络模式(wifi,2g,3g,4g)
        public string network;
        // 电信运营商识别码(移动、联通、电信)
        public string provider;

        public string ToHttpGetParams()
        {
            string ua = device;
            if (!string.IsNullOrEmpty(osVersion))
                ua = string.Concat(ua, ";", osVersion);
            if(!string.IsNullOrEmpty(ua)) ua = StringUtil.GetStringUTF8(ua);
            return string.Format("imei={0}&imsi={1}&provider={2}&wifi={3}&ua={4}", imei, imsi, provider, network, ua);
        }
    }
}
