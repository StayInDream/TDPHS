using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace LP
{
    /// <summary>
    /// 游戏驱动
    /// </summary>
    public class Engine : BaseBehaviour
    {


        public static Engine Inst
        {
            get; private set;
        }

        void Awake()
        {

        }

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /// <summary>
        /// 获得现在时间
        /// </summary>
        /// <returns></returns>
        internal static string GetNowString()
        {
            return DateTime.Now.ToString( "yyyy-MM-dd HH-mm-ss" );
        }
    }
}
