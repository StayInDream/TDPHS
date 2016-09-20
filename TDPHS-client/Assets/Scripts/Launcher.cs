using UnityEngine;
using System.Collections;
using LP;

/// <summary>
/// 游戏加载器
/// </summary>
public class Launcher : BaseBehaviour
{
    public static bool isDebug = true;

    public static Launcher Inst
    {
        get; private set;
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
        DontDestroyOnLoad( CachedGameObject );
        //启动游戏
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
