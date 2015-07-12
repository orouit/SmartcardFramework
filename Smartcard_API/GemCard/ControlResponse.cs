/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

namespace Core.Smartcard
{
    public class ControlResponse   
    {
        public byte[] Data
        {
            get;
            private set;
        }

        public ControlResponse(byte[] data)
        {
           Data = data;
        }
    }
}
