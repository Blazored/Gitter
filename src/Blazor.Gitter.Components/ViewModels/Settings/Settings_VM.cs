using Blazor.Gitter.Components.Models.Settings;
using Blazor.Gitter.Components.MVVMFramework.Serialization;
using Blazor.Gitter.Components.MVVMFramework.ViewModel;
using Blazored.LocalStorage;

using System;
using System.Collections.Generic;

//
//  2019-05-10  Mark Stega
//              Created
//

namespace Blazor.Gitter.Components.ViewModels.Settings
{
    public class Settings_VM : ViewModelBase
    {
 
        #region properties
        public Settings_M.eAccentColor pAccentColor
        {
            get { return GetValue<Settings_M.eAccentColor>(); }
            set { SetValue(value); }
        }

        public int pChatroomUpdateInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public int pRoomUpdateInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        public Settings_M.eSubmitKey pSubmitKey
        {
            get { return GetValue<Settings_M.eSubmitKey>(); }
            set { SetValue(value); }
        }

        public Settings_M.eTheme pTheme
        {
            get { return GetValue<Settings_M.eTheme>(); }
            set { SetValue(value); }
        }

        public int pTimeoutInterval
        {
            get { return GetValue<int>(); }
            set { SetValue(value); }
        }

        #endregion

        #region ctor

        public Settings_VM() { }

        #endregion

    }
}