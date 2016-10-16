using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.SocketServer;
using PhotonHostRuntimeInterfaces;
using ExitGames.Logging;
using log4net;
using log4net.Config;
using ExitGames.Logging.Log4Net;
using System.IO;
using TaidouServer.Handlers;
using TaidouCommon;

namespace TaidouServer
{
    public class TaidouApplication : ApplicationBase
    {
        private static TaidouApplication _instance;
        private static readonly ILogger log = ExitGames.Logging.LogManager.GetCurrentClassLogger();

        public Dictionary<byte , HandlerBase> dic_handlers = new Dictionary<byte , HandlerBase>();

        public TaidouApplication() { _instance = this; }

        public static TaidouApplication instance
        {
            get { if ( _instance == null ) _instance = new TaidouApplication(); return _instance; }
        }

        protected override PeerBase CreatePeer( InitRequest initRequest )
        {
            return new MClientPeer( initRequest );
        }

        protected override void Setup()
        {
            InitLogging();
        }

        protected override void TearDown()
        {
            log.Debug( "===> TaidouApplication TearDown complete..." );
        }

        void RegisteHandlers()
        {
            if ( dic_handlers.ContainsKey( ( byte )OpretionCode.Login ) )
                dic_handlers.Add( ( byte )OpretionCode.Login , new LoginHandler() );


        }

        protected virtual void InitLogging()
        {
            ExitGames.Logging.LogManager.SetLoggerFactory( Log4NetLoggerFactory.Instance );
            GlobalContext.Properties["Photon:ApplicationLogPath"] = Path.Combine( this.ApplicationRootPath , "log" );
            GlobalContext.Properties["LogFileName"] = "TD" + this.ApplicationName;
            XmlConfigurator.ConfigureAndWatch( new FileInfo( Path.Combine( this.BinaryPath , "log4net.config" ) ) );
            log.Debug( "===> TaidouApplication InitLogging complete..." );
        }
    }
}
