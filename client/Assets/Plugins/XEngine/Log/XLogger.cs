using System;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
using System.Diagnostics;
#endif

namespace XEngine
{
    public sealed class XLogger
    {
        // 输出日志到Console，放在这里是为了尽量减少OnOpenAsset中对line的修改
        private void LogToConsole(XLogLevel level, string message)
        {
#if UNITY_EDITOR
            _listStackFrames.Add(AnalyzeStackFrame());
#endif
            // 包装消息的Tag、颜色、日期等信息
            message = WrapMessage(level, message);
            switch (level)
            {
                case XLogLevel.Debug:
                case XLogLevel.Info:
                    UnityEngine.Debug.Log(message);
                    break;
                case XLogLevel.Warn:
                    UnityEngine.Debug.LogWarning(message);
                    break;
                case XLogLevel.Error:
                    UnityEngine.Debug.LogError(message);
                    break;
                case XLogLevel.Exception:
                    UnityEngine.Debug.LogError(message);
                    break;
            }
        }


        /// <summary>
        /// 获取该日志器的名称(唯一)
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// 获取或设置该日志器是否可用
        /// </summary>
        public bool Enable { get; set; }
        /// <summary>
        /// 获取或设置该日志器的最低日志等级
        /// </summary>
        public XLogLevel Level { get; set; }
        /// <summary>
        /// 屏幕日志功能是否可用
        /// </summary>
        public bool EnableLogToScreen { get; set; }



        private XLogger(string name)
        {
            Enable = true;
            Level = XLogLevel.Debug;
        }

        public bool IsLogTypeAllowed(XLogLevel level)
        {
            return level >= Level;
        }

