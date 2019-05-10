using Blazor.Gitter.Components.MVVMFramework.Serialization;
using Blazored.LocalStorage;

using System;
using System.Collections.Generic;

//
//  2019-05-10  Mark Stega
//              Created
//

namespace Blazor.Gitter.Components.Models.Settings
{
    public class Settings_M
    {
        #region enums

        public enum eVersion
        {
            unknown = 0,
            v001 = 1
        };

        public enum eAccentColor
        {
            LightCyan = 0,
            LightRed = 1,
        };

        public enum eSubmitKey
        {
            Enter = 0,
            CtrlEnter = 1,
        }

        public enum eTheme
        {
            Dark = 0,
            Light = 1
        }

        #endregion

        #region members

        // Warning - These are used in the desrialization/deserialization process - Understand that before
        // making any changes as you are likely to invalidate stored data

        static private string kVersion = "Version";
        static private string kAccentColor = "kAccentColor";
        static private string kChatroomUpdateInterval = "kChatroomUpdateInterval";
        static private string kRoomUpdateInterval = "kRoomUpdateInterval";
        static private string kSubmitKey = "kSubmitKey";
        static private string kTheme = "kTheme";
        static private string kTimeoutInterval = "kTimeoutInterval";

        static private string kLocalStorageKey = "Blazor.Gitter.Settings";

        private readonly ILocalStorageService m_LocalStorageService;

        #endregion

        #region properties

        public eVersion Version { get; set; }

        public eAccentColor pAccentColor { get; set; }

        public int pChatroomUpdateInterval { get; set; }

        public int pRoomUpdateInterval { get; set; }

        public eSubmitKey pSubmitKey { get; set; }

        public eTheme pTheme { get; set; }

        public int pTimeoutInterval { get; set; }

        #endregion

        #region ctor

        private Settings_M() { }
        public Settings_M(
            ILocalStorageService localStorage
            )
        {
            m_LocalStorageService = localStorage;
        }

        #endregion

        #region Serialization

        private string Serialize()
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            dictionary.Add(kVersion, ((int)Version).ToString());

            dictionary.Add(kAccentColor, pAccentColor.ToString());
            dictionary.Add(kChatroomUpdateInterval, pChatroomUpdateInterval.ToString());
            dictionary.Add(kRoomUpdateInterval, pRoomUpdateInterval.ToString());
            dictionary.Add(kSubmitKey, pSubmitKey.ToString());
            dictionary.Add(kTheme, pTheme.ToString());

            return SimpleSerialization.SerializeDictionary(dictionary);
        }

        private void Deserialize(string p_SerialValue)
        {
            if (p_SerialValue == null || p_SerialValue.Length == 0)
            {
                // We have no stored value, we need to set defaults
                Version = eVersion.v001;
                pAccentColor = eAccentColor.LightCyan;
                pChatroomUpdateInterval = 60;
                pRoomUpdateInterval = 60;
                pSubmitKey = eSubmitKey.Enter;
                pTheme = eTheme.Dark;
                pTimeoutInterval = 60;
            }
            else
            {
                Dictionary<string, string> dictionary = SimpleSerialization.DeserializeDictionary(p_SerialValue);

                Version = (eVersion)Convert.ToInt32(dictionary[kVersion]);

                switch (Version)
                {
                    case eVersion.v001:
                        pAccentColor = (eAccentColor)Convert.ToInt32(dictionary[kAccentColor]);
                        pChatroomUpdateInterval = Convert.ToInt32(dictionary[kChatroomUpdateInterval]);
                        pRoomUpdateInterval = Convert.ToInt32(dictionary[kRoomUpdateInterval]);
                        pSubmitKey = (eSubmitKey)Convert.ToInt32(dictionary[kSubmitKey]);
                        pTheme = (eTheme)Convert.ToInt32(dictionary[kTheme]);
                        pTimeoutInterval = Convert.ToInt32(dictionary[kTimeoutInterval]);
                        break;

                    default:
                        throw new Exception("Error deserializing Settings_VM");
                }
            }
        }

        #endregion

        #region Persist

        public void Save()
        {
            var serialized = Serialize();
            m_LocalStorageService.SetItem(kLocalStorageKey, serialized);
        }

        public async void Restore()
        {
            var serialized = await m_LocalStorageService.GetItem<string>(kLocalStorageKey);
            Deserialize(serialized);
        }

        #endregion

    }
}