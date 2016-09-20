using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;

using UnityEngine;
using Object = UnityEngine.Object;

namespace LP
{
    /// <summary>
    /// 文件日志辅助类
    /// </summary>
    public static class Logger
    {
        // NeedStop
        private static bool mNeedStop = false;
        // Writer
        private static StreamWriter mWriter = null;
        // 消息对象队列
        //    private static LocklessQueue<LogMessage> mMessageQueue = new LocklessQueue<LogMessage>();
        private static Queue<LogMessage> mMessageQueue = new Queue<LogMessage>();

        #region 日志消息
        private sealed class LogMessage
        {
            /// <summary>
            /// 标签
            /// </summary>
            public string Tag
            {
                get; private set;
            }
            /// <summary>
            /// 日志类型
            /// </summary>
            public LogType Type
            {
                get; private set;
            }
            /// <summary>
            /// 日志内容
            /// </summary>
            public object Message
            {
                get; private set;
            }
            /// <summary>
            /// StackTrace
            /// </summary>
            public string Trace
            {
                get; private set;
            }
            // 当前是否被使用
            private bool mIsUsed = false;
            // 池
            private static List<LogMessage> mPoolList = new List<LogMessage>( 10 );

            private LogMessage()
            {
                mIsUsed = true;
            }

            /// <summary>
            /// 清理对象池
            /// </summary>
            public static void Clear()
            {
                foreach(LogMessage msg in mPoolList)
                {
                    msg.Recycle();
                }
                mPoolList.Clear();
            }

            /// <summary>
            /// 获取一个消息对象,为了减少对象的开辟，使用对象池
            /// </summary>
            public static LogMessage Acquire( string tag, LogType type, object message, string stackTrace )
            {
                LogMessage log = null;
                // 优先尝试从对象池中查找已Recycle过的对象
                foreach(LogMessage msg in mPoolList)
                {
                    if(!msg.mIsUsed)
                    {
                        log = msg;
                        log.mIsUsed = true;
                        break;
                    }
                }
                if(log == null)
                {
                    // 构建新对象并加入对象池
                    log = new LogMessage();
                    mPoolList.Add( log );
                }
                // 赋值
                log.Tag = tag;
                log.Type = type;
                log.Message = message;
                log.Trace = stackTrace;
                return log;
            }

            /// <summary>
            /// 释放,让对象可以循环使用
            /// </summary>
            public void Recycle()
            {
                mIsUsed = false;
                Tag = null;
                Message = null;
                Type = LogType.Log;
            }

        }
    }
    #endregion
}
