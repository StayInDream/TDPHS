using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using DG.Tweening;

namespace FairyGUI
{
    /// <summary>
    /// Window class.
    /// 窗口使用前首先要设置窗口中需要显示的内容，这通常是在编辑器里制作好的，可以直接使用Window.contentPane进行设置。
    /// 建议把设置contentPane等初始化操作放置到Window.onInit方法中。
    /// 另外，FairyGUI还提供了一套机制用于窗口动态创建。动态创建是指初始时仅指定窗口需要使用的资源，等窗口需要显示时才实际开始构建窗口的内容。
    /// 首先需要在窗口的构造函数中调用Window.addUISource。这个方法需要一个IUISource类型的参数，而IUISource是一个接口，
    /// 用户需要自行实现载入相关UI包的逻辑。当窗口第一次显示之前，IUISource的加载方法将会被调用，并等待载入完成后才返回执行Window.OnInit，然后窗口才会显示。
    /// 
    /// 如果你需要窗口显示时播放动画效果，那么覆盖doShowAnimation编写你的动画代码，并且在动画结束后调用onShown。覆盖onShown编写其他需要在窗口显示时处理的业务逻辑。
    /// 如果你需要窗口隐藏时播放动画效果，那么覆盖doHideAnimation编写你的动画代码，并且在动画结束时调用Window.hideImmediately（注意不是直接调用onHide！）。覆盖onHide编写其他需要在窗口隐藏时处理的业务逻辑。
    /// </summary>
    public class Window : GComponent
    {
        /// <summary>
        /// 
        /// </summary>
        public bool bringToFontOnClick
        {
            get; set;
        }

        GComponent _frame;
        GComponent _contentPane;
        GObject _modalWaitPane;
        GObject _closeButton;
        GObject _dragArea;
        GObject _contentArea;
        bool _modal;

        List<IUISource> _uiSources;
        bool _inited;
        bool _loading;

        //LP     
        public WindowLayer _layer;
        public bool isDispoes = false;
        private AnimType _AnimTypeOfOpen = AnimType.None;
        private AnimType _AnimTypeOfClose = AnimType.None;
        //

        protected int _requestingCmd;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"> 默认在普通层</param>
        public Window( WindowLayer layer = WindowLayer.Normal )
            : base()
        {
            _uiSources = new List<IUISource>();
            this.focusable = true;
            bringToFontOnClick = UIConfig.bringWindowToFrontOnClick;

            displayObject.onAddedToStage.Add( __onShown );
            displayObject.onRemovedFromStage.Add( __onHide );
            displayObject.onTouchBegin.AddCapture( __touchBegin );

            //LP
            _layer = layer;

        }

        /// <summary>
        /// Set a UISource to this window. It must call before the window is shown. When the window is first time to show,
        /// UISource.Load is called. Only after all UISource is loaded, the window will continue to init.
        /// 为窗口添加一个源。这个方法建议在构造函数调用。当窗口第一次显示前，UISource的Load方法将被调用，然后只有所有的UISource
        /// 都ready后，窗口才会继续初始化和显示。
        /// </summary>
        /// <param name="source"></param>
        public void AddUISource( IUISource source )
        {
            _uiSources.Add( source );
        }

        public void SetWindowName()
        {
            this.rootContainer.gameObject.name = contentPane.name;
        }


