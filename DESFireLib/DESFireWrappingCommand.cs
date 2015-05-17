/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.DESFire
{
    /// <summary>
    /// Wraps a DESFire command within an APDU type 4
    /// </summary>
    public class DESFireWrappingCommand : APDUCommand
    {
        const byte DESFIRE_CLASS = 0x90;

        public DESFireWrappingCommand(byte desFireIns, byte[] desFireParams)
            : base(DESFIRE_CLASS, desFireIns, 0x00, 0x00, desFireParams, 0x00)
        {
        }
    }
}
