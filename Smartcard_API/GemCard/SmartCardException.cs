/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;

namespace Core.Smartcard
{
	/// <summary>
	///  Smart card exceptions
	/// </summary>
    [Serializable]
	public class SmartCardException : Exception
	{
		public SmartCardException() : base("Smart card exception")
		{
		}

		public SmartCardException(string Message) : base(Message)
		{
		}
	}

    [Serializable]
    public class ApduCommandException : Exception
    {
		public const string
			NotValidDocument = "The file is not a valid APDU command document",
			NoSuchCommand = "No such APDU command in the document",
            ParamLeFormat = "Le parameter format is not correct",
            ParamP3Format = "P3 parameter format is not correct",
            NoSuchSequence = "No such APDU sequence in the document", 
            MissingApduOrCommand = "An Apdu or a Sequence is missing in this Sequence";
            
		public ApduCommandException() : base("APDU command exception")
		{
		}

		public ApduCommandException(string Message) : base(Message)
		{
		}
    }
}
