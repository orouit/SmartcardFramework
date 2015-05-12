/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class MifareHelper
    {
        private APDUPlayer apduPlayer;
        private APDUResponse lastApduResponse = null;

        public MifareHelper(ICard card)
        {
            apduPlayer = new APDUPlayer(card);
        }

        public bool AuthenticateBlock(int blockNumber, int keyType = MifareParam.KEY_TYPE_A, int keyNumber = 0)
        {
            AuthenticateBlockCommand authBlockCmd = new AuthenticateBlockCommand((byte) blockNumber, (byte)keyType, (byte) keyNumber);
            lastApduResponse = apduPlayer.ExecuteApduCommand(authBlockCmd);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool ReadBlocks(int startBlock, int numberOfBlocks, out byte[] data)
        {
            bool success = false;
            data = null;
            ReadBinaryBlocksCommand readBlocksCmd = new ReadBinaryBlocksCommand((byte)startBlock, (byte)numberOfBlocks);
            lastApduResponse = apduPlayer.ExecuteApduCommand(readBlocksCmd);

            if (lastApduResponse.Status == MifareStatus.SUCCESS)
            {
                data = lastApduResponse.Data;
                success = true;
            }

            return success;
        }

        public bool UpdateBlocks(int startBlock, byte[] data)
        {
            UpdateBinaryBlocksCommand updateBlocksCmd = new UpdateBinaryBlocksCommand((byte)startBlock, data);
            lastApduResponse = apduPlayer.ExecuteApduCommand(updateBlocksCmd);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }
    }
}
