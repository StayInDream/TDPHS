using UnityEngine;
using System.Collections;
using Spate;
using System.Collections.Generic;
using System;
using Jimmy;
using UnityEngine.SceneManagement;
using FairyGUI;

/// <summary>
/// 游戏加载器
/// </summary>
public class Launcher : BaseBehaviour
{
    public static Launcher Inst
    {
        get; private set;
    }
    public static LevelAfterUpdate Next
    {
        get; set;
    }

    public static List<string> listShouldOpenWindow = new List<string>();

    void Awake()
    {
        if ( Inst != null )
        {
            MLogger.LogError( "重复挂载GameLauncher" );
            Destroy( this );
            return;
        }
        Inst = this;
        DontDestroyOnLoad( CachedGameObject );
        // 启动Engine
        Engine.StartEngine( CachedGameObject );

        //  EffectManager.Start(CachedGameObject);
        //  ChatService.StartService(CachedGameObject);
        //   SuperSDK.SetListener(CachedGameObject.AddComponent<GameSDKListener>());
        // 本地推送
        //   CachedGameObject.AddComponent<LocalPusher>();
    }

    void Start()
    {
        //AssetManager.LoadAllSoundEffects();
        //AssetAPI.LoadCategory("ui_effect");
        // 初始化SDK,初始化成功后才能打开SettingsView
        // SuperSDK.SDK_Init();
        WindowManager.SetMainView( "主界面" , "" );// "mainScreen" );     
        WindowManager.OpenWindow<MainWindow>();
    }
    void Update()
    {
        //if(Input.GetKeyDown( KeyCode.Space ))
        //    WindowManager.OpenWindow<StartWindow>();
    }
    // launcher->update->game
    public static void LaunchGame()
    {
        Next = LevelAfterUpdate.Game;
        SceneManager.LoadScene( "launcher" );
    }

    // launcher->update->battle
    public static void LaunchBattle()
    {
        Next = LevelAfterUpdate.Battle;
        SceneManager.LoadScene( "launcher" );
    }

    public enum LevelAfterUpdate
    {
        Game,
        Battle
    }
}
