/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class UpdateBinaryBlocksCommand : APDUCommand
    {
        public UpdateBinaryBlocksCommand(byte blockNumber, byte[] data)
            : base(MifareParam.CLASS, MifareParam.INS_UPDATE_BLOCKS, 0, blockNumber, data, 0)
        {
        }
    }
}
