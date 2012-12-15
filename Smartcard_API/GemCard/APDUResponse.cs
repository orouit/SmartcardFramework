using System;
using System.Text;

namespace GemCard
{
	/// <summary>
	/// This class represents the APDU response sent by the card
	/// </summary>
	public class APDUResponse
	{
		/// <summary>
		///	Status bytes length
		/// </summary>
		public const int SW_LENGTH = 2;		

		private byte[]	m_baData = null;	
		private byte	m_bSw1;
		private byte	m_bSw2;

		/// <summary>
		/// Constructor from the byte data sent back by the card
		/// </summary>
		/// <param name="baData">Buffer of data from the card</param>
		public APDUResponse(byte[] baData)
		{
			if (baData.Length > SW_LENGTH)
			{
				m_baData = new byte[baData.Length - SW_LENGTH];

				for (int nI = 0; nI < baData.Length - SW_LENGTH; nI++)
					m_baData[nI] = baData[nI];
			}

			m_bSw1 = baData[baData.Length - 2];
			m_bSw2 = baData[baData.Length - 1];
		}

		/// <summary>
		/// Response data get property. Contains the data sent by the card minus the 2 status bytes (SW1, SW2)
		/// null if no data were sent by the card
		/// </summary>
		public	byte[]	Data
		{
			get
			{
				return m_baData;
			}
		}

		/// <summary>
		/// SW1 byte get property
		/// </summary>
		public byte	SW1
		{
			get
			{
				return m_bSw1;
			}
		}

		/// <summary>
		/// SW2 byte get property
		/// </summary>
		public byte	SW2
		{
			get
			{
				return m_bSw2;
			}
		}

		/// <summary>
		/// Status get property
		/// </summary>
		public	ushort	Status
		{
			get
			{
				return (ushort) (((short) m_bSw1 << 8) + (short) m_bSw2);
			}
		}

        /// <summary>
        /// Overrides the ToString method to format to a string the APDUResponse object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string sRet;
    
            // Display SW1 SW2
            sRet = string.Format("SW={0:X04}", Status);

            if (m_baData != null)
            {
                StringBuilder sData = new StringBuilder(m_baData.Length * 2);
                for (int nI = 0; nI < m_baData.Length; nI++)
                    sData.AppendFormat("{0:X02}", m_baData[nI]);

                sRet += " Data=" + sData.ToString();
            }

            return sRet;
        }
	}
}
