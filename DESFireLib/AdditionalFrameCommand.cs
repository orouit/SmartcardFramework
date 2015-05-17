/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.DESFire
{
    public class AdditionalFrameCommand : DESFireWrappingCommand
    {
        public AdditionalFrameCommand(byte[] data)
            : base(DESFireIns.NextFrame, data)
        {
        }
    }
}
