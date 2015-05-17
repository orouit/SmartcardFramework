/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class ReadValueBlockCommand : APDUCommand
    {
        public ReadValueBlockCommand(byte blockNo)
            : base(MifareParam.CLASS, MifareParam.INS_READ_VALUE_BLOCK, 0x00, blockNo, null, 4)
        {
        }
    }
}
