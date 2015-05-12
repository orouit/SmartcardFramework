/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Text;

using Core.Smartcard;

namespace Core.Smartcard
{
    /// <summary>
    /// This class is used to log a command
    /// </summary>
    public class APDULog
    {
        private APDUCommand m_apduCmd = null;
        private APDUResponse m_apduResp = null;

        public APDULog(APDUCommand apduCmd, APDUResponse apduResp)
        {
            m_apduCmd = apduCmd;
            m_apduResp = apduResp;
        }

        #region Accessors
        public APDUCommand ApduCmd
        {
            get { return m_apduCmd; }
            set { m_apduCmd = value; }
        }

        public APDUResponse ApduResp
        {
            get { return m_apduResp; }
            set { m_apduResp = value; }
        }
        #endregion

        public override string ToString()
        {
            return m_apduCmd.ToString() + "\r\n" + m_apduResp.ToString();
        }
    }

    /// <summary>
    /// List of APDULog
    /// </summary>
    public class APDULogList : List<APDULog>
    {
    }
}
