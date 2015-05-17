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

		private	byte	bCla;
		private	byte	bIns;
		private	byte	bP1;
		private	byte	bP2;
		private	byte[]	baData;
		private	byte	bLe;	

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
			this.bCla = bCla;
			this.bIns = bIns;
			this.bP1 = bP1;
			this.bP2 = bP2;
			this.baData = baData;
			this.bLe = bLe;
		}

		/// <summary>
		/// Update the current APDU with selected parameters
		/// </summary>
		/// <param name="apduParam">APDU parameters</param>
		public	void	Update(APDUParam apduParam)
		{
			if (apduParam.UseData)
				baData = apduParam.Data;

			if (apduParam.UseLe)
				bLe = apduParam.Le;

			if (apduParam.UseP1)
				bP1 = apduParam.P1;

			if (apduParam.UseP2)
				bP2 = apduParam.P2;

            if (apduParam.UseChannel)
                bCla += apduParam.Channel;
        }

        #region Accessors
        /// <summary>
		/// Class get property
		/// </summary>
		public	byte Class
		{
			get
			{
				return bCla;
			}
		}

		/// <summary>
		/// Instruction get property
		/// </summary>
		public byte	Ins
		{
			get
			{
				return bIns;
			}
		}

		/// <summary>
		/// Parameter P1 get property
		/// </summary>
		public byte	P1
		{
			get
			{
				return bP1;
			}
		}

		/// <summary>
		/// Parameter P2 get property
		/// </summary>
		public byte P2
		{
			get
			{
				return bP2;
			}
		}

		/// <summary>
		/// Data get property
		/// </summary>
		public byte[] Data
		{
			get
			{
				return baData;
			}

            protected set { baData = value; }
		}

		/// <summary>
		/// Length expected get property
		/// </summary>
		public byte	Le
		{
			get
			{
				return bLe;
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
                bP3 = bLe;    

            if (baData != null)
            {
                StringBuilder sData = new StringBuilder(baData.Length * 2);
                for (int nI = 0; nI < baData.Length; nI++)
                    sData.AppendFormat("{0:X02}", baData[nI]);

                strData = "Data=" + sData.ToString();
                bLc = (byte) baData.Length;
                bP3 = bLc;
            }
            
            //string strApdu = string.Format("Class={0:X02} Ins={1:X02} P1={2:X02} P2={3:X02} Le={4:X02} Lc={5:X02} ",
                //m_bCla, m_bIns, m_bP1, m_bP2, m_bLe, bLc);
            StringBuilder strApdu = new StringBuilder();

            strApdu.AppendFormat("Class={0:X02} Ins={1:X02} P1={2:X02} P2={3:X02} P3={4:X02} ",
                bCla, bIns, bP1, bP2, bP3);
            if (baData != null)
                strApdu.Append(strData);

            return strApdu.ToString();
        }
    }
}
