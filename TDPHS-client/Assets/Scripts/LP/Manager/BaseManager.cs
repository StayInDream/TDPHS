﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace LP
{
    /// <summary>
    /// 管理器基类
    /// </summary>
   public abstract class BaseManager
    {
        /// <summary>
        /// 管理器唤醒
        /// </summary>
        public virtual void OnAwake()
        {

        }

        /// <summary>
        /// 启动回调
        /// </summary>
        public virtual void OnStart()
        {

        }

        /// <summary>
        /// 逐帧更新
        /// </summary>
        public virtual void OnUpdate()
        {

        }

        public virtual void OnLateUpdate()
        {

        }

        public virtual void OnFixedUpdate()
        {

        }

        public virtual void OnApplicationPause( bool pause )
        {

        }

        public virtual void OnApplicationQuit()
        {

        }

        /// <summary>
        /// 重置数据
        /// </summary>
        public virtual void OnReset()
        {

        }

        /// <summary>
        /// 销毁数据
        /// </summary>
        public virtual void OnDestroy()
        {

        }
    }
   
}
