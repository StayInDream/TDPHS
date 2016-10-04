namespace Jimmy
{
    /// <summary>
    /// SDK订单信息
    /// </summary>
    public sealed class SDKOrder
    {
        public string cpOrderId;
        /// <summary>
        /// 物品ID，游戏内定义的，比如1表示月卡，2表示季卡等
        /// </summary>
        public string goodsId;
        /// <summary>
        /// 物品名称，游戏内定义，比如月卡，季卡等
        /// </summary>
        public string goodsName;
        /// <summary>
        /// 单价
        /// </summary>
        public float price;
        /// <summary>
        /// 总价
        /// </summary>
        public float amount;
        /// <summary>
        /// 数量
        /// </summary>
        public int count;
        /// <summary>
        /// 附加参数,会在支付回调中完整的返回
        /// </summary>
        public string extrasParams;
        /// <summary>
        /// 支付回调地址
        /// </summary>
        public string callbackUrl;

        public SDKPay CreatePay()
        {
            SDKPay pay = new SDKPay();
            pay.cpOrderId = cpOrderId;
            pay.extraParam = extrasParams;
            return pay;
        }
    }
}
