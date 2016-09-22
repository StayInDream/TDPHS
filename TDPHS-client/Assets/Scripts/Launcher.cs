using UnityEngine;
using System.Collections;
using LP;
using System;
using System.IO;

/// <summary>
/// 游戏加载器
/// </summary>
public class Launcher : BaseBehaviour
{
    private  bool isDebug = true;

    public static Launcher Inst
    {
        get; private set;
    }

    public static bool IsDebug
    {
        private set
        {
            Inst.isDebug = value;
        }
        get
        {
            return Inst.isDebug;
        }
        
    }

    void Awake()
    {
        if(Inst != null)
        {
            //( "重复挂载GameLauncher" );
            Destroy( this );
            return;
        }
        Inst = this;
        IsDebug = File.Exists(Settings.UNITY_LOG_FILE);
        DontDestroyOnLoad( CachedGameObject );
        //启动游戏
        Engine.StartEngine(CachedGameObject);
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
