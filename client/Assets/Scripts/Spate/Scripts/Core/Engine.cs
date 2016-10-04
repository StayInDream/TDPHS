using System;
using System.Threading;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Jimmy;
using FairyGUI;

namespace Spate
{
    public sealed class Engine : BaseBehaviour
    {
        private static int mainContextID;

        private static int num_current = 0;
        private static int num_manager = 0;
        private static BaseManager[] mangers = null;
     

        public static Engine StartEngine( GameObject go )
        {
            if(Inst == null)
            {
                Inst = go.AddComponent<Engine>();
                DontDestroyOnLoad( Inst );
            }
            return Inst;
        }

        public static Engine Inst
        {
            get; private set;
        }

        void Awake()
        {
            UIObjectFactory.SetLoaderExtension( typeof( GLoader ) );
            FontManager.Clear();

            Inst = this;
            // 获取当前线程上下文ID
            mainContextID = Thread.CurrentContext.ContextID;
            // 实例化所有的Manager
            num_manager = 8;
            mangers = new BaseManager[num_manager];
            mangers[0] = new WindowManager();
            mangers[1] = new DataManager();
            mangers[2] = new NetManager();
            mangers[3] = new AssetManager();
            mangers[4] = new SoundManager();
            mangers[5] = new AlarmManager();
            mangers[6] = new AsyncManager();
            mangers[7] = new WindowAlertExecutor();
            // 初始化日志
            MLogger.Initialize();
           
            SendAwake();
        }

        void SendAwake()
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnAwake();
            }
        }

       
        void Start()
        {
            Time.timeScale = 1;
            Application.targetFrameRate = 30;
            GRoot.inst.SetContentScaleFactor( 1136, 640 );
            WarmupAllShaders();
            MLogger.LogEngine( "==========Engine Start At {0}==========", GetNowString() );
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnStart();
            }
        }

        void Update()
        {
            switch(mUpdateMode)
            {
                case eUpdateMode.Update:
                    {
                        for(num_current = 0; num_current < num_manager; num_current++)
                        {
                            mangers[num_current].OnUpdate();
                        }
                    }
                    break;
                case eUpdateMode.Wait:
                    {
                        mUpdateMode = eUpdateMode.Awake;
                    }
                    break;
                case eUpdateMode.Awake:
                    {
                        SendAwake();
                        mUpdateMode = eUpdateMode.Start;
                    }
                    break;
                case eUpdateMode.Start:
                    {
                        Start();
                        if(mOnResetFinish != null)
                        {
                            mOnResetFinish();
                            mOnResetFinish = null;
                        }
                        mUpdateMode = eUpdateMode.Update;
                    }
                    break;
            }

#if UNITY_EDITOR
            if(Input.GetKeyUp( KeyCode.C ))
            {
                Resources.UnloadUnusedAssets();
            }
            //else if (Input.GetKeyUp(KeyCode.Alpha1))
            //{
            //    QualitySettings.SetQualityLevel((int)QualityLevel.Fastest, true);
            //    Application.targetFrameRate = 20;
            //}
            //else if (Input.GetKeyUp(KeyCode.Alpha2))
            //{
            //    QualitySettings.SetQualityLevel((int)QualityLevel.Simple, true);
            //    Application.targetFrameRate = 30;
            //}
            //else if (Input.GetKeyUp(KeyCode.Alpha3))
            //{
            //    QualitySettings.SetQualityLevel((int)QualityLevel.Beautiful, true);
            //    Application.targetFrameRate = 60;
            //}
#endif
        }

        void LateUpdate()
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnLateUpdate();
            }
        }

        void FixedUpdate()
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnFixedUpdate();
            }
        }

        void OnApplicationPause( bool pause )
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnApplicationPause( pause );
            }
        }

        void OnApplicationQuit()
        {
            MLogger.LogEngine( "==========App Quit At {0}==========", GetNowString() );
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnApplicationQuit();
            }

            Time.timeScale = 1;
        }

        void OnDestroy()
        {
            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnDestroy();
            }
            // 关闭文件日志
            MLogger.Dispose();
        }

        // 引发Engine所有的Manager进行重置
        public static void Reset( Action onResetFinish )
        {
            SuperSDK.IsLoginReady = false;
            SuperSDK.CacheUser = null;

            MLogger.LogEngine( "==========Engine Reset At {0}==========", GetNowString() );
            Notifier.Unreg();
            SettingsView settings = Inst.GetComponent<SettingsView>();
            if(settings != null)
                GameObject.Destroy( settings );
            //  HeartBeater beater = Inst.GetComponent<HeartBeater>();
            //if (beater != null)
            //    GameObject.Destroy(beater);
            //EffectManager effectManager = Inst.GetComponent<EffectManager>();
            //if (effectManager != null)
            //    effectManager.ClearAll();
            //ChatService chatService = Inst.GetComponent<ChatService>();
            //if (chatService != null)
            //    chatService.StopService();

            for(num_current = 0; num_current < num_manager; num_current++)
            {
                mangers[num_current].OnReset();
            }

            mOnResetFinish = onResetFinish;
            mUpdateMode = eUpdateMode.Wait;
        }

        // 检测当前是否属于主线程环境
        public static bool IsMainContext()
        {
            return mainContextID == Thread.CurrentContext.ContextID;
        }

        public static string GetNowString()
        {
            return DateTime.Now.ToString( "yyyy-MM-dd HH-mm-ss" );
        }

        // 1表示需要等待一帧,2表示需要重新调用Start,3表示回复Update
        private static eUpdateMode mUpdateMode = eUpdateMode.Update;
        private static Action mOnResetFinish;

        private enum eUpdateMode
        {
            Update,
            Wait,
            Awake,
            Start
        }

        private static void WarmupAllShaders()
        {
            Shader.Find( "Custom/Diffuse" );
            Shader.Find( "Custom/Diffuse(Gloss)" );
            Shader.Find( "Custom/Diffuse(UnFog)" );
            Shader.Find( "Custom/DiffuseEditor" );

            Shader.Find( "Custom/Transparent/Diffuse" );
            Shader.Find( "Custom/Transparent/Diffuse(Gloss)" );
            Shader.Find( "Custom/Transparent/Diffuse(UnFog)" );
            Shader.Find( "Custom/Transparent/DiffuseEditor" );

            Shader.Find( "Custom/Transparent/Cutout/Diffuse" );
            Shader.Find( "Custom/Transparent/Cutout/Diffuse(Gloss)" );
            Shader.Find( "Custom/Transparent/Cutout/Diffuse(UnFog)" );
            Shader.Find( "Custom/Transparent/Cutout/Diffuse(Cull)" );
            Shader.Find( "Custom/Transparent/Cutout/DiffuseEditor" );

            Shader.WarmupAllShaders();
        }
    }
}