        /// <summary>
        /// Stage下的容器
        /// </summary>
        public GComponent contentPane
        {
            set
            {
                if ( _contentPane != value )
                {
                    if ( _contentPane != null )
                        RemoveChild( _contentPane );
                    _contentPane = value;
                    if ( _contentPane != null )
                    {
                        AddChild( _contentPane );
                        this.SetSize( _contentPane.width , _contentPane.height );
                        _contentPane.AddRelation( this , RelationType.Size );
                        _contentPane.fairyBatching = true;
                        _frame = _contentPane.GetChild( "frame" ) as GComponent;
                        if ( _frame != null )
                        {
                            this.closeButton = _frame.GetChild( "closeButton" );
                            this.dragArea = _frame.GetChild( "dragArea" );
                            this.contentArea = _frame.GetChild( "contentArea" );
                        }
                    }
                    else
                        _frame = null;
                }
            }
            get
            {
                return _contentPane;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GComponent frame
        {
            get
            {
                return _frame;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public GObject closeButton
        {
            get
            {
                return _closeButton;
            }
            set
            {
                if ( _closeButton != null )
                    _closeButton.onClick.Remove( closeEventHandler );
                _closeButton = value;
                if ( _closeButton != null )
                    _closeButton.onClick.Add( closeEventHandler );
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public GObject dragArea
        {
            get
            {
                return _dragArea;
            }
            set
            {
                if ( _dragArea != value )
                {
                    if ( _dragArea != null )
                    {
                        _dragArea.draggable = false;
                        _dragArea.onDragStart.Remove( __dragStart );
                    }

                    _dragArea = value;
                    if ( _dragArea != null )
                    {
                        if ( ( _dragArea is GGraph ) && ( ( GGraph )_dragArea ).displayObject == null )
                            ( ( GGraph )_dragArea ).DrawRect( _dragArea.width , _dragArea.height , 0 , Color.clear , Color.clear );
                        _dragArea.draggable = true;
                        _dragArea.onDragStart.Add( __dragStart );
                    }
                }
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public GObject contentArea
        {
            get
            {
                return _contentArea;
            }
            set
            {
                _contentArea = value;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        public GObject modalWaitingPane
        {
            get
            {
                return _modalWaitPane;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Show()
        {
            GRoot.inst.ShowWindow( this );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        public void ShowOn( GRoot r )
        {
            r.ShowWindow( this );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Hide()
        {
            if ( this.isShowing )
                DoHideAnimation();
        }

        /// <summary>
        /// Hide window immediately, no OnHide will be called.
        /// </summary>
        public void HideImmediately()
        {
            this.root.HideWindowImmediately( this );
        }

        /// <summary>
        /// Make the window be center of the screen.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="restraint">Add relations to ensure keeping center on screen size changed.</param>
        public void CenterOn( GRoot r , bool restraint )
        {
            this.SetXY( ( int )( ( r.width - this.width ) / 2 ) , ( int )( ( r.height - this.height ) / 2 ) );
            if ( restraint )
            {
                this.AddRelation( r , RelationType.Center_Center );
                this.AddRelation( r , RelationType.Middle_Middle );
            }
        }

        /// <summary>
        /// Switch show and hide status.
        /// </summary>
        public void ToggleStatus()
        {
            if ( isTop )
                Hide();
            else
                Show();
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isShowing
        {
            get
            {
                return parent != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool isTop
        {
            get
            {
                return parent != null && parent.GetChildIndex( this ) == parent.numChildren - 1;
            }
        }

        /// <summary>
        /// 遮罩
        /// </summary>
        public bool modal
        {
            get
            {
                return _modal;
            }
            set
            {
                _modal = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void BringToFront()
        {
            this.root.BringToFront( this );
        }

        /// <summary>
        /// 
        /// </summary>
        public void ShowModalWait()
        {
            ShowModalWait( 0 );
        }

        /// <summary>
        /// Display a modal waiting sign in the front.
        /// 显示一个等待标志在最前面。等待标志的资源可以通过UIConfig.windowModalWaiting。等待标志组件会设置为屏幕大小，请内部做好关联。
        /// 还可以设定一个requestingCmd作为等待的命令字，在CloseModalWait里传入相同的命令字ModalWait将结束，否则CloseModalWait无效。
        /// </summary>
        /// <param name="requestingCmd"></param>
        public void ShowModalWait( int requestingCmd )
        {
            if ( requestingCmd != 0 )
                _requestingCmd = requestingCmd;

            if ( UIConfig.windowModalWaiting != null )
            {
                if ( _modalWaitPane == null )
                    _modalWaitPane = UIPackage.CreateObjectFromURL( UIConfig.windowModalWaiting );

                LayoutModalWaitPane();

                AddChild( _modalWaitPane );
            }
        }

        virtual protected void LayoutModalWaitPane()
        {
            if ( _contentArea != null )
            {
                Vector2 pt = _frame.LocalToGlobal( Vector2.zero );
                pt = this.GlobalToLocal( pt );
                _modalWaitPane.SetXY( ( int )pt.x + _contentArea.x , ( int )pt.y + _contentArea.y );
                _modalWaitPane.SetSize( _contentArea.width , _contentArea.height );
            }
            else
                _modalWaitPane.SetSize( this.width , this.height );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CloseModalWait()
        {
            return CloseModalWait( 0 );
        }

        /// <summary>
        /// Close modal waiting. If rquestingCmd is equal to the value you transfer in ShowModalWait, mowal wait will be closed.
        /// Otherwise, this function has no effect.
        /// 关闭模式等待。如果requestingCmd和ShowModalWait传入的不相同，则这个函数没有任何动作，立即返回。
        /// </summary>
        /// <param name="requestingCmd"></param>
        /// <returns></returns>
        public bool CloseModalWait( int requestingCmd )
        {
            if ( requestingCmd != 0 )
            {
                if ( _requestingCmd != requestingCmd )
                    return false;
            }
            _requestingCmd = 0;

            if ( _modalWaitPane != null && _modalWaitPane.parent != null )
                RemoveChild( _modalWaitPane );

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool modalWaiting
        {
            get
            {
                return ( _modalWaitPane != null ) && _modalWaitPane.inContainer;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Init()
        {
            if ( _inited || _loading )
                return;

            if ( _uiSources.Count > 0 )
            {
                _loading = false;
                int cnt = _uiSources.Count;
                for ( int i = 0; i < cnt; i++ )
                {
                    IUISource lib = _uiSources[i];
                    if ( !lib.loaded )
                    {
                        lib.Load( __uiLoadComplete );
                        _loading = true;
                    }
                }

                if ( !_loading )
                    _init();
            }
            else
                _init();
        }

        /// <summary>
        /// 
        /// </summary>
        virtual protected void OnInit()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        virtual protected void OnShown()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        virtual protected void OnHide()
        {
        }

        /// <summary>
        /// 窗口打开动画，若需要请重载
        /// </summary>
        /// <param name="animType">若当前UIPanel下的conmpent Name是当前的窗口名，在DoShowAnimation 参数中动设置画模式，否则，在此方法下的Base.。。中设置动画类型</param>
        //virtual protected void DoShowAnimation( WindowAnimType animType = WindowAnimType.None )
        //{
        //    //  OnShown();
        //    PlayWindowAnim( true, animType );
        //}
        virtual protected void DoShowAnimation()
        {
            //  OnShown();
            PlayAnim( WinAnimTypeOfOpen , OnShown );
        }

        /// <summary>      
        /// 窗口关闭动画,若需要请重载         
        /// </summary>
        /// <param name="animType">若当前UIPanel下的conmpent Name是当前的窗口名，在DoHideAnimation 参数中设置动画模式，否则，在此方法下的Base.。。中设置动画类型 </param>
        virtual protected void DoHideAnimation()
        {
            //this.HideImmediately();
            PlayAnim( WinAnimTypeOfClose , HideImmediately );
        }

        void __uiLoadComplete()
        {
            int cnt = _uiSources.Count;
            for ( int i = 0; i < cnt; i++ )
            {
                IUISource lib = _uiSources[i];
                if ( !lib.loaded )
                    return;
            }

            _loading = false;
            _init();
        }

        void _init()
        {
            _inited = true;
            OnInit();

            if ( this.isShowing )
                DoShowAnimation();
        }

        override public void Dispose()
        {
            if ( _modalWaitPane != null && _modalWaitPane.parent == null )
                _modalWaitPane.Dispose();
            base.Dispose();
        }

        protected void closeEventHandler()
        {
            Hide();
        }

        void __onShown()
        {
            if ( !_inited )
                Init();
            else
                DoShowAnimation();
        }

        void __onHide()
        {
            CloseModalWait();
            OnHide();
        }

        private void __touchBegin( EventContext context )
        {
            if ( this.isShowing && bringToFontOnClick )
            {
                BringToFront();
            }
        }

        private void __dragStart( EventContext context )
        {
            context.PreventDefault();

            this.StartDrag( ( int )context.data );
        }

        //LP




        /// <summary>
        /// 窗口所在层
        /// </summary>
        public WindowLayer Layer
        {
            get
            {
                return _layer;
            }
        }

        /// <summary>
        /// 窗口的打开动画类型
        /// </summary>
        public AnimType WinAnimTypeOfOpen
        {
            set
            {
                _AnimTypeOfOpen = value;
            }
            get
            {
                return _AnimTypeOfOpen;
            }
        }

        /// <summary>
        /// 窗口的关闭动画类型
        /// </summary>
        public AnimType WinAnimTypeOfClose
        {
            set
            {
                _AnimTypeOfClose = value;
            }
            get
            {
                return _AnimTypeOfClose;
            }
        }

        #region 窗口动画
        /// <summary>
        /// 简单的窗口动画
        /// </summary>
        /// <param name="openOrclose">true为播放打开动画</param>
        /// <param name="win">窗口</param>
      /*  private void PlayWindowAnim( bool openOrclose )
        {

            if(openOrclose)
            {             
                switch(_AnimTypeOfOpen)
                {
                    case WindowAnimType.None:
                        OnShown();
                        break;
                    case WindowAnimType.ShowToBiggerCenter:
                        SetScale( 0.1f, 0.1f );
                        SetPivot( 0.5f, 0.5f );
                        TweenScale( new Vector2( 1, 1 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowToBiggerFromLeftUp:
                        SetScale( 0.1f, 0.1f );
                        SetPivot( 0, 0 );
                        TweenScale( new Vector2( 1, 1 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;

                    case WindowAnimType.ShowToBiggerFromLeftBottom:
                        SetScale( 0.1f, 0.1f );
                        SetPivot( 0, 1 );
                        TweenScale( new Vector2( 1, 1 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowToBiggerFromRightUp:
                        SetScale( 0.1f, 0.1f );
                        SetPivot( 1, 0 );
                        TweenScale( new Vector2( 1, 1 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowToBiggerFromRightBottom:
                        SetScale( 0.1f, 0.1f );
                        SetPivot( 1, 1 );
                        TweenScale( new Vector2( 1, 1 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowFromTop:
                        SetPosition( 0, -GRoot.inst.height * 1.2f, 0 );
                        SetPivot( 0.5f, 0.5f );
                        TweenMove( new Vector2( 0, 0 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowFromBottom:
                        SetPosition( 0, GRoot.inst.height * 1.2f, 0 );
                        SetPivot( 0.5f, 0.5f );
                        TweenMove( new Vector2( 0, 0 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowFromLeft:
                        SetPosition( -GRoot.inst.width * 1.2f, 0, 0 );
                        SetPivot( 0.5f, 0.5f );
                        TweenMove( new Vector2( 0, 0 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    case WindowAnimType.ShowFromRight:
                        SetPosition( GRoot.inst.width * 1.2f, 0, 0 );
                        SetPivot( 0.5f, 0.5f );
                        TweenMove( new Vector2( 0, 0 ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( OnShown );
                        break;
                    default:
                        break;
                }
            }
            else
            {
                //   _AnimTypeOfClose = animType;

                switch(_AnimTypeOfClose)
                {
                    case WindowAnimType.None:
                        HideImmediately();
                        break;
                    case WindowAnimType.HideToSmallInCenter:
                        SetPivot( 0.5f, 0.5f );
                        TweenScale( new Vector2( 0.01f, 0.01f ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToSmallInLeftUp:
                        SetPivot( 0, 0 );
                        TweenScale( new Vector2( 0.01f, 0.01f ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToSmallInLeftBottom:
                        SetPivot( 0, 1 );
                        TweenScale( new Vector2( 0.01f, 0.01f ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToSmallInRightUp:
                        SetPivot( 1, 0 );
                        TweenScale( new Vector2( 0.01f, 0.01f ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToSmallInRightBottom:
                        SetPivot( 1, 1 );
                        TweenScale( new Vector2( 0.01f, 0.01f ), 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToTop:
                        SetPivot( 0.5f, 0.5f );
                        TweenMoveY( -GRoot.inst.height * 1.2f, 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToBottom:
                        SetPivot( 0.5f, 0.5f );
                        TweenMoveY( GRoot.inst.height * 1.2f, 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToLeft:
                        SetPivot( 0.5f, 0.5f );
                        TweenMoveX( -GRoot.inst.width * 1.2f, 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    case WindowAnimType.HideToRight:
                        SetPivot( 0.5f, 0.5f );
                        TweenMoveX( GRoot.inst.width * 1.2f, 0.3f ).SetEase( Ease.OutQuad ).OnComplete( HideImmediately );
                        break;
                    default:
                        break;
                }

            }

        }*/
        #endregion

        /// <summary>
        /// 子类实例化，子类重写时在此调用show
        /// </summary>
        public virtual void OnAwake()
        {

        }

    }
}

/// <summary>
/// 窗口简单动画模式
/// </summary>


public enum WindowLayer
{
    /// <summary>
    /// 壁纸层,处于最低
    /// </summary>
    Wallpaper,
    /// <summary>
    /// 普通UI层
    /// </summary>
    Normal,
    /// <summary>
    /// 普通对话框,弹出框层
    /// </summary>
    Dialog,
    /// <summary>
    /// 系统警告层，最顶层
    /// </summary>
    Alert
}

