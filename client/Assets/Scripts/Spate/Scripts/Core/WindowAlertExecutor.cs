using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

using UnityEngine;
using System.Collections;

namespace Spate
{
    internal class WindowAlertExecutor : BaseManager
    {
        public static WindowAlertExecutor Ins;
        //跑马灯请求间隔
        private int marqueeTime = 60;
        public int intenval = 0;
        private float nexttime = 0f;
        public bool IsEnterRole = false;
        List<PlayerData> listmyplayer = new List<PlayerData>();
        //IOS订单查询请求间隔
        private int iosgetgaptime = 3;
        public int iosgettime = 0;

        //网络或资源下载错误提示
        private bool isShowNetOrAssetError = false;
        public bool IsShowNetOrAssetError
        {
            get
            {
                return isShowNetOrAssetError;
            }
            set
            {
                isShowNetOrAssetError = value;
            }
        }
        //逻辑层错误弹窗
        private bool isShowLuoJiError = false;
        public bool IsShowLuoJiError
        {
            get
            {
                return isShowLuoJiError;
            }
            set
            {
                isShowLuoJiError = value;
            }
        }

        public override void OnAwake()
        {
            Ins = this;
            //NotifyWindow
            Notifier.Reg( NetState.Begine, OnBegineHandler );
            Notifier.Reg<string>( NetState.Error, OnNetErrorHandler );
            Notifier.Reg( NetState.Timeout, OnNetTimeOutHandler );
            Notifier.Reg( NetState.Succeed, OnSuccessHandler );
            Notifier.Reg<Action<string, string>, string, string>( AssetDownloadCode.Error, OnAssetErrorHandler );
            Notifier.Reg<Action<string, string>, string, string>( AssetDownloadCode.Timeout, OnAssetTimeOutHandler );
            Notifier.Reg<Action<string, string>, string, string>( AssetDownloadCode.LossData, OnAssetLossDataHandler );

            //PopupWindow
            Notifier.Reg<int>( GlobalUtil.SERVERRET, ShowServerErrorCode );
            Notifier.Reg<string>( GlobalUtil.LUOJIERROR, ShowLuoJiMessage );
            Notifier.Reg<string>( GlobalUtil.LUOJISUCCESS, ShowLuoJiMessage );
            //Notifier.Reg( GlobalUtil.PLAY_WINDOW_ANIMATION, PlayWindowAnimation );
            //Notifier.Reg( GlobalUtil.END_PLAY_WINDOW_ANIMATION, EndPlayWindowAnimation );
            Notifier.Reg( GlobalUtil.SURE_EQUIP_INHERIT_POP, SureEquipInherit );
            ///弹窗不需要回调的 
            Notifier.Reg( GlobalUtil.CDKEY_INPUT_ERROR, NoCallBack );
            Notifier.Reg( GlobalUtil.GOLD_NOT_ENOUGH, NoCallBack );
            Notifier.Reg( GlobalUtil.CONVERT_ERROR, NoCallBack );
            Notifier.Reg( GlobalUtil.GOOD_NO_ENOUGH, NoCallBack );
            Notifier.Reg( GlobalUtil.CHECK_EQUIP_ACTION, CheckEquipAction );

            Notifier.Reg<int>( GlobalUtil.PLAYERSVRDATA_UPDATE, PlayerDataUpdate );
        }

        public override void OnReset()
        {
            isShowNetOrAssetError = false;
            isShowLuoJiError = false;
            IsEnterRole = false;
        }

        public override void OnDestroy()
        {
            //NotifyWindow
            Notifier.Unreg( NetState.Begine, OnBegineHandler );
            Notifier.Unreg<string>( NetState.Error, OnNetErrorHandler );
            Notifier.Unreg( NetState.Timeout, OnNetTimeOutHandler );
            Notifier.Unreg( NetState.Succeed, OnSuccessHandler );
            Notifier.Unreg<Action<string, string>, string, string>( AssetDownloadCode.Error, OnAssetErrorHandler );
            Notifier.Unreg<Action<string, string>, string, string>( AssetDownloadCode.Timeout, OnAssetTimeOutHandler );
            Notifier.Unreg<Action<string, string>, string, string>( AssetDownloadCode.LossData, OnAssetLossDataHandler );

            //PopupWindow
            Notifier.Unreg<int>( GlobalUtil.SERVERRET, ShowServerErrorCode );
            Notifier.Unreg<string>( GlobalUtil.LUOJIERROR, ShowLuoJiMessage );
            Notifier.Unreg<string>( GlobalUtil.LUOJISUCCESS, ShowLuoJiMessage );
            //Notifier.Unreg( GlobalUtil.PLAY_WINDOW_ANIMATION, PlayWindowAnimation );
            //Notifier.Unreg( GlobalUtil.END_PLAY_WINDOW_ANIMATION, EndPlayWindowAnimation );
            Notifier.Unreg( GlobalUtil.SURE_EQUIP_INHERIT_POP, SureEquipInherit );
            //弹窗不需要回调的 
            Notifier.Unreg( GlobalUtil.CDKEY_INPUT_ERROR, NoCallBack );
            Notifier.Unreg( GlobalUtil.GOLD_NOT_ENOUGH, NoCallBack );
            Notifier.Unreg( GlobalUtil.CONVERT_ERROR, NoCallBack );
            Notifier.Unreg( GlobalUtil.GOOD_NO_ENOUGH, NoCallBack );
            Notifier.Unreg( GlobalUtil.CHECK_EQUIP_ACTION, CheckEquipAction );

            Notifier.Unreg<int>( GlobalUtil.PLAYERSVRDATA_UPDATE, PlayerDataUpdate );
        }

