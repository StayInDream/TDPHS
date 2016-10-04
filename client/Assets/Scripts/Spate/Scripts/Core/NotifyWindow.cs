
using System;
using FairyGUI;
namespace Spate
{
    public class NotifyWindow : Window
    {
        internal void OnBegineHandler( object key, object[] args )
        {
            throw new NotImplementedException();
        }

        internal void OnNetErrorHandler( object key, string error )
        {
            throw new NotImplementedException();
        }

        internal void OnNetTimeOutHandler( object key, object[] args )
        {
            throw new NotImplementedException();
        }

        internal void OnSuccessHandler( object key, object[] args )
        {
            throw new NotImplementedException();
        }

        internal void OnAssetErrorHandler( object key, Action<string, string> onClick, string name, string error )
        {
            throw new NotImplementedException();
        }

        internal void OnAssetTimeOutHandler( object key, Action<string, string> onClick, string name, string userData )
        {
            throw new NotImplementedException();
        }

        internal void OnAssetLossDataHandler( object key, Action<string, string> onClick, string name, string userData )
        {
            throw new NotImplementedException();
        }
    }
}