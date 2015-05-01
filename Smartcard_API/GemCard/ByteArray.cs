/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

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
    }
}
