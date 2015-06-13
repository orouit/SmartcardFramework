/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Globalization;
using System.Text;

namespace Core.Utility
{
    static public class ByteArray
    {
        static public string ToString(byte[] data)
        {
            StringBuilder sDataOut = new StringBuilder();

            if (data != null)
            {
                sDataOut = new StringBuilder(data.Length * 2);
                for (int nI = 0; nI < data.Length; nI++)
                {
                    sDataOut.AppendFormat("{0:X02}", data[nI]);
                }
            }

            return sDataOut.ToString();
        }

        static public byte[] Parse(string data)
        {
            byte[] byteData = new byte[0];

            int dataLength = data.Length / 2;
            if (dataLength != 0)
            {
                byteData = new byte[dataLength];
                for (int nJ = 0; nJ < dataLength; nJ++)
                {
                    byteData[nJ] = byte.Parse(data.Substring(nJ * 2, 2), NumberStyles.AllowHexSpecifier);
                }
            }

            return byteData;
        }

        /// <summary>
        /// Check if the byte[] is composed of the same unique value
        /// </summary>
        /// <param name="srce"></param>
        /// <param name="value">value to check, defaul is 0</param>
        /// <returns></returns>
        static public bool IsSetOf(this byte[] srce, byte value = 0)
        {
            bool ret = true;

            foreach (byte data in srce)
            {
                if (data != 0)
                {
                    ret = false;
                    break;
                }
            }

            return ret;
        }

        static public void Fill(this byte[] srce, byte value)
        {
            for (int n = 0; n < srce.Length; n++ )
            {
                srce[n] = value;
            }
        }

        static public void Fill(this byte[] srce, byte value, int offset, int count)
        {
            for (int n = offset; n < offset + count; n++)
            {
                srce[n] = value;
            }
        }

        static public bool AreSame(byte[] dataA, byte[] dataB)
        {
            bool same = false;
            if (dataA.Length == dataB.Length)
            {
                same = true;
                for (int n = 0; n < dataA.Length; n++)
                {
                    if (dataA[n] != dataB[n])
                    {
                        same = false;
                        break;
                    }
                }
            }

            return same;
        }

        static public byte[] Concatenate(byte[] dataA, byte[] dataB)
        {
            byte[] result = new byte[dataA.Length + dataB.Length];

            Buffer.BlockCopy(dataA, 0, result, 0, dataA.Length);
            Buffer.BlockCopy(dataB, 0, result, dataA.Length, dataB.Length);

            return result;
        }

        static public byte[] RotateLeft(byte[] data)
        {
            byte[] rotated = new byte[data.Length];

            byte firstByte = data[0];
            Buffer.BlockCopy(data, 1, rotated, 0, 7);
            rotated[7] = firstByte;
            //Buffer.BlockCopy(data, 0, rotated, 7, 1);

            return rotated;
        }

        static public bool CompareWithRotated(byte[] data, byte[] data_r)
        {
            byte[] tmpRndr = ByteArray.RotateLeft(data);
            return ByteArray.AreSame(data_r, tmpRndr);
        }

        static public byte[] ReverseBuffer(byte[] data)
        {
            byte[] reversed = new byte[data.Length];

            for (int n = 0; n < data.Length; n++)
            {
                reversed[data.Length - n - 1] = data[n];
            }

            return reversed;
        }
    }
}
