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

        static public byte[] Concatenate(byte[] dataA, byte[] dataB)
        {
            byte[] result = new byte[dataA.Length + dataB.Length];

            Buffer.BlockCopy(dataA, 0, result, 0, dataA.Length);
            Buffer.BlockCopy(dataB, 0, result, dataA.Length, dataB.Length);

            return result;
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
