/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.Mifare
{
    public class AuthenticateBlockCommand : APDUCommand
    {
        public AuthenticateBlockCommand(byte blockNumber, byte keyType, byte keyNumber)
            : base(MifareParam.CLASS, MifareParam.INS_AUTH_BLOCK, 0, 0, new byte[] { 0x01, 0, blockNumber, keyType, keyNumber }, 0)
        {
        }
    }
}
