using System;
using System.IO;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

/// <summary>
/// 网络控制器,提供对网络的连接状态的维护、数据的包装发送以及消息的接收
/// </summary>
public class NetManager : MonoBehaviour
{
    public static NetManager Inst
    {
        get { return mInst; }
    }

    public void SetServerHost(string host, int port)
    {
        IPAddress.Parse(host);
    }





    void Awake()
    {
        if (mInst != null)
        {
            // TODO: log
            Destroy(this);
            return;
        }
        mInst = this;
        // 初始化TcpClient
        mClient = new TcpClient(AddressFamily.InterNetwork);
        mClient.SendTimeout = 10;
        mClient.SendBufferSize = 4096;
        mClient.ReceiveTimeout = 10;
        mClient.ReceiveBufferSize = 4096;
        // 初始化buffer
        //mReadBuffer = new byte[mClient.ReceiveBufferSize];
        //mWriteBuffer = new byte[mClient.SendBufferSize];

        //mConnector = new NetConnector(mClient);
    }

    void Update()
    {

    }


    private TcpClient mClient;
    //private NetConnector mConnector;

    //private byte[] mReadBuffer;
    //private byte[] mWriteBuffer;

    private static NetManager mInst;




    /// <summary>
    /// 网络连接器
    /// </summary>
    private class NetConnector
    {
        public NetConnector(TcpClient client)
        {
            mClient = client;
            mTimes = 0;
            mStatus = Status.None;
        }

        public IPAddress HostIp { get; set; }

        public int HostPort { get; set; }

        // 检测当前是否已经连接上
        public bool CheckConnected()
        {
            if (Status.Connected == mStatus || mClient.Connected)
                return true;
            // 维护连接状态
            if (Status.NotConnected == mStatus)
            {
                Connect();
            }
            return false;
        }

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect()
        {
            mStatus = Status.Connecting;
            mClient.BeginConnect(HostIp, HostPort, OnConnectAsyncCallback, null);
        }

        /// <summary>
        /// 连接回调
        /// </summary>
        private void OnConnectAsyncCallback(IAsyncResult ar)
        {
            mClient.EndConnect(ar);
            if (mClient.Connected)
            {
                mTimes = 0;
                mStatus = Status.Connected;
            }
            else
            {
                mTimes++;
                if (mTimes >= 3)
                {
                    mTimes = 0;
                    mStatus = Status.None;
                    if (OnConnectFaultEvent != null)
                        OnConnectFaultEvent("");
                }
                else
                {
                    mStatus = Status.NotConnected;
                }
            }
        }

        // 连接状态
        private enum Status : byte
        {
            None,
            NotConnected,
            Connecting,
            Connected
        }


        // tcp client
        private TcpClient mClient;
        // 当前连接状态
        private Status mStatus;
        // 连接失败连续的次数
        private int mTimes;

        public event Action<string> OnConnectFaultEvent;
    }


    private struct NetMessage
    {


        private byte[] rawData;
    }
}
