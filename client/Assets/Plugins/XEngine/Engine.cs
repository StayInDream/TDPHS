using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;

namespace XEngine
{
    public sealed class Engine : XBehaviour
    {
        public event UnityAction OnStartEvent;
        public event UnityAction OnStopEvent;

        public event UnityAction OnPauseEvent;
        public event UnityAction OnResumeEvent;

        public event UnityAction OnPreUpdateEvent;
        public event UnityAction OnUpdateEvent;
        public event UnityAction OnLateUpdateEvent;
        public event UnityAction OnFixedUpdateEvent;

        public event UnityAction OnGUIEvent;


        public static Engine Start(GameObject go)
        {
            if (Inst == null)
                go.AddComponent<Engine>();
            return Inst;
        }

        public static Engine Inst { get; private set; }

        void Awake()
        {
            if (Inst != null)
            {
                XLogger.Get().Warn("禁止重复实例化Engine");
                Destroy(this);
                return;
            }
            Inst = this;
            DontDestroyOnLoad(CachedGameObject);

            XLogger.Get(null).Debug("DEBUG");
            XLogger.Get(null).Info("INFO");
            XLogger.Get(null).Warn("WARN");
            XLogger.Get(null).Error("ERROR");
            XLogger.Get(null).Exception(new Exception("EX"));
        }

        void Start()
        {
            if (OnStartEvent != null)
                OnStartEvent();
        }

        void Update()
        {
            if (OnPreUpdateEvent != null)
                OnPreUpdateEvent();
            if (OnUpdateEvent != null)
                OnUpdateEvent();
        }

        void LateUpdate()
        {
            if (OnLateUpdateEvent != null)
                OnLateUpdateEvent();
        }

        void FixedUpdate()
        {
            if (OnFixedUpdateEvent != null)
                OnFixedUpdateEvent();
        }

        void OnGUI()
        {
            if (OnGUIEvent != null)
                OnGUIEvent();
        }

        void OnDestroy()
        {
            if (OnStopEvent != null)
                OnStopEvent();
        }

        void OnApplicationPause(bool pause)
        {
            if (pause && OnPauseEvent != null)
                OnPauseEvent();
            else if (!pause && OnResumeEvent != null)
                OnResumeEvent();
        }

        void OnApplicationQuit()
        {

        }
    }
}