        public bool ShowAppNeedUpdatePopup( string url )
        {
            OpenPopupWindow().ShowAppNeedUpdate( url );
            return false;
        }

        public bool ShowAppNeedRestartPopup()
        {
            OpenPopupWindow().ShowAppNeedRestart();
            return false;
        }


        #region 通知窗口
        private NotifyWindow OpenNotifyWindow()
        {
            NotifyWindow notifywindow = WindowManager.FindWindow<NotifyWindow>();
            if(notifywindow == null || !notifywindow.isShowing)
            {
                notifywindow = WindowManager.OpenWindow<NotifyWindow>();
            }
            isShowNetOrAssetError = true;
            return notifywindow;
        }
        private bool OnBegineHandler( object key, params object[] args )
        {
            OpenNotifyWindow().OnBegineHandler( key, args );
            return false;
        }
        private bool OnNetErrorHandler( object key, string error )
        {
            OpenNotifyWindow().OnNetErrorHandler( key, error );
            return false;
        }
        private bool OnNetTimeOutHandler( object key, params object[] args )
        {
            OpenNotifyWindow().OnNetTimeOutHandler( key, args );
            return false;
        }
        private bool OnSuccessHandler( object key, params object[] args )
        {
            OpenNotifyWindow().OnSuccessHandler( key, args );
            return false;
        }
        private bool OnAssetErrorHandler( object key, Action<string, string> onClick, string name, string error )
        {
            OpenNotifyWindow().OnAssetErrorHandler( key, onClick, name, error );
            return false;
        }
        private bool OnAssetTimeOutHandler( object key, Action<string, string> onClick, string name, string userData )
        {
            OpenNotifyWindow().OnAssetTimeOutHandler( key, onClick, name, userData );
            return false;
        }
        private bool OnAssetLossDataHandler( object key, Action<string, string> onClick, string name, string userData )
        {
            OpenNotifyWindow().OnAssetLossDataHandler( key, onClick, name, userData );
            return false;
        }
        #endregion

        #region PopupWindow
        private PopupWindow OpenPopupWindow()
        {
            PopupWindow popupwindow = WindowManager.FindWindow<PopupWindow>();
            if(popupwindow == null || !popupwindow.isShowing)
            {
                popupwindow = WindowManager.OpenWindow<PopupWindow>();
            }
            isShowLuoJiError = true;
            if(isShowNetOrAssetError)
            {
                WindowManager.OpenWindow<NotifyWindow>();
            }
            return popupwindow;
        }
        private bool ShowServerErrorCode( object code, int ret )
        {
            OpenPopupWindow().ShowServerErrorCode( code, ret );
            return false;
        }
        private bool ShowLuoJiMessage( object code, string info )
        {
            OpenPopupWindow().ShowLuoJiMessage( code, info );
            return false;
        }
        private bool SureEquipInherit( object code, params object[] args )
        {
            OpenPopupWindow().SureEquipInherit( code, args );
            return false;
        }
        private bool NoCallBack( object code, params object[] args )
        {
            OpenPopupWindow().NoCallBack( code, args );
            return false;
        }
        private bool CheckEquipAction( object code, params object[] args )
        {
            OpenPopupWindow().CheckEquipAction( code, args );
            return false;
        }
        //public void ShowPopupWindowBtnSure( string info, UIEventListener.VoidDelegate btn_sure, UIEventListener.VoidDelegate btn_close )
        //{
        //    OpenPopupWindow().ShowPopupWindowBtnSure( info, btn_sure, btn_close );
        //}

        //public void ShowPopupWindowCanceSure( string info, UIEventListener.VoidDelegate btn_sure, UIEventListener.VoidDelegate btn_cancel )
        //{
        //    OpenPopupWindow().ShowPopupWindowBtnCancelSure( info, btn_sure, btn_cancel );
        //}
        #endregion

        private bool PlayerDataUpdate( object code, int pid )
        {
            //if(BattleLauncher.Inst != null)
            //{
            //    PlayerData playerdata = DataManager.Get<PlayerData>( pid );
            //    if(playerdata != null)
            //    {
            //        DataAPI.RefreshSinglePlayer( ref playerdata );
            //        if(DataUtil.IsTeamPlayer( playerdata ))
            //        {
            //            GetTeamPlayerList();
            //            int score = 0;
            //            for(int i = 0; i < listmyplayer.Count; i++)
            //            {
            //                score += DataUtil.GetHeroScore( listmyplayer[i] );
            //            }
            //            TeamSvrData teamsvrdata = DataManager.Get<TeamSvrData>();
            //            teamsvrdata.teampower = score;
            //            Notifier.Notify( GlobalUtil.TEAM_POWER_CHANGE );
            //        }
            //    }
            //}
            return false;
        }
    }
}