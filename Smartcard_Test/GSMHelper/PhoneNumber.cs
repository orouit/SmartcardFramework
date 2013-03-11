using System;
using System.Collections.Generic;
using System.Text;

namespace Core.GSMHelper
{
    public class PhoneNumber
    {
        public const int   
            TON_INTERNATIONAL = 1,
            TON_UNKNOWN = 0,
            TON_NATIONAL_NUMBER =2;
        
        public const int	NPI_ISDN = 1;

        public const int 
            GSM_MAX_DIAL_LENGTH = 20,
            GSM_MIN_DIAL_REC_LENGTH = 14,
            GSM_DIAL_BYTES = 10;

        const byte
            GSM_TON_MASK = 0x70,
            GSM_NPI_MASK = 0x0F,
            HIGH_MASK = 0xF0,
            LOW_MASK = 0x0F,
            END_OF_NUMBER = 0xFF; 

        const int 
            POS_LENGTHNUMBER = 0,
            POS_TONNPI = 1,
            POS_DIALLING = 2;

        // 7 bits default alphabet to ANSI coding
        // [Low Byte][High Byte]
        char[,]    DEFAULT_TO_ANSI = new char[,]
        {       
         /*  0    1    2    3    4    5    6    7  */
/* 0 */    {'@', ' ', ' ', '0', '¡', 'P', '¿', 'p'},
/* 1 */    {'£', ' ', '!', '1', 'A', 'Q', 'a', 'q'},
/* 2 */    {'$', 'ø', '"', '2', 'B', 'R', 'b', 'r'},
/* 3 */    {'¥', ' ', '#', '3', 'C', 'S', 'c', 's'},
/* 4 */    {'è', ' ', '¤', '4', 'D', 'T', 'd', 't'},
/* 5 */    {'é', ' ', '%', '5', 'E', 'U', 'e', 'u'},
/* 6 */    {'ù', ' ', '&', '6', 'F', 'V', 'f', 'v'},
/* 7 */    {'ì', ' ', '\'','7', 'G', 'W', 'g', 'w'},
/* 8 */    {'ò', ' ', '(', '8', 'H', 'X', 'h', 'x'},
/* 9 */    {'Ç', ' ', ')', '9', 'I', 'Y', 'i', 'y'},
/* A */    {(char)0x0A,' ', '*', ':', 'J', 'Z', 'j', 'z'},
/* B */    {'Ø', ' ', '+', ';', 'K', 'Ä', 'k', 'ä'},
/* C */    {'ø', 'Æ', ',', '<', 'L', 'Ö', 'l', 'ö'},
/* D */    {(char)0x0D,'æ', '-', '=', 'M', 'Ñ', 'm', 'ñ'},
/* E */    {'Å', 'ß', '.', '>', 'N', 'Ü', 'n', 'ü'},
/* F */    {'å', 'É', '/', '?', 'O', '§', 'o', 'à'}
        };  

        byte[] m_data;

        /// <summary>
        /// Build the 
        /// </summary>
        /// <param name="data"></param>
        public PhoneNumber(byte[] data)
        {
            m_data = data;
        }


        /// <summary>
        /// Return true if the number is empty
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                bool ret = true;

                for (int nI = 0; nI < m_data.Length; nI++)
                {
                    if (m_data[nI] != END_OF_NUMBER)
                    {
                        ret = false;
                        break;
                    }
                }

                return ret;
            }
        }


            /// <summary>
            /// Gets the Identifier value for this Phone number
            /// </summary>
            public string Identifier
        {
            get
            {
                short nId;
                StringBuilder identifier = new StringBuilder();

                if (IdentifierLength != 0)
                {
                    nId = 0;
                    while ((m_data[nId] != 0xFF) &&
                          (nId < IdentifierLength))
                    {
                        identifier.Append(ToANSI(m_data[nId++]));
                    }
                }

                return identifier.ToString();

            }
        }


        /// <summary>
        /// Type of Numbering, this is used to know if the number is International or local;
        /// </summary>
        public int TypeOfNumbering
        {
            get
            {
                return (m_data[IdentifierLength + POS_TONNPI] & GSM_TON_MASK) >> 4;
            }
        }


        /// <summary>
        /// Gets the number for this Phone number
        /// </summary>
        public string Number
        {
            get
            {
                StringBuilder number = new StringBuilder();
                int nI = 0;
                byte
                    bHigh,
                    bLow;

                byte[] m_address = new byte[GSM_MIN_DIAL_REC_LENGTH];
                Buffer.BlockCopy(m_data, IdentifierLength, m_address, 0, GSM_MIN_DIAL_REC_LENGTH);

                while ((m_address[POS_DIALLING + nI] != END_OF_NUMBER) &&
                      (nI < NumberLength))
                {
                    bHigh = (byte)(m_address[POS_DIALLING + nI] & HIGH_MASK);
                    bLow = (byte)(m_address[POS_DIALLING + nI] & LOW_MASK);

                    number.Append(GetASCII(bLow));
                    if (bHigh == HIGH_MASK)
                    {
                        break;
                    }
                    else
                    {
                        bHigh >>= 4;
                        number.Append(GetASCII(bHigh));

                        nI += 1;
                    }
                }

                // Manage international number
                if (TypeOfNumbering == TON_INTERNATIONAL)
                {
                    number.Insert(0, '+');
                }

                return number.ToString();
            }
        }


        /// <summary>
        /// Gets the ANSI character for a GSM character 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private char ToANSI(short data)
        {
            byte bHigh, bLow;

            bHigh = (byte)((data & 0x70) >> 4);
            bLow = (byte)(data & 0x0F);

            return DEFAULT_TO_ANSI[bLow, bHigh];
        }


        /// <summary>
        /// Gets the ASCII character for Phone number digit
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private char GetASCII(byte data)
        {
            char cDig = ' ';

            if (data <= 9)
            {
                cDig = (char)(((char)data) + '0');
            }
            else
            {
                switch (data)
                {
                    case 0x0A:
                        {
                            cDig = '*';
                            break;
                        }

                    case 0x0B:
                        {
                            cDig = '#';
                            break;
                        }

                    case 0x0C:
                        {
                            cDig = 'P';
                            break;
                        }

                    case 0x0D:
                        {
                            cDig = 'B';
                            break;
                        }

                    case 0x0E:
                        {
                            cDig = 'C';
                            break;
                        }
                }
            }

            return cDig;
        }


        /// <summary>
        /// Gets the length of the phone number in bytes
        /// </summary>
        private int NumberLength
        {
            get
            {
                return GSM_DIAL_BYTES;
            }
        }


        /// <summary>
        /// Gets the length of the Identifier
        /// </summary>
        private int IdentifierLength
        {
            get
            {
                return m_data.Length - GSM_MIN_DIAL_REC_LENGTH;
            }
        }


        /// <summary>
        /// Gets the number of bytes used for the Phone number, this includes the TON & NPI byte
        /// </summary>
        private int NumberOfBytesForPhoneNumber
        {
            get
            {
                return m_data[IdentifierLength + POS_LENGTHNUMBER] - 1;
            }   
        }


        /// <summary>
        /// Gets a string representation of the phone number
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string ret = "Not Used";

            if (!IsEmpty)
            {
                ret = string.Format(Identifier + ": " + Number);
            }

            return ret;
        }
    }
}
