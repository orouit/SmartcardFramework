/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
namespace Core.Smartcard.DESFire
{
    public enum DESFireStatus : byte
    {
        OPERATION_OK = 0x00,
        ILLEGAL_COMMAND_CODE = 0x1C,
        NO_SUCH_KEY = 0x40, 
        LENGTH_ERROR = 0x7E,
        PERMISSION_DENIED = 0x9D,
        AUTHENTICATION_ERROR = 0xAE,
        ADDITIONAL_FRAME = 0xAF
    }

    public class DESFireHelper
    {
        const byte DESFIRE_SW1 = 0x91;
        const int RND_SIZE = 8;

        private APDUPlayer apduPlayer;
        private APDUResponse lastApduResponse = null;

        public DESFireHelper(ICard card)
        {
            apduPlayer = new APDUPlayer(card);
        }

        protected DESFireStatus ExecuteDESFireCommand(DESFireWrappingCommand desFireCommand, out byte[] responseData)
        {
            DESFireStatus status = DESFireStatus.ILLEGAL_COMMAND_CODE;
            lastApduResponse = apduPlayer.ExecuteApduCommand(desFireCommand);
            responseData = null;

            if (lastApduResponse.SW1 == DESFIRE_SW1)
            {
                status = (DESFireStatus) lastApduResponse.SW2;
                responseData = lastApduResponse.Data;
            }
            else
            {
                throw new SmartCardException(string.Format("Incorrect SW1[{0}] for a DESFire wrapping", lastApduResponse.SW1));
            }

            return status;
        }

        public  DESFireStatus Authenticate(int keyNo, out byte[] encRndB)
        {
            AuthenticateCommand authenticateCommand = new AuthenticateCommand((byte)keyNo);
            DESFireStatus status = ExecuteDESFireCommand(authenticateCommand, out encRndB);
            if (status == DESFireStatus.ADDITIONAL_FRAME)
            {
                encRndB = lastApduResponse.Data;
            }

            return status;
        }

        public DESFireStatus AdditionalFrame(byte[] data, out byte[] dataOut)
        {
            AdditionalFrameCommand additionalFrameCommand = new AdditionalFrameCommand(data);
            DESFireStatus status = ExecuteDESFireCommand(additionalFrameCommand, out dataOut);
            if (status == DESFireStatus.ADDITIONAL_FRAME)
            {
                dataOut = lastApduResponse.Data;
            }

            return status;
        }
    }
}
