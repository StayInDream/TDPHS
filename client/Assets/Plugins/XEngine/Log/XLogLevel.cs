
namespace XEngine
{
    public enum XLogLevel
    {
        /// <summary>
        /// 通常用于输出一些调试类的信息,Release模式下建议关闭该等级
        /// </summary>
        Debug,
        /// <summary>
        /// 辅助性的提示
        /// </summary>
        Info,
        /// <summary>
        /// 警告
        /// </summary>
        Warn,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
        /// <summary>
        /// 异常
        /// </summary>
        Exception
    }
}
