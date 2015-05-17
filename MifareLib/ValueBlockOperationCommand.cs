/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using Core.Utility;

namespace Core.Smartcard.Mifare
{
    public enum ValueBlockOperation : byte
    {
        STORE = 0,
        INC, 
        DEC,
        COPY
    }

    public class ValueBlockOperationCommand : APDUCommand
    {
        public ValueBlockOperationCommand(ValueBlockOperation operation, byte blockNo, byte[] value)
            : base(MifareParam.CLASS, MifareParam.INS_VALUE_BLOCK_OPER, 0x00, blockNo, null, 0)
        {
            Data = ByteArray.Concatenate(new byte[] { (byte)operation }, value);
        }
    }
}
