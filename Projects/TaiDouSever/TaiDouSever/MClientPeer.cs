using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using TaidouServer.Handlers;
using ExitGames.Logging;

namespace TaidouServer
{
    class MClientPeer : ClientPeer
    {
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();
        public MClientPeer( InitRequest initRequest ) : base( initRequest )
        {

        }

        protected override void OnDisconnect( DisconnectReason reasonCode , string reasonDetail )
        {
            throw new NotImplementedException();
        }

        protected override void OnOperationRequest( OperationRequest operationRequest , SendParameters sendParameters )
        {
            HandlerBase handlerbase;
            TaidouApplication.instance.dic_handlers.TryGetValue( operationRequest.OperationCode , out handlerbase );

            if ( handlerbase != null )
            {

            }
            else
            {
                log.Debug( "[Error:] ===>Cant find Handler from OperationCode:" + operationRequest.OperationCode );
            }
        }
    }
}
