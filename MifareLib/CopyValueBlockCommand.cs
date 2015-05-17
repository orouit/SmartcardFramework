/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class CopyValueBlockCommand : APDUCommand
    {
        public CopyValueBlockCommand(byte srceBlockNo, byte destBlockNo)
            : base(MifareParam.CLASS, MifareParam.INS_VALUE_BLOCK_OPER, 0x00, srceBlockNo, new byte[] { (byte)ValueBlockOperation.COPY, destBlockNo }, 0)
        {
        }
    }
}
