/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Smartcard
{
    /// <summary>
    /// This class is used to update a set of parameters of an APDUCommand object
    /// </summary>
    public class APDUParam
    {
        byte
            m_bClass = 0,
            m_bChannel = 0,
            m_bP2 = 0,
            m_bP1 = 0;
        byte[] m_baData = null;
        short m_nLe = -1;
        bool 
            m_fUseP1 = false,
            m_fUseP2 = false,
            m_fChannel = false,
            m_fData = false,
            m_fClass = false,
            m_fLe = false;

        #region Constructors
        public APDUParam()
        {
        }

        /// <summary>
        /// Copy constructor (used for cloning)
        /// </summary>
        /// <param name="param"></param>
        public APDUParam(APDUParam param)
        {
            // Copy field
            if (param.m_baData != null)
                param.m_baData.CopyTo(m_baData, 0);
            m_bClass = param.m_bClass;
            m_bChannel = param.m_bChannel;
            m_bP1 = param.m_bP1;
            m_bP2 = param.m_bP2;
            m_nLe = param.m_nLe;

            // Copy flags field
            m_fChannel = param.m_fChannel;
            m_fClass = param.m_fClass;
            m_fData = param.m_fData;
            m_fLe = param.m_fLe;
            m_fUseP1 = param.m_fUseP1;
            m_fUseP2 = param.m_fUseP2;
        }

        public APDUParam(byte bClass, byte bP1, byte bP2, byte[] baData, short nLe)
        {
            this.Class = bClass;
            this.P1 = bP1;
            this.P2 = bP2;
            this.Data = baData;
            this.Le = (byte)nLe;
        }
        #endregion

        /// <summary>
        /// Clones the current APDUParam instance
        /// </summary>
        /// <returns></returns>
        public APDUParam Clone()
        {
            return new APDUParam(this);
        }

        /// <summary>
        /// Resets the current instance, all flags are set to false
        /// </summary>
        public void Reset()
        {
            m_bClass = 0;
            m_bChannel = 0;
            m_bP2 = 0;
            m_bP1 = 0;

            m_baData = null;
            m_nLe = -1;

            m_fUseP1 = false;
            m_fUseP2 = false;
            m_fChannel = false;
            m_fData = false;
            m_fClass = false;
            m_fLe = false;
        }

        #region Flags properties
        public bool UseClass
        {
            get { return m_fClass; }
        }

        public bool UseChannel
        {
            get { return m_fChannel; }
        }

        public bool UseLe
        {
            get { return m_fLe; }
        }

        public bool UseData
        {
            get { return m_fData; }
        }

        public bool UseP1
        {
            get { return m_fUseP1; }
        }

        public bool UseP2
        {
            get { return m_fUseP2; }
        }
        #endregion

        #region Parameter properties
        public byte P1
        {
            get { return m_bP1; }

            set
            {
                m_bP1 = value;
                m_fUseP1 = true;
            }
        }

        public byte P2
        {
            get { return m_bP2; }
            set
            {
                m_bP2 = value;
                m_fUseP2 = true;
            }

        }

        public byte[] Data
        {
            get { return m_baData; }
            set
            {
                m_baData = value;
                m_fData = true;
            }
        }

        public byte Le
        {
            get { return (byte)m_nLe; }
            set
            {
                m_nLe = value;
                m_fLe = true;
            }
        }

        public byte Channel
        {
            get { return m_bChannel; }
            set
            {
                m_bChannel = value;
                m_fChannel = true;
            }
        }

        public byte Class
        {
            get { return m_bClass; }
            set
            {
                m_bClass = value;
                m_fClass = true;
            }
        }

        #endregion
    }
}
