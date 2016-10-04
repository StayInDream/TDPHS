using UnityEngine;
using System.Collections;
using Spate;
using Jimmy;
using MiniJson;
using FairyGUI;
using System;

public class LoginWindow : Window, IUISource
{
    public const int NAME_MAX_LENGTH = 20;
    public const int NAME_MIN_LENGTH = 6;
    const string DEFAULT = "请输入6-20位账号和密码";
    const string DESC = "您输入的账号或密码长度不正确。请输入6-20位账号和密码";

    private string[] EnglishArr = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
    private int[] NumberArr = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    IUISource iuiSource;

    public string fileName
    {
        get
        {
            throw new NotImplementedException();
        }

        set
        {
            throw new NotImplementedException();
        }
    }

    public bool loaded
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public LoginWindow()
    {
        //iuiSource.fileName = Settings.UIPACKAGE_WIN_FOLDER + "Win";
        //  AddUISource( iuiSource );       
        WindowManager.SetMainView( "Win", "LoadingComment" );
        GObject go = this as GObject;
        
        Center();
    }
    protected override void OnInit()
    {
        GComponent obj = UIPackage.CreateObject( "Win", "Component2" ) as GComponent;
        WindowManager.MainView.AddChild( obj );

        GComponent obj2 = UIPackage.CreateObject( "Win", "Component1" ) as GComponent;
        WindowManager.MainView.AddChild( obj2 );


    }
 

    public void Load( UILoadCallback callback )
    {
        throw new NotImplementedException();
    }
}