/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;

namespace Core.Smartcard
{
    /// <summary>
    /// This class wraps the data that are sent to the method SCardControl. ControlCode corresponds to the 
    /// parameter dwControlCode and is a 4 bytes data.
    /// ControlData is the data buffer sent to the method. It's length depends on the control command
    /// </summary>
    public class ControlCommand
    {
        const int CODE_LENGTH = 4;

        public byte[] ControlCode
        {
            get;
            set;
        }

        public byte[] ControlData
        {
            get;
            set;
        }

        public ControlCommand(byte[] controlCode, byte[] controlData)
        {
            if (controlCode.Length != CODE_LENGTH)
            {
                throw new ArgumentOutOfRangeException("controlCode buffer must be 4 bytes long");
            }

            ControlCode = controlCode;
            ControlData = controlData;
        }
    }
}
