/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class ReadBinaryBlocksCommand : APDUCommand
    {
        const int BLOCK_SIZE = 16;

        public ReadBinaryBlocksCommand(byte blockNumber, byte numberOfBlocks)
            : base(MifareParam.CLASS, MifareParam.INS_READ_BLOCKS, 0, blockNumber, null, (byte) (numberOfBlocks * BLOCK_SIZE))
        {
        }
    }
}
