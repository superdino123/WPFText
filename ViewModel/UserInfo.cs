using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ViewModel
{
    public class UserInfo
    {
        #region LoginTime

        private DateTime _LoginTime;

        public DateTime LoginTime
        {
            get { return _LoginTime;}
            set
            {
                if (_LoginTime != null && _LoginTime.Equals(value)) return;
                _LoginTime = value;
            }
        }

        #endregion

        #region ServerIP

        private string _ServerIP;

        public string ServerIP
        {
            get { return _ServerIP; }
            set
            {
                if (_ServerIP != null && _ServerIP.Equals(value)) return;
                _ServerIP = value;
            }
        }

        #endregion ServerIP

        #region DataBaseName

        private string _DataBaseName;

        public string DataBaseName
        {
            get { return _DataBaseName; }
            set
            {
                if (_DataBaseName != null && _DataBaseName.Equals(value)) return;
                _DataBaseName = value;
            }
        }

        #endregion DataBaseName

        #region AutoLogin

        private bool _AutoLogin;

        public bool AutoLogin
        {
            get { return _AutoLogin; }
            set
            {
                if (_AutoLogin != null && _AutoLogin.Equals(value)) return;
                _AutoLogin = value;
            }
        }

        #endregion AutoLogin

        #region CurrentClientVersion

        private string _CurrentClientVersion;

        public string CurrentClientVersion
        {
            get { return _CurrentClientVersion; }
            set
            {
                if (_CurrentClientVersion != null && _CurrentClientVersion.Equals(value)) return;
                _CurrentClientVersion = value;
            }
        }

        #endregion CurrentClientVersion

        #region UserArea

        private UserArea _UserArea;

        public UserArea UserArea
        {
            get { return _UserArea; }
            set
            {
                if (_UserArea != null && _UserArea.Equals(value)) return;
                _UserArea = value;
            }
        }

        #endregion
    }
}
