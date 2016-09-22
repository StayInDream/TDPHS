using System;
using System.Threading;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LP
{
    /// <summary>
    /// 游戏驱动
    /// </summary>
    public class Engine : BaseBehaviour
    {
        private static int mainContextID;
        private static BaseManager[] mangers = null;
        private int num_manager = 6;

        private int num_current = 0;

        public static Engine Inst
        {
            get; private set;
        }

        public static Engine StartEngine( GameObject go )
        {
            if(Inst == null)
            {
                Inst = go.AddComponent<Engine>();
                DontDestroyOnLoad( Inst );
            }
            return Inst;
        }

        void Awake()
        {
            Inst = this;
            // 获取当前线程上下文ID
            mainContextID = Thread.CurrentContext.ContextID;

            mangers = new BaseManager[num_manager];
            mangers[0] = new WindowAlertExecutor();
            mangers[1] = new DataManager();
            mangers[2] = new NetManager();
            mangers[3] = new AssetManager();
            mangers[4] = new SoundManager();
            mangers[5] = new AlarmManager();

            // 初始化日志
            MLogger.Initialize();

            SendAwake();
        }

        /// <summary>
        /// 启动所有BaseManager的子类的 Awake
        /// </summary>
        private void SendAwake()
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnAwake();
            }
        }

        // Use this for initialization
        void Start()
        {
            Time.timeScale = 1;
            MLogger.LogEngine( "==========Engine Start At {0}==========", GetNowString() );

            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnStart();
            }
        }

        // Update is called once per frame
        void Update()
        {

        }


        /// <summary>
        /// 获得现在时间
        /// </summary>
        /// <returns></returns>
        public static string GetNowString()
        {
            return DateTime.Now.ToString( "yyyy-MM-dd HH-mm-ss" );
        }

    }
}