        public void Debug(string message)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Debug))
            {
                Log(XLogLevel.Debug, message);
            }
        }

        public void Debug(string format, params object[] args)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Debug))
            {
                Log(XLogLevel.Debug, string.Format(format, args));
            }
        }

        public void Info(string message)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Info))
            {
                Log(XLogLevel.Info, message);
            }
        }

        public void Info(string format, params object[] args)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Info))
            {
                Log(XLogLevel.Info, string.Format(format, args));
            }
        }

        public void Warn(string message)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Warn))
            {
                Log(XLogLevel.Warn, message);
            }
        }

        public void Warn(string format, params object[] args)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Warn))
            {
                Log(XLogLevel.Warn, string.Format(format, args));
            }
        }

        public void Error(string message)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Error))
            {
                Log(XLogLevel.Error, message);
            }
        }

        public void Error(string format, params object[] args)
        {
            if (Enable && IsLogTypeAllowed(XLogLevel.Error))
            {
                Log(XLogLevel.Error, string.Format(format, args));
            }
        }

        public void Exception(Exception ex)
        {
            if (Enable)
                Log(XLogLevel.Exception, ex.Message);
        }

        // 输出日志
        private void Log(XLogLevel level, string message)
        {
            if (Enable)
            {
                LogToConsole(level, message);
                LogToScreen(level, message);
                LogToFile(level, message);
            }
        }

        // 输出日志到屏幕
        private void LogToScreen(XLogLevel level, string message)
        {
            if (EnableLogToScreen)
            {
                message = WrapMessage(level, message);
                SettingsForScreenLog.AppendLog(message);
            }
        }

        // 输出日志到File
        private void LogToFile(XLogLevel level, string message)
        {

        }

        // 给日志包装颜色显示
        private string WrapMessage(XLogLevel level, string message)
        {
            string color;
            switch (level)
            {
                case XLogLevel.Debug:
                    color = "#0070BB";
                    break;
                case XLogLevel.Warn:
                    color = "#BBBB23";
                    break;
                case XLogLevel.Error:
                    color = "#FF0006";
                    break;
                case XLogLevel.Exception:
                    color = "#8F0005";
                    break;
                default:
                    color = "#48BB31";
                    break;
            }
            return string.Format("<color={0}>[{1}]{2}</color>", color, Name, message);
        }



        /// <summary>
        /// 屏幕日志设置
        /// </summary>
        public static ScreenLogSettings SettingsForScreenLog { get; private set; }

        // 缓存所有的logger
        private static Dictionary<string, XLogger> mLoggers = new Dictionary<string, XLogger>(new LoggerNameComparer());

        public static XLogger Get()
        {
            return Get(null);
        }

        public static XLogger Get(string name)
        {
            // 默认Logger
            if (string.IsNullOrEmpty(name))
                name = "ROOT";
            name = name.ToUpper();
            XLogger logger = null;
            if (!mLoggers.TryGetValue(name, out logger))
            {
                logger = new XLogger(name);
                mLoggers.Add(name, logger);
            }
            return logger;
        }

        private static void RenderScreenLog()
        {

        }

        // 实现Dictionary Key的有序忽略大小比较
        private class LoggerNameComparer : IEqualityComparer<string>
        {
            public bool Equals(string x, string y)
            {
                return string.Equals(x, y, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(string obj)
            {
                return obj.ToLower().GetHashCode();
            }
        }

#if UNITY_EDITOR
        private static int _instanceID;
        private static List<StackFrame> _listStackFrames = new List<StackFrame>();
        // Console Window
        private static object _consoleWindow;
        private static object _logListView;
        private static FieldInfo _logListViewTotalRows;
        private static FieldInfo _logListViewCurrentRow;
        // LogEntry
        private static MethodInfo _logEntriesGetEntry;
        private static object _logEntry;
        //instanceId 非UnityEngine.Object的运行时 InstanceID 为零所以只能用 LogEntry.Condition 判断  
        private static FieldInfo _logEntryCondition;

        static XLogger()
        {
            // 如果类文件改动了,记得修改
            _instanceID = AssetDatabase.LoadAssetAtPath<MonoScript>("Assets/Plugins/XEngine/Log/XLogger.cs").GetInstanceID();
            _listStackFrames.Clear();
            CheckConsoleWindowListView();
        }

        private static bool CheckConsoleWindowListView()
        {
            if (_logListView == null)
            {
                Assembly unityEditorAssembly = Assembly.GetAssembly(typeof(EditorWindow));
                // ConsoleWindow
                {
                    Type consoleWindowType = unityEditorAssembly.GetType("UnityEditor.ConsoleWindow");
                    _consoleWindow = consoleWindowType.GetField("ms_ConsoleWindow", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);

                    FieldInfo listViewFieldInfo = consoleWindowType.GetField("m_ListView", BindingFlags.Instance | BindingFlags.NonPublic);
                    _logListView = listViewFieldInfo.GetValue(_consoleWindow);
                    _logListViewTotalRows = listViewFieldInfo.FieldType.GetField("totalRows", BindingFlags.Instance | BindingFlags.Public);
                    _logListViewCurrentRow = listViewFieldInfo.FieldType.GetField("row", BindingFlags.Instance | BindingFlags.Public);
                }
                //LogEntries  
                {
                    Type logEntriesType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntries");
                    _logEntriesGetEntry = logEntriesType.GetMethod("GetEntryInternal", BindingFlags.Static | BindingFlags.Public);
                    Type logEntryType = unityEditorAssembly.GetType("UnityEditorInternal.LogEntry");
                    _logEntry = Activator.CreateInstance(logEntryType);
                    _logEntryCondition = logEntryType.GetField("condition", BindingFlags.Instance | BindingFlags.Public);
                }
            }
            return _logListView != null;
        }

        private static StackFrame GetListViewRowCount()
        {
            if (CheckConsoleWindowListView())
            {
                int totalRows = (int)_logListViewTotalRows.GetValue(_logListView);
                int row = (int)_logListViewCurrentRow.GetValue(_logListView);
                int logByThisClassCount = 0;
                for (int i = totalRows - 1; i >= row; i--)
                {
                    _logEntriesGetEntry.Invoke(null, new object[] { i, _logEntry });
                    string condition = _logEntryCondition.GetValue(_logEntry) as string;
                    //判断是否是由LoggerUtility打印的日志  
                    if (condition.Contains("XEngine.XLogger"))
                        logByThisClassCount++;
                }
                //同步日志列表，ConsoleWindow 点击Clear 会清理  
                while (_listStackFrames.Count > totalRows)
                    _listStackFrames.RemoveAt(0);
                if (_listStackFrames.Count >= logByThisClassCount)
                    return _listStackFrames[_listStackFrames.Count - logByThisClassCount];
            }
            return null;
        }

        [UnityEditor.Callbacks.OnOpenAsset(0)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            // 如果对XLogg进行改动,那么要记得修改line的值
            if (instanceID == _instanceID && (line == 26 || line == 29 || line == 32 || line == 35))
            {
                StackFrame sf = GetListViewRowCount();
                if (sf != null)
                {
                    string fileName = sf.GetFileName();
                    string fileAssetPath = fileName.Substring(fileName.IndexOf("Assets"));
                    AssetDatabase.OpenAsset(AssetDatabase.LoadAssetAtPath<MonoScript>(fileAssetPath), sf.GetFileLineNumber());
                    return true;
                }
            }
            return false;
        }


        // 分析调用Logger的第一层外堆栈
        private static StackFrame AnalyzeStackFrame()
        {
            StackTrace st = new StackTrace(true);
            StackFrame sf = st.GetFrame(0);
            for (int i = 1; i < st.FrameCount; i++)
            {
                StackFrame f = st.GetFrame(i);
                if (!f.GetFileName().EndsWith(@"\XLogger.cs"))
                {
                    sf = f;
                    break;
                }
            }
            return sf;
        }
#endif
    }
}
