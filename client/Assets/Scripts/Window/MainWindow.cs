using UnityEngine;
using System.Collections;
using Spate;
using Jimmy;
using MiniJson;
using FairyGUI;
using System;


public class MainWindow : Window
{
    private GButton btn_QunYingXiu;
    private GButton btn_LunWuDian;
    private GButton btn_luoShenGe;
    private GButton btn_WuKaHai;
    private GButton btn_DianJiangTai;
    private GButton btn_TianJiBang;
    private GButton btn_XiTong;

    private GTextField text_PlayerName;
    private GTextField text_YuanBao;
    private GTextField text_QianBi;
    private GTextField text_LiLian;

    private GImage image_PalyerIcon;
    private GImage image_ChengHao;

    private GGroup playerIcon;

    public static MainWindow instance;

    public MainWindow()
    {

    }

    public override void OnAwake()
    {
        if ( instance == null )
            instance = new MainWindow();
        instance.Show();
    }
    protected override void OnUpdate()
    {
        base.OnUpdate();
    }

    protected override void OnInit()
    {
        base.OnInit();
        //WinAnimTypeOfOpen = WindowAnimType.ShowToBiggerFromLeftUp;
        // WinAnimTypeOfClose = WindowAnimType.HideToSmallInLeftUp;
        contentPane = UIPackage.CreateObject( "主界面" , "mainScreen" ).asCom;
        modal = true;
        Center();

        btn_QunYingXiu = contentPane.GetChild( "n57群英秀" ).asButton;
        btn_LunWuDian = contentPane.GetChild( "n58论武殿" ).asButton;
        btn_luoShenGe = contentPane.GetChild( "n59洛神阁" ).asButton;
        btn_WuKaHai = contentPane.GetChild( "n50武卡海" ).asButton;
        btn_DianJiangTai = contentPane.GetChild( "n50点将台" ).asButton;
        btn_TianJiBang = contentPane.GetChild( "n50天机榜" ).asButton;
        btn_XiTong = contentPane.GetChild( "n50系统" ).asButton;

        btn_QunYingXiu.onClick.Add( btnOnClick );
        btn_LunWuDian.onClick.Add( btnOnClick );
        btn_luoShenGe.onClick.Add( btnOnClick );
        btn_WuKaHai.onClick.Add( btnOnClick );
        btn_DianJiangTai.onClick.Add( btnOnClick );
        btn_TianJiBang.onClick.Add( btnOnClick );
        btn_XiTong.onClick.Add( btnOnClick );


        //  image_PalyerIcon = contentPane.GetChild( "n1" ).asImage;
        image_ChengHao = contentPane.GetChild( "n10" ).asImage;


        playerIcon = contentPane.GetChild( "n32" ).asGroup;


        Debug.Log( btn_QunYingXiu.icon = UIPackage.GetItemURL("主界面","论武殿"));
    }

    private void btnOnClick( EventContext context )
    {
        switch ( EventContext.NameOfEventContext( context ) )
        {
            case "n26"://群英秀
                WindowManager.OpenWindow<StartWindow>();
                break;
            case "n25"://论舞殿
                addCompent( "点将台+武卡海" , "dianjiangtai" );
                break;
            case "n24"://洛神阁

                break;
            case "n30"://武卡海

                break;
            case "n29"://点将台

                break;
            case "n28"://天机榜

                break;
            case "n27"://系统

                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 在此界面上新增组件
    /// </summary>
    /// <param name="comName">组件名</param>
    void addCompent( string uipackageName , string comName )
    {
        GComponent go = UIPackage.CreateObject( uipackageName , comName ).asCom;
        GObject obj = AddChild( go );

        obj.PlayAnim( AnimType.ShowToBiggerCenter );
        Center();


    }


    protected override void DoShowAnimation()
    {
        base.DoShowAnimation();
    }

    protected override void DoHideAnimation()
    {
        base.DoHideAnimation();

    }

}
