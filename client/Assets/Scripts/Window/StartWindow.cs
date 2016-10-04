using UnityEngine;
using System.Collections;
using Spate;
using Jimmy;
using MiniJson;
using FairyGUI;
using System;

public class StartWindow : Window
{
    public static StartWindow instance;

    private GProgressBar loadingBar;
    private Controller m_Controller;
    private GList m_serverleft;
    private GList m_serverright;
    private GTextInput text_userName;
    private GTextInput text_userPassworld;
    private GButton btn_region;
    private GButton btn_enterServer;
    private GButton btn_test;


    public StartWindow()
    {

    }

    public override void OnAwake()
    {
        if(instance == null)
            instance = new StartWindow();
        instance.Show();
    }


    protected override void OnInit()
    {
        base.OnInit();

        //WinAnimTypeOfOpen = WindowAnimType.ShowToBiggerCenter;
      //  WinAnimTypeOfClose = WindowAnimType.HideToSmallInLeftUp;

        contentPane = UIPackage.CreateObject( "开始窗口", "StartWindow" ).asCom;
        modal = true;
        Center();
        loadingBar = contentPane.GetChild( "n26" ) as GProgressBar;
        loadingBar.value = 0;
        m_serverleft = contentPane.GetChild( "list_left" ) as GList;
        m_serverright = contentPane.GetChild( "list_right" ) as GList;
        text_userName = contentPane.GetChild( "n17" ) as GTextInput;
        text_userPassworld = contentPane.GetChild( "n18" ) as GTextInput;
        m_Controller = contentPane.GetController( "c1" );
        m_Controller.selectedIndex = 0;
        btn_region = contentPane.GetChild( "btn_region" ) as GButton;
        btn_enterServer = contentPane.GetChild( "btn_enter" ) as GButton;

        btn_enterServer.onClick.Add( btnOnclick );
        btn_region.onClick.Add( btnOnclick );

      //  Debug.Log( name+ contentPane.sortingOrder );
    }

    private void btnOnclick( EventContext context )
    {
        string btnname = ( (GObject)( context.sender ) ).name;
        switch(btnname)
        {
            case "btn_region":
                Debug.Log( btnname );
                m_Controller.selectedIndex = 2;
                break;
            case "btn_enter":
                // m_Controller.selectedIndex = 1;
                WindowManager.CloseWindow( this );
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 显示服务区
    /// </summary>
    private void showSververList()
    {
        m_serverleft.itemRenderer = RenderSvrverLeftItemList;
        m_serverleft.SetVirtual();
        m_serverright.itemRenderer = RenderSvrverRightItemList;
        m_serverright.SetVirtual();
        // m_serverright.numItems = 500;
        m_serverleft.numItems = 35 % 10 + 1;
    }

    private void RenderSvrverLeftItemList( int index, GObject item )
    {
        GTextField title = item.asCom.GetChild( "title" ).asTextField;
        title.text = index + "-";

        item.onClick.Add( () =>
         {
             GImage icon_serverchoose = item.asCom.GetChild( "icon_serverchoose" ) as GImage;
             for(int i = 0; i < m_serverleft.numChildren; i++)
             {
                 if(m_serverleft.GetChildAt( i ) != item)
                 {
                     ( m_serverleft.GetChildAt( i ).asCom.GetChild( "icon_serverchoose" ) as GImage ).visible = false;
                 }
             }
             icon_serverchoose.visible = true;

         } );
        Debug.Log( index );
    }

    private void RenderSvrverRightItemList( int index, GObject item )
    {
        item.onClick.Add( () =>
         {
             GImage icon_serverchoose = item.asCom.GetChild( "icon_serverchoose" ) as GImage;
             for(int i = 0; i < m_serverright.numChildren; i++)
             {
                 if(m_serverright.GetChildAt( i ) != item)
                 {
                     ( m_serverright.GetChildAt( i ).asCom.GetChild( "icon_serverchoose" ) as GImage ).visible = false;
                 }
             }
             icon_serverchoose.visible = true;

         } );
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if(m_Controller != null && m_Controller.selectedIndex == 1)
        {
            loadingBar.value += UnityEngine.Random.Range( 0.1f, 0.5f );
            if(loadingBar.value >= 100)
            {
                m_Controller = null;
                loadingBar.value = 0;
                WindowManager.CloseWindow( this );
            }
        }



    }

    protected override void OnShown()
    {
        base.OnShown();
    }
    protected override void OnHide()
    {
        base.OnHide();
        if(isDispoes)
        {
            Dispose();
        }
    }

    protected override void DoShowAnimation()
    {
        Debug.Log( WinAnimTypeOfOpen );
        base.DoShowAnimation();

    }


    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();
        WindowManager.OpenWindow<MainWindow>();

    }

}
