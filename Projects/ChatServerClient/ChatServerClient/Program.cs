using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExitGames.Client.Photon;

namespace ChatServerClient
{
    class Program
    {
        static void Main( string[] args )
        {
            ChatServerListener listener = new ChatServerListener();
            PhotonPeer peer = new PhotonPeer( listener, ConnectionProtocol.Tcp );

            //连接服务器
            peer.Connect( "192.168.1.249:4530", "ChatServer" );
            Console.WriteLine( "Conneting..." );
            while(listener.isConneted == false)
            {
                peer.Service();
            }

            //
            Dictionary<byte, object> dic = new Dictionary<byte, object>();
            dic.Add( 1, "用户名" );
            dic.Add( 2, "密码" );
            peer.OpCustom( 1, dic, true );

            while(true)
            {
                peer.Service();
            }

        }
    }

    class ChatServerListener : IPhotonPeerListener
    {
        public bool isConneted = false;
        public void DebugReturn( DebugLevel level, string message )
        {
        }

        public void OnEvent( EventData eventData )
        {
        }

        public void OnMessage( object messages )
        {
        }

        public void OnOperationResponse( OperationResponse operationResponse )
        {
            Dictionary<byte, object> dic = operationResponse.Parameters;
            object obj = null;
            dic.TryGetValue( 1, out obj );
            if(obj != null)
            {
                Console.WriteLine( string.Format( "Get value from server", obj.ToString() ) );
            }
        }

        public void OnStatusChanged( StatusCode statusCode )
        {
            switch(statusCode)
            {
                case StatusCode.Connect:
                    isConneted = true;
                    Console.WriteLine( "Has Conneted..." );
                    break;
                case StatusCode.Disconnect:
                    break;
                case StatusCode.Exception:
                    break;
                case StatusCode.ExceptionOnConnect:
                    break;
                case StatusCode.SecurityExceptionOnConnect:
                    break;
                case StatusCode.QueueOutgoingReliableWarning:
                    break;
                case StatusCode.QueueOutgoingUnreliableWarning:
                    break;
                case StatusCode.SendError:
                    break;
                case StatusCode.QueueOutgoingAcksWarning:
                    break;
                case StatusCode.QueueIncomingReliableWarning:
                    break;
                case StatusCode.QueueIncomingUnreliableWarning:
                    break;
                case StatusCode.QueueSentWarning:
                    break;
                case StatusCode.ExceptionOnReceive:
                    break;
                case StatusCode.TimeoutDisconnect:
                    break;
                case StatusCode.DisconnectByServer:
                    break;
                case StatusCode.DisconnectByServerUserLimit:
                    break;
                case StatusCode.DisconnectByServerLogic:
                    break;
                case StatusCode.EncryptionEstablished:
                    break;
                case StatusCode.EncryptionFailedToEstablish:
                    break;
                default:
                    break;
            }
        }
    }

}
