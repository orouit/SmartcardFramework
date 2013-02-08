using System;

namespace GemCard
{
    /// <summary>
    /// Delegate for the CardInserted event
    /// </summary>
    public delegate void CardInsertedEventHandler(object sender, string reader);

    /// <summary>
    /// Delegate for the CardRemoved event
    /// </summary>
    public delegate void CardRemovedEventHandler(object sender, string reader);


	/// <summary>
	/// This interface gives access to the basic card functions. It must be implemented by a class.
	/// </summary>
	public interface	ICard
	{
        /// <summary>
        /// Event handler for the card insertion
        /// </summary>
        event CardInsertedEventHandler OnCardInserted;

        /// <summary>
        /// Event handler for the card removal
        /// </summary>
        event CardRemovedEventHandler OnCardRemoved;

		/// <summary>
		/// Wraps the PCSC funciton
		/// LONG SCardListReaders(SCARDCONTEXT hContext, 
		///		LPCTSTR mszGroups, 
		///		LPTSTR mszReaders, 
		///		LPDWORD pcchReaders 
		///	);
		/// </summary>
		/// <returns>A string array of the readers</returns>
		string[] ListReaders();

		/// <summary>
		///  Wraps the PCSC function
		///  LONG SCardConnect(
		///		IN SCARDCONTEXT hContext,
		///		IN LPCTSTR szReader,
		///		IN DWORD dwShareMode,
		///		IN DWORD dwPreferredProtocols,
		///		OUT LPSCARDHANDLE phCard,
		///		OUT LPDWORD pdwActiveProtocol
		///	);
		/// </summary>
		/// <param name="Reader"></param>
		/// <param name="ShareMode"></param>
		/// <param name="PreferredProtocols"></param>
		void Connect(string Reader, SHARE ShareMode, PROTOCOL PreferredProtocols);

		/// <summary>
		/// Wraps the PCSC function
		///	LONG SCardDisconnect(
		///		IN SCARDHANDLE hCard,
		///		IN DWORD dwDisposition
		///	);
		/// </summary>
		/// <param name="Disposition"></param>
		void Disconnect(DISCONNECT Disposition);

		/// <summary>
		/// Wraps the PCSC function
		/// LONG SCardTransmit(
		///		SCARDHANDLE hCard,
		///		LPCSCARD_I0_REQUEST pioSendPci,
		///		LPCBYTE pbSendBuffer,
		///		DWORD cbSendLength,
		///		LPSCARD_IO_REQUEST pioRecvPci,
		///		LPBYTE pbRecvBuffer,
		///		LPDWORD pcbRecvLength
		///	);
		/// </summary>
		/// <param name="ApduCmd">APDUCommand object with the APDU to send to the card</param>
		/// <returns>An APDUResponse object with the response from the card</returns>
		APDUResponse Transmit(APDUCommand ApduCmd);

        /// <summary>
        /// Wraps the PSCS function
        /// LONG SCardBeginTransaction(
        ///     SCARDHANDLE hCard
        //  );
        /// </summary>
        void BeginTransaction();

        /// <summary>
        /// Wraps the PCSC function
        /// LONG SCardEndTransaction(
        ///     SCARDHANDLE hCard,
        ///     DWORD dwDisposition
        /// );
        /// </summary>
        void EndTransaction(DISCONNECT Disposition);

        /// <summary>
        /// Gets the attributes of the card
        /// </summary>
        /// <param name="AttribId">Identifier for the Attribute to get</param>
        /// <returns>Attribute content</returns>
        byte[] GetAttribute(UInt32 AttribId);
	}

	/// <summary>
	/// SCOPE context
	/// </summary>
	public enum SCOPE
	{
		/// <summary>
		/// The context is a user context, and any database operations are performed within the
		/// domain of the user.
		/// </summary>
		User,		

		/// <summary>
		/// The context is that of the current terminal, and any database operations are performed
		/// within the domain of that terminal.  (The calling application must have appropriate
		/// access permissions for any database actions.)
		/// </summary>
		Terminal,	

		/// <summary>
		/// The context is the system context, and any database operations are performed within the
		/// domain of the system.  (The calling application must have appropriate access
		/// permissions for any database actions.)
		/// </summary>
		System	
	}

	/// <summary>
	/// SHARE mode enumeration
	/// </summary>
	public	enum SHARE
	{
		/// <summary>
		/// This application is not willing to share this card with other applications.
		/// </summary>
		Exclusive = 1,	

		/// <summary>
		/// This application is willing to share this card with other applications.
		/// </summary>
		Shared,			

		/// <summary>
		/// This application demands direct control of the reader, so it is not available to other applications.
		/// </summary>
		Direct			
	}


	/// <summary>
	/// PROTOCOL enumeration
	/// </summary>
	public	enum	PROTOCOL
	{
		/// <summary>
		/// There is no active protocol.
		/// </summary>
		Undefined	= 0x00000000,	

		/// <summary>
		/// T=0 is the active protocol.
		/// </summary>
		T0			= 0x00000001,	

		/// <summary>
		/// T=1 is the active protocol.
		/// </summary>
		T1			= 0x00000002,	

		/// <summary>
		/// Raw is the active protocol.
		/// </summary>
		Raw			= 0x00010000, 
		Default		= unchecked ((int) 0x80000000),  // Use implicit PTS.

		/// <summary>
		/// T=1 or T=0 can be the active protocol
		/// </summary>
		T0orT1		= T0 | T1
	}


	/// <summary>
	/// DISCONNECT action enumeration
	/// </summary>
	public	enum	DISCONNECT
	{
		/// <summary>
		/// Don't do anything special on close
		/// </summary>
		Leave,		

		/// <summary>
		/// Reset the card on close
		/// </summary>
		Reset,		

		/// <summary>
		/// Power down the card on close
		/// </summary>
		Unpower,	

		/// <summary>
		/// Eject(!) the card on close
		/// </summary>
		Eject	
	}
}
