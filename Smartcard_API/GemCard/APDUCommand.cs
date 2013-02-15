/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Text;

namespace Core.Smartcard
{
	/// <summary>
	/// This class represents a command APDU
	/// </summary>
	public	class	APDUCommand
	{
		/// <summary>
		/// Minimun size in bytes of an APDU command
		/// </summary>
		public	const int	APDU_MIN_LENGTH = 4;

		private	byte	m_bCla;
		private	byte	m_bIns;
		private	byte	m_bP1;
		private	byte	m_bP2;
		private	byte[]	m_baData;
		private	byte	m_bLe;	

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="bCla">Class byte</param>
		/// <param name="bIns">Instruction byte</param>
		/// <param name="bP1">Parameter P1 byte</param>
		/// <param name="bP2">Parameter P2 byte</param>
		/// <param name="baData">Data to send to the card if any, null if no data to send</param>
		/// <param name="bLe">Number of data expected, 0 if none</param>
		public	APDUCommand(byte bCla, byte bIns, byte bP1, byte bP2, byte[] baData, byte bLe)
		{
			m_bCla = bCla;
			m_bIns = bIns;
			m_bP1 = bP1;
			m_bP2 = bP2;
			m_baData = baData;
			m_bLe = bLe;
		}

		/// <summary>
		/// Update the current APDU with selected parameters
		/// </summary>
		/// <param name="apduParam">APDU parameters</param>
		public	void	Update(APDUParam apduParam)
		{
			if (apduParam.UseData)
				m_baData = apduParam.Data;

			if (apduParam.UseLe)
				m_bLe = apduParam.Le;

			if (apduParam.UseP1)
				m_bP1 = apduParam.P1;

			if (apduParam.UseP2)
				m_bP2 = apduParam.P2;

            if (apduParam.UseChannel)
                m_bCla += apduParam.Channel;
        }

        #region Accessors
        /// <summary>
		/// Class get property
		/// </summary>
		public	byte Class
		{
			get
			{
				return m_bCla;
			}
		}

		/// <summary>
		/// Instruction get property
		/// </summary>
		public byte	Ins
		{
			get
			{
				return m_bIns;
			}
		}

		/// <summary>
		/// Parameter P1 get property
		/// </summary>
		public byte	P1
		{
			get
			{
				return m_bP1;
			}
		}

		/// <summary>
		/// Parameter P2 get property
		/// </summary>
		public byte P2
		{
			get
			{
				return m_bP2;
			}
		}

		/// <summary>
		/// Data get property
		/// </summary>
		public byte[] Data
		{
			get
			{
				return m_baData;
			}
		}

		/// <summary>
		/// Length expected get property
		/// </summary>
		public byte	Le
		{
			get
			{
				return m_bLe;
			}
        }

        #endregion

        /// <summary>
        /// Overrides the ToString method to format to a string the APDUCommand object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string strData = null;
            byte
                bLc = 0,
                bP3 = m_bLe;    

            if (m_baData != null)
            {
                StringBuilder sData = new StringBuilder(m_baData.Length * 2);
                for (int nI = 0; nI < m_baData.Length; nI++)
                    sData.AppendFormat("{0:X02}", m_baData[nI]);

                strData = "Data=" + sData.ToString();
                bLc = (byte) m_baData.Length;
                bP3 = bLc;
            }
            
            //string strApdu = string.Format("Class={0:X02} Ins={1:X02} P1={2:X02} P2={3:X02} Le={4:X02} Lc={5:X02} ",
                //m_bCla, m_bIns, m_bP1, m_bP2, m_bLe, bLc);
            StringBuilder strApdu = new StringBuilder();

            strApdu.AppendFormat("Class={0:X02} Ins={1:X02} P1={2:X02} P2={3:X02} P3={4:X02} ",
                m_bCla, m_bIns, m_bP1, m_bP2, bP3);
            if (m_baData != null)
                strApdu.Append(strData);

            return strApdu.ToString();
        }
    }
}
