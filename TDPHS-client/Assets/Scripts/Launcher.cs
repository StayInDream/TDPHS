using UnityEngine;
using System.Collections;
using LP;

/// <summary>
/// 游戏加载器
/// </summary>
public class Launcher : BaseBehaviour
{
    [SerializeField]
    public  bool isDebug = true;

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
        IsDebug = isDebug;
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
