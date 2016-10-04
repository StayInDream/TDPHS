using System;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using FairyGUI;
using UnityEngine;

namespace Spate
{
    /// <summary>
    /// 窗口管理,提供打开窗口，关闭窗口，窗口的层级、进退栈维护
    /// </summary>
    /// 
    internal class WindowManager : BaseManager
    {
        // 所有已经打开过的窗口
        private static List<Window> mAllWindows = new List<Window>();
        // 已经打开的壁纸层窗口
        private static List<Window> mWallpaperWindows = new List<Window>();
        // 已经打开的普通UI层窗口
        private static List<Window> mNormalWindows = new List<Window>();
        // 已经打开的普通对话框(弹出框)窗口
        private static List<Window> mDialogWindows = new List<Window>();
        // 已经打开的系统警告窗口
        private static List<Window> mAlertWindows = new List<Window>();

        // 当前处于打开状态的窗口

        private static List<Window> mActiveWindows = new List<Window>();
        //
        public static Dictionary<string, int> listShouldOpenWindow = new Dictionary<string, int>();

        public static Dictionary<WindowLayer, UIPanel> dic_mUIPanels = new Dictionary<WindowLayer, UIPanel>();

        private static StageCamera mStageCamera;
        private static GameObject mRoot = null;
        //当前UIPanel显示的包
        private static string CuruiPackage;
        private static UIPanel mUIPanel;
        private static GComponent mainView;

        public override void OnAwake()
        {
            mRoot = GameObject.FindObjectOfType<Launcher>().gameObject;
            mUIPanel = GameObject.FindObjectOfType<UIPanel>();
            mStageCamera = GameObject.FindObjectOfType<StageCamera>();
            GameObject.DontDestroyOnLoad( mRoot );
            UIPackage.AddPackage( Settings.UIPACKAGE_WIN_FOLDER + "主界面" );
         //   UIPackage.AddPackage( Settings.UIPACKAGE_WIN_FOLDER + "开始窗口" );

        }

        public override void OnReset()
        {
            if(mRoot != null)
                GameObject.Destroy( mRoot );
            mRoot = null;
            mUIPanel = null;
            mainView = null;
            CuruiPackage = null;
            mAllWindows.Clear();
            mAlertWindows.Clear();
            mDialogWindows.Clear();
            mNormalWindows.Clear();
            mWallpaperWindows.Clear();
            mActiveWindows.Clear();
            listShouldOpenWindow.Clear();
            dic_mUIPanels.Clear();
            mStageCamera = null;
        }

        public static UIPanel MUIPanel
        {
            get
            {
                return mUIPanel;
            }
        }

        /// <summary>
        /// UIPanel下的界面的主容器，增删元件调用 .AddChild(GComponent)等等进行添加。
        /// </summary>
        public static GComponent MainView
        {
            get
            {
                return mainView;
            }
        }
        /// <summary>
        /// UIPanel下的packageName，也就是当前显示的包名。
        /// </summary>
        public static string CurPackname
        {
            get
            {
                return CuruiPackage;
            }
        }

        /// <summary>
        /// 窗口的主界面显示,切换包 请调用此函数,否则请在要显示的 xxxWindow的OnInit中调用contenPanel = UIPackage.CreateObject方法添加组件显示
        /// </summary>
        /// <param name="packageName">包名</param>
        /// <param name="main">要显示的主ui名，既窗口的整体框架，其他组件的载体</param>
        /// <param name="layer">层</param>
        public static void SetMainView( string packageName, string main )
        {
            if(string.IsNullOrEmpty( packageName ))
            {
                MLogger.LogError( "包名不能为null或者空！" );
                return;
            }
            UIPackage package = UIPackage.GetByName( packageName );
            if(package == null)
            {
                UIPackage.AddPackage( Settings.UIPACKAGE_WIN_FOLDER + packageName );
            }

            mUIPanel.packageName = packageName;
            mUIPanel.componentName = main;
            mUIPanel.CreateUI();
            mainView = mUIPanel.ui;

            if(mainView != null)
            {
                mainView.SetSize( GRoot.inst.width, GRoot.inst.height );
                mainView.AddRelation( GRoot.inst, RelationType.Size );
            }
            CuruiPackage = packageName;
        }

        public static bool GlobalInput
        {
            get;
            private set;
        }

        public static void OpenShouldWindow( bool isStartGame = false )
        {

            listShouldOpenWindow.Clear();
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public static T OpenWindow<T>( params object[] args ) where T : Window
        {
            Window win = null;
            // string winName = typeof( T ).Name;
            ConstructorInfo[] infos = typeof( T ).GetConstructors();
            win = infos[0].Invoke( args ) as T;

            if(!mActiveWindows.Contains( win ))
            {
                mActiveWindows.Add( win );
            }
            if(!mAllWindows.Contains( win ))
            {
                mAllWindows.Add( win );
            }
            
            win.OnAwake();
            RefreshDepth( win );
            return win as T;
        }


        public static T CloseWindow<T>( bool isDispose = false, params object[] args ) where T : Window
        {
            Window win = null;
            string winName = typeof( T ).Name;
            ConstructorInfo[] infos = typeof( T ).GetConstructors();
            win = infos[0].Invoke( args ) as T;

            CloseWindow( win, isDispose );
            return win as T;
        }

        public static void CloseWindow( Window win, bool isDispose = false )
        {
            if(win != null)
            {
                if(mActiveWindows.Contains( win ))
                {
                    mActiveWindows.Remove( win );
                }
                if(isDispose)
                {
                    win.isDispoes = true;
                    if(mAlertWindows.Contains( win ))
                        mAlertWindows.Remove( win );
                }
                win.Hide();
            }
        }


        private static void RefreshDepth( Window window )
        {
            if(window == null)
                return;

            switch(window.Layer)
            {
                case WindowLayer.Alert:
                    break;
                case WindowLayer.Dialog:
                    break;
                case WindowLayer.Wallpaper:
                    break;
                default:

                    break;
            }
        }


        /// <summary>
        /// 查找窗口
        /// </summary>
        public static T FindWindow<T>() where T : Window
        {
            Window window = null;
            ConstructorInfo[] infos = typeof( T ).GetConstructors();
            Window win = infos[0].Invoke( null ) as T;
            if(mAllWindows.Contains( win ))
                window = win;

            return window as T;
        }

        /// <summary>
        /// 通知本地时间已更新
        /// </summary>
        private void NotifyTimeIsChange()
        {
            Notifier.Notify( GlobalUtil.LOCAL_TIME_CHANGE );
        }

        /// <summary>
        /// 尝试根据类名推导出真实的Type
        /// </summary>
        private static Type TryFindClassType( string className )
        {
            Type t = null;
            // login -> LoginWindow
            // 1,首字母大写
            string inferName = className.ToUpper( 0, 1 );
            // 2,尾缀添加Csv
            if(!inferName.EndsWith( "Window" ))
                inferName = inferName + "Window";
            // 根据名称查找类型(考虑垮程序集查找的问题)
            t = Type.GetType( inferName, false, false );
            if(t == null)
            {
                string assemblyQualifiedName = string.Format( "{0}, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null", inferName );
                t = Type.GetType( assemblyQualifiedName );
            }
            if(t == null)
                throw new Exception( string.Format( "推导出的类型{0}不存在", inferName ) );
            return t;
        }
    }
}