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
    public static class MLogger
    {
        // NeedStop
        private static bool mNeedStop = false;
        // Writer
        private static StreamWriter mWriter = null;
        // 消息对象队列
        //    private static LocklessQueue<LogMessage> mMessageQueue = new LocklessQueue<LogMessage>();
        private static Queue<LogMessage> mMessageQueue = new Queue<LogMessage>();

        /// <summary>
        /// 初始化日志工具
        /// </summary>
        public static void Initialize()
        {
            mNeedStop = false;
            if(mWriter == null)
            {
                string logPath = Path.Combine( Settings.UNITY_LOG_FOLDER, string.Concat( Engine.GetNowString(), ".log" ) );

                foreach(string item in Directory.GetFiles( Settings.UNITY_LOG_FOLDER, "*.log", SearchOption.TopDirectoryOnly ))
                {
                    try
                    {
                        if(new FileInfo( item ).Length == 0)
                        {
                            File.Delete( item );
                            continue;
                        }
                        string shortname = Path.GetFileNameWithoutExtension( item );
                        DateTime dt = DateTime.ParseExact( shortname, "yyyy-MM-dd HH-mm-ss", null );
                        TimeSpan span = DateTime.Now - dt;
                        if(span.TotalDays >= 2)
                            File.Delete( item );
                    }
                    catch(Exception)
                    {
                        try
                        {
                            File.Delete( item );
                        }
                        catch { }
                    }
                }

                mWriter = new StreamWriter( logPath, false, Encoding.UTF8 );
                mWriter.AutoFlush = true;
            }

            // 每次启动的时候多拉几行空格，便于阅读
            if(mWriter.BaseStream.Position > 0L)
            {
                for(int i = 0; i < 10; i++)
                {
                    mWriter.WriteLine();
                }
            }

            Application.logMessageReceivedThreaded += OnLogCallback;

            //开辟一个线程来执行LogMessage
            Thread logThread = new Thread( LogMessageHandler, 5 );
        }

        private static void LogMessageHandler()
        {
            while(true)
            {
                LogMessage message = null;
                
            }
        }

        private static void OnLogCallback( string condition, string stackTrace, LogType type )
        {
            // 写入到文件中
            mWriter.WriteLine( condition );
            if(type != LogType.Log && !string.IsNullOrEmpty( stackTrace ))
                mWriter.WriteLine( stackTrace );
        }

        /// <summary>
        /// 输出Engine模块的Debug信息
        /// </summary>
        internal static void LogEngine( string format, params object[] args )
        {
            LogEngine( string.Format( format, args ) );
        }

        /// <summary>
        /// 输出Engine模块的Debug信息
        /// </summary>
        public static void LogEngine( string message )
        {
            Log( "Engine", LogType.Log, message );
        }

        public static void Log( string tag, LogType type, object message )
        {
            // 非Debug模式不输出
            if(!Settings.Debug)
                return;
            // 直接用Debug.Log会造成较大的CPU消耗,所以用线程来输出日志
            string stackTrace = null;
            if(message is Exception)
                stackTrace = StackTraceUtility.ExtractStringFromException( message );
            else
                stackTrace = StackTraceUtility.ExtractStackTrace();
            mMessageQueue.Enqueue( LogMessage.Acquire( tag, type, message, stackTrace ) );
        }


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
        #endregion
    }
}
