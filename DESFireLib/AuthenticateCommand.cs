/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.DESFire
{
    public class AuthenticateCommand : DESFireWrappingCommand
    {
        public AuthenticateCommand(byte keyNo)
            : base(DESFireIns.Authenticate, new byte[] { keyNo })
        {
        }
    }
}
