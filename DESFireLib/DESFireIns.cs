/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard.DESFire
{
    public class DESFireIns
    {
        public static byte NextFrame = 0xAF;
        
        // Security commands
        public static byte
                Authenticate = 0x0A,
                ChangeKeySettings = 0x54,
                GetKeySerttings = 0x45,
                ChangeKey = 0xC4,
                GetKeyVersion = 0x64;

        // PICC level commands
        public static byte
            CreateApplication = 0xCA,
            DeleteApplication = 0xDA,
            GetApplicationIDs = 0x6A,
            SelectApplication = 0x5A,
            FormatPICC = 0xFC,
            GetVersion = 0x60;
    }
}
