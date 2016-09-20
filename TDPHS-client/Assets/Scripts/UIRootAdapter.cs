﻿using UnityEngine;
using System.Collections;

namespace LP
{
    /// <summary>
    /// UI Root适配器
    /// </summary>
    [RequireComponent( typeof( UIRoot ) )]
    [ExecuteInEditMode()]
    public class UIRootAdapter : MonoBehaviour
    {
        private UIRoot mRoot;
        // 基准宽高比
        private static float Base_Aspect = 1.775f;
        private float mCurAspect;
        private float mOldAspect;

        // Use this for initialization
        void Start()
        {
            mRoot = GetComponent<UIRoot>();
            if(mRoot == null)
            {
                Destroy( this );
                return;
            }

            //计算基准宽高比
            Base_Aspect = mRoot.manualWidth * 1.0f / mRoot.manualHeight;
            // 手机平台上RunOnce
            if(Application.platform == RuntimePlatform.Android)
            {
                Update();
                Destroy( this );
            }
        }

        // Update is called once per frame
        void Update()
        {
            mCurAspect = Screen.width * 1.0f / Screen.height;
            if(mCurAspect != mOldAspect)
            {
                if(mCurAspect < Base_Aspect)
                {
                    mRoot.fitWidth = true;
                    mRoot.fitHeight = false;
                }
                else
                {
                    mRoot.fitWidth = false;
                    mRoot.fitHeight = true;
                }
                mOldAspect = mCurAspect;
            }
        }
    }
}
