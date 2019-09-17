using Blazor.Gitter.Components.Models.Settings;
using Blazor.Gitter.Components.MVVMFramework.Serialization;
using Blazor.Gitter.Components.MVVMFramework.ViewModel;
using Blazored.LocalStorage;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

//
//  2019-05-10  Mark Stega
//              Created
//

namespace Blazor.Gitter.Components.ViewModels.Settings
{
    public class Settings_VM : ViewModelBase
    {
        private Settings_M m_SettingsM;

        #region ctor

        private Settings_VM() { }
        public Settings_VM(
            Settings_M settingsM
            )
        {
            m_SettingsM = settingsM;
        }

        #endregion

        #region properties
        public int pAccentColor
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pAccentColorText
        {
            get { return ((Settings_M.eAccentColor)pAccentColor).ToString(); }
        }

        public int pChatroomUpdateInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pChatroomUpdateIntervalText
        {
            get { return pChatroomUpdateInterval.ToString(); }
        }

        public int pRoomUpdateInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pRoomUpdateIntervalText
        {
            get { return pRoomUpdateInterval.ToString(); }
        }

        public int pSubmitKey
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pSubmitKeyText
        {
            get { return ((Settings_M.eSubmitKey)pSubmitKey).ToString(); }
        }

        public int pTheme
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pThemeText
        {
            get { return ((Settings_M.eTheme)pTheme).ToString(); }
        }

        public int pTimeoutInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }
        public string pTimeoutIntervalText
        {
            get { return pTimeoutInterval.ToString(); }
        }

        #endregion

        #region Persist

        public async Task Save()
        {
            // Set the M properties
            m_SettingsM.pAccentColor = (Settings_M.eAccentColor) pAccentColor;
            m_SettingsM.pChatroomUpdateInterval = pChatroomUpdateInterval;
            m_SettingsM.pRoomUpdateInterval = pRoomUpdateInterval;
            m_SettingsM.pSubmitKey = (Settings_M.eSubmitKey)pSubmitKey;
            m_SettingsM.pTheme = (Settings_M.eTheme)pTheme;
            m_SettingsM.pTimeoutInterval = pTimeoutInterval;

            // set the current M properties
            await m_SettingsM.Save();
        }

        public async Task Restore()
        {
            // Get the current M properties
            await m_SettingsM.Restore();

            // Set the VM properties
            pAccentColor = (int)m_SettingsM.pAccentColor;
            pChatroomUpdateInterval = m_SettingsM.pChatroomUpdateInterval;
            pRoomUpdateInterval = m_SettingsM.pRoomUpdateInterval;
            pSubmitKey = (int)m_SettingsM.pSubmitKey;
            pTheme = (int)m_SettingsM.pTheme;
            pTimeoutInterval = m_SettingsM.pTimeoutInterval;
        }

        #endregion

    }
}