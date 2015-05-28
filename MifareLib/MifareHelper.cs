/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using Core.Utility;
using System;

namespace Core.Smartcard.Mifare
{
    public class MifareHelper : CardHelper
    {
        public MifareHelper(ICard card)
            : base(card)
        {
        }

        public bool AuthenticateBlock(int blockNumber, int keyType = MifareParam.KEY_TYPE_A, int keyNumber = 0)
        {
            AuthenticateBlockCommand authBlockCmd = new AuthenticateBlockCommand((byte) blockNumber, (byte)keyType, (byte) keyNumber);
            lastApduResponse = apduPlayer.ExecuteApduCommand(authBlockCmd);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool ReadBlocks(int startBlockNumber, int numberOfBlocks, out byte[] data)
        {
            bool success = false;
            data = null;
            ReadBinaryBlocksCommand readBlocksCmd = new ReadBinaryBlocksCommand((byte)startBlockNumber, (byte)numberOfBlocks);
            lastApduResponse = apduPlayer.ExecuteApduCommand(readBlocksCmd);

            if (lastApduResponse.Status == MifareStatus.SUCCESS)
            {
                data = lastApduResponse.Data;
                success = true;
            }

            return success;
        }

        public bool UpdateBlocks(int startBlockNumber, byte[] data)
        {
            UpdateBinaryBlocksCommand updateBlocksCmd = new UpdateBinaryBlocksCommand((byte)startBlockNumber, data);
            lastApduResponse = apduPlayer.ExecuteApduCommand(updateBlocksCmd);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool StoreValueBlock(int blockNumber, Int32 value)
        {
            byte[] valueBytes = ByteArray.ReverseBuffer(BitConverter.GetBytes(value));
            ValueBlockOperationCommand valueOperationCommand = new ValueBlockOperationCommand(ValueBlockOperation.STORE, (byte)blockNumber, valueBytes);
            lastApduResponse = apduPlayer.ExecuteApduCommand(valueOperationCommand);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool IncrementValueBlock(int blockNumber, Int32 value)
        {
            byte[] valueBytes = ByteArray.ReverseBuffer(BitConverter.GetBytes(value));
            ValueBlockOperationCommand valueOperationCommand = new ValueBlockOperationCommand(ValueBlockOperation.INC, (byte)blockNumber, valueBytes);
            lastApduResponse = apduPlayer.ExecuteApduCommand(valueOperationCommand);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool DecrementValueBlock(int blockNumber, Int32 value)
        {
            byte[] valueBytes = ByteArray.ReverseBuffer(BitConverter.GetBytes(value));
            ValueBlockOperationCommand valueOperationCommand = new ValueBlockOperationCommand(ValueBlockOperation.DEC, (byte)blockNumber, valueBytes);
            lastApduResponse = apduPlayer.ExecuteApduCommand(valueOperationCommand);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool CopyValueBlock(int srceBlockNumber, int destBlockNumber)
        {
            CopyValueBlockCommand copyBlockCommand = new CopyValueBlockCommand((byte)srceBlockNumber, (byte)destBlockNumber);
            lastApduResponse = apduPlayer.ExecuteApduCommand(copyBlockCommand);

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }

        public bool ReadValueBlock(int blockNumber, out Int32 value)
        {
            value = 0;
            ReadValueBlockCommand readValueBlockCommand = new ReadValueBlockCommand((byte)blockNumber);
            lastApduResponse = apduPlayer.ExecuteApduCommand(readValueBlockCommand);

            if (lastApduResponse.Status == MifareStatus.SUCCESS)
            {
                byte[] valueBytes = ByteArray.ReverseBuffer(lastApduResponse.Data);
                value = BitConverter.ToInt32(valueBytes, 0);
            }

            return lastApduResponse.Status == MifareStatus.SUCCESS;
        }
    }
}
