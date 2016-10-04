using UnityEngine;
using System.Collections.Generic;

namespace XEngine
{
    public class ScreenLogSettings
    {
        /// <summary>
        /// 显示的区域
        /// </summary>
        public Rect area = new Rect(0, 0, 400, 400);
        /// <summary>
        /// 最大记录条数
        /// </summary>
        public int maxNum = 50;
        /// <summary>
        /// 快捷键,显示/隐藏屏幕日志
        /// </summary>
        public KeyCode hostKey = KeyCode.U;
        /// <summary>
        /// 当前是否处于显示状态
        /// </summary>
        public bool visible = true;
        /// <summary>
        /// 日志缓存
        /// </summary>
        public List<string> logs = new List<string>(50);


        public void AppendLog(string log)
        {
            if (logs.Count >= maxNum)
            {
                for (int i = 0, len = maxNum - logs.Count; i < len; i++)
                {
                    logs.RemoveAt(i);
                }
            }
            logs.Add(log);
        }

        public void Clear()
        {
            logs.Clear();
        }
    }
}
