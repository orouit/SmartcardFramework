using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GemCard
{
    /// <summary>
    /// CARD_STATE enumeration, used by the PC/SC function SCardGetStatusChanged
    /// </summary>
    enum CARD_STATE
    {
        UNAWARE = 0x00000000,
        IGNORE = 0x00000001,
        CHANGED = 0x00000002,
        UNKNOWN = 0x00000004,
        UNAVAILABLE = 0x00000008,
        EMPTY = 0x00000010,
        PRESENT = 0x00000020,
        ATRMATCH = 0x00000040,
        EXCLUSIVE = 0x00000080,
        INUSE = 0x00000100,
        MUTE = 0x00000200,
        UNPOWERED = 0x00000400
    }

	/// <summary>
	/// Wraps the SCARD_IO_STRUCTURE
    ///  
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct	SCard_IO_Request
	{
		public UInt32	m_dwProtocol;
		public UInt32	m_cbPciLength;
	}


    /// <summary>
    /// Wraps theSCARD_READERSTATE structure of PC/SC
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct SCard_ReaderState
    {
        public string m_szReader;
        public IntPtr m_pvUserData;
        public UInt32 m_dwCurrentState;
        public UInt32 m_dwEventState;
        public UInt32 m_cbAtr;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
        public byte[] m_rgbAtr;
    }
    
	/// <summary>
	/// Implementation of ICard using native (P/Invoke) interoperability for PC/SC
	/// </summary>
	public class CardNative : CardBase, IDisposable
	{
        private IntPtr m_hContext = IntPtr.Zero;
        private IntPtr m_hCard = IntPtr.Zero;
		private	UInt32	m_nProtocol = (uint) PROTOCOL.T0;
		private	int	m_nLastError = 0;
        const int SCARD_S_SUCCESS = 0;

		#region PCSC_FUNCTIONS
        /// <summary>
        /// Native SCardGetStatusChanged from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <param name="dwTimeout"></param>
        /// <param name="rgReaderStates"></param>
        /// <param name="cReaders"></param>
        /// <returns></returns>
        [DllImport("winscard.dll", SetLastError = true)]
        internal static extern int SCardGetStatusChange(IntPtr hContext,
            UInt32 dwTimeout,
            [In,Out] SCard_ReaderState[] rgReaderStates,
            UInt32 cReaders);

		/// <summary>
		/// Native SCardListReaders function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="mszGroups"></param>
		/// <param name="mszReaders"></param>
		/// <param name="pcchReaders"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true)]
        internal static extern int SCardListReaders(IntPtr hContext,
			[MarshalAs(UnmanagedType.LPTStr)] string mszGroups,
			IntPtr mszReaders,
            out UInt32 pcchReaders);

		/// <summary>
		/// Native SCardEstablishContext function from winscard.dll
		/// </summary>
		/// <param name="dwScope"></param>
		/// <param name="pvReserved1"></param>
		/// <param name="pvReserved2"></param>
		/// <param name="phContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true)]
		internal	static	extern	int	SCardEstablishContext(UInt32 dwScope,
			IntPtr pvReserved1,
			IntPtr pvReserved2,
			IntPtr phContext);

		/// <summary>
		/// Native SCardReleaseContext function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true)]
        internal static extern int SCardReleaseContext(IntPtr hContext);

        /// <summary>
        /// Native SCardIsValidContext function from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport("winscard.dll", SetLastError = true)]
        internal static extern int SCardIsValidContext(IntPtr hContext);

		/// <summary>
		/// Native SCardConnect function from winscard.dll
		/// </summary>
		/// <param name="hContext"></param>
		/// <param name="szReader"></param>
		/// <param name="dwShareMode"></param>
		/// <param name="dwPreferredProtocols"></param>
		/// <param name="phCard"></param>
		/// <param name="pdwActiveProtocol"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true, CharSet=CharSet.Auto)]
        internal static extern int SCardConnect(IntPtr hContext,
			[MarshalAs(UnmanagedType.LPTStr)] string szReader,
			UInt32	dwShareMode, 
			UInt32	dwPreferredProtocols,
			IntPtr	phCard, 
			IntPtr	pdwActiveProtocol);

		/// <summary>
		/// Native SCardDisconnect function from winscard.dll
		/// </summary>
		/// <param name="hCard"></param>
		/// <param name="dwDisposition"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true)]
        internal static extern int SCardDisconnect(IntPtr hCard,
			UInt32 dwDisposition);

		/// <summary>
		/// Native SCardTransmit function from winscard.dll
		/// </summary>
		/// <param name="hCard"></param>
		/// <param name="pioSendPci"></param>
		/// <param name="pbSendBuffer"></param>
		/// <param name="cbSendLength"></param>
		/// <param name="pioRecvPci"></param>
		/// <param name="pbRecvBuffer"></param>
		/// <param name="pcbRecvLength"></param>
		/// <returns></returns>
		[DllImport("winscard.dll", SetLastError=true)]
        internal static extern int SCardTransmit(IntPtr hCard,
			[In] ref SCard_IO_Request pioSendPci,
			byte[] pbSendBuffer,
			UInt32 cbSendLength,
			IntPtr pioRecvPci,
			[Out] byte[] pbRecvBuffer,
			out UInt32 pcbRecvLength
			);

        /// <summary>
        /// Native SCardBeginTransaction function of winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport("winscard.dll", SetLastError = true)]
        internal static extern int SCardBeginTransaction(IntPtr hContext);

        /// <summary>
        /// Native SCardEndTransaction function of winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport("winscard.dll", SetLastError = true)]
        internal static extern int SCardEndTransaction(IntPtr hContext, UInt32 dwDisposition);

        [DllImport("winscard.dll", SetLastError = true)]
        internal static extern int SCardGetAttrib(IntPtr hCard,
            UInt32 dwAttribId,
            [Out] byte[] pbAttr,
            out UInt32 pcbAttrLen);

        #endregion WINSCARD_FUNCTIONS

		/// <summary>
		/// Default constructor
		/// </summary>
		public CardNative()
		{
		}

		/// <summary>
		/// Object destruction
		/// </summary>
		~CardNative()
		{
			Disconnect(DISCONNECT.Unpower);

			ReleaseContext();
		}

		#region ICard Members

		/// <summary>
		/// Wraps the PCSC function
		/// LONG SCardListReaders(SCARDCONTEXT hContext, 
		///		LPCTSTR mszGroups, 
		///		LPTSTR mszReaders, 
		///		LPDWORD pcchReaders 
		///	);
		/// </summary>
		/// <returns>A string array of the readers</returns>
		public	override string[]	ListReaders()
		{
			EstablishContext(SCOPE.User);

			string[]	sListReaders = null;
            UInt32 pchReaders = 0;
			IntPtr	szListReaders = IntPtr.Zero;

			m_nLastError = SCardListReaders(m_hContext, null, szListReaders, out pchReaders);
			if (m_nLastError == 0)
			{
				szListReaders = Marshal.AllocHGlobal((int) pchReaders);
				m_nLastError = SCardListReaders(m_hContext, null, szListReaders, out pchReaders);
				if (m_nLastError == 0)
				{
					char[] caReadersData = new char[pchReaders];
					int	nbReaders = 0;
					for (int nI = 0; nI < pchReaders; nI++)
					{
						caReadersData[nI] = (char) Marshal.ReadByte(szListReaders, nI);

						if (caReadersData[nI] == 0)
							nbReaders++;
					}

					// Remove last 0
					--nbReaders;

					if (nbReaders != 0)
					{
						sListReaders = new string[nbReaders];
						char[] caReader = new char[pchReaders];
						int	nIdx = 0;
						int	nIdy = 0;
						int	nIdz = 0;
						// Get the nJ string from the multi-string

						while(nIdx < pchReaders - 1)
						{
							caReader[nIdy] = caReadersData[nIdx];
							if (caReader[nIdy] == 0)
							{
								sListReaders[nIdz] = new string(caReader, 0, nIdy);
								++nIdz;
								nIdy = 0;
								caReader = new char[pchReaders];
							}
							else
								++nIdy;

							++nIdx;
						}
					}

				}

				Marshal.FreeHGlobal(szListReaders);
			}

			ReleaseContext();

			return sListReaders;
		}

		/// <summary>
		/// Wraps the PCSC function 
		/// LONG SCardEstablishContext(
		///		IN DWORD dwScope,
		///		IN LPCVOID pvReserved1,
		///		IN LPCVOID pvReserved2,
		///		OUT LPSCARDCONTEXT phContext
		///	);
		/// </summary>
		/// <param name="Scope"></param>
		public void EstablishContext(SCOPE Scope)
		{
			IntPtr hContext = Marshal.AllocHGlobal(Marshal.SizeOf(m_hContext));

			m_nLastError = SCardEstablishContext((uint) Scope, IntPtr.Zero, IntPtr.Zero, hContext);
			if (m_nLastError != 0)
			{
				string msg = "SCardEstablishContext error: " + m_nLastError;

				Marshal.FreeHGlobal(hContext);
				throw new Exception(msg);
			}

            m_hContext = Marshal.ReadIntPtr(hContext);

			Marshal.FreeHGlobal(hContext);
		}


		/// <summary>
		/// Wraps the PCSC function
		/// LONG SCardReleaseContext(
		///		IN SCARDCONTEXT hContext
		///	);
		/// </summary>
		public void ReleaseContext()
		{
			if (SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
			{
				m_nLastError = SCardReleaseContext(m_hContext);

				if (m_nLastError != 0)
				{
					string	msg = "SCardReleaseContext error: " + m_nLastError;
					throw new Exception(msg);
				}

                m_hContext = IntPtr.Zero;
			}
		}

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
		public override void Connect(string Reader, SHARE ShareMode, PROTOCOL PreferredProtocols)
		{
			EstablishContext(SCOPE.User);

			IntPtr	hCard = Marshal.AllocHGlobal(Marshal.SizeOf(m_hCard));
			IntPtr	pProtocol = Marshal.AllocHGlobal(Marshal.SizeOf(m_nProtocol));

			m_nLastError = SCardConnect(m_hContext, 
				Reader, 
				(uint) ShareMode, 
				(uint) PreferredProtocols, 
				hCard,
				pProtocol);

			if (m_nLastError != 0)
			{
				string msg = "SCardConnect error: " + m_nLastError;

				Marshal.FreeHGlobal(hCard);
				Marshal.FreeHGlobal(pProtocol);
				throw new Exception(msg);
			}

            m_hCard = Marshal.ReadIntPtr(hCard);
			m_nProtocol = (uint) Marshal.ReadInt32(pProtocol);

			Marshal.FreeHGlobal(hCard);
			Marshal.FreeHGlobal(pProtocol);
		}

		/// <summary>
		/// Wraps the PCSC function
		///	LONG SCardDisconnect(
		///		IN SCARDHANDLE hCard,
		///		IN DWORD dwDisposition
		///	);
		/// </summary>
		/// <param name="Disposition"></param>
		public override void Disconnect(DISCONNECT Disposition)
		{
            if (SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
			{
				m_nLastError = SCardDisconnect(m_hCard, (uint) Disposition);
                m_hCard = IntPtr.Zero;

				if (m_nLastError != 0)
				{
					string msg = "SCardDisconnect error: " + m_nLastError;
					throw new Exception(msg);
				}

				ReleaseContext();
			}
		}

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
		public override APDUResponse Transmit(APDUCommand ApduCmd)
		{
			uint	RecvLength = (uint) (ApduCmd.Le + APDUResponse.SW_LENGTH);
			byte[]	ApduBuffer = null;
			byte[]	ApduResponse = new byte[ApduCmd.Le + APDUResponse.SW_LENGTH];
			SCard_IO_Request	ioRequest = new SCard_IO_Request();
			ioRequest.m_dwProtocol = m_nProtocol;
			ioRequest.m_cbPciLength = 8;

			// Build the command APDU
			if (ApduCmd.Data == null)
			{
				ApduBuffer = new byte[APDUCommand.APDU_MIN_LENGTH + ((ApduCmd.Le != 0) ? 1 : 0)];

				if (ApduCmd.Le != 0)
					ApduBuffer[4] = (byte) ApduCmd.Le;
			}
			else
			{
				ApduBuffer = new byte[APDUCommand.APDU_MIN_LENGTH + 1 + ApduCmd.Data.Length];

				for (int nI = 0; nI < ApduCmd.Data.Length; nI++)
					ApduBuffer[APDUCommand.APDU_MIN_LENGTH + 1 + nI] = ApduCmd.Data[nI];

				ApduBuffer[APDUCommand.APDU_MIN_LENGTH] = (byte) ApduCmd.Data.Length;
			}

			ApduBuffer[0] = ApduCmd.Class;
			ApduBuffer[1] = ApduCmd.Ins;
			ApduBuffer[2] = ApduCmd.P1;
			ApduBuffer[3] = ApduCmd.P2;

			m_nLastError = SCardTransmit(m_hCard, ref ioRequest, ApduBuffer, (uint) ApduBuffer.Length, IntPtr.Zero, ApduResponse, out RecvLength); 
			if (m_nLastError != 0)
			{
				string msg = "SCardTransmit error: " + m_nLastError;
				throw new Exception(msg);
			}
			
			byte[] ApduData = new byte[RecvLength];

			for (int nI = 0; nI < RecvLength; nI++)
				ApduData[nI] = ApduResponse[nI];

			return new APDUResponse(ApduData);
		}


        /// <summary>
        /// Wraps the PSCS function
        /// LONG SCardBeginTransaction(
        ///     SCARDHANDLE hCard
        //  );
        /// </summary>
        public override void BeginTransaction()
        {
            if (SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
            {
                m_nLastError = SCardBeginTransaction(m_hCard);
                if (m_nLastError != 0)
                {
                    string msg = "SCardBeginTransaction error: " + m_nLastError;
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Wraps the PCSC function
        /// LONG SCardEndTransaction(
        ///     SCARDHANDLE hCard,
        ///     DWORD dwDisposition
        /// );
        /// </summary>
        /// <param name="Disposition">A value from DISCONNECT enum</param>
        public override void EndTransaction(DISCONNECT Disposition)
        {
            if (SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
            {
                m_nLastError = SCardEndTransaction(m_hCard, (UInt32)Disposition);
                if (m_nLastError != 0)
                {
                    string msg = "SCardEndTransaction error: " + m_nLastError;
                    throw new Exception(msg);
                }
            }
        }

        /// <summary>
        /// Gets the attributes of the card
        /// </summary>
        /// <param name="AttribId">Identifier for the Attribute to get</param>
        /// <returns>Attribute content</returns>
        public override byte[] GetAttribute(UInt32 AttribId)
        {
            byte[] attr = null;
            UInt32 attrLen = 0;

            m_nLastError = SCardGetAttrib(m_hCard, AttribId, attr, out attrLen);
            if (m_nLastError == 0)
            {
                if (attrLen != 0)
                {
                    attr = new byte[attrLen];
                    m_nLastError = SCardGetAttrib(m_hCard, AttribId, attr, out attrLen);
                    if (m_nLastError != 0)
                    {
                        string msg = "SCardGetAttr error: " + m_nLastError;
                        throw new Exception(msg);
                    }
                }
            }
            else
            {
                string msg = "SCardGetAttr error: " + m_nLastError;
                throw new Exception(msg);
            }

            return attr;
        }
        #endregion

        /// <summary>
        /// This function must implement a card detection mechanism.
        /// 
        /// When card insertion is detected, it must call the method CardInserted()
        /// When card removal is detected, it must call the method CardRemoved()
        /// 
        /// </summary>
        protected override void RunCardDetection(object Reader)
        {
            bool bFirstLoop = true;
            IntPtr hContext = IntPtr.Zero;    // Local context
            IntPtr phContext;

            phContext = Marshal.AllocHGlobal(Marshal.SizeOf(hContext));

            if (SCardEstablishContext((uint) SCOPE.User, IntPtr.Zero, IntPtr.Zero, phContext) == 0)
            {
                hContext = Marshal.ReadIntPtr(phContext);
                Marshal.FreeHGlobal(phContext);

                UInt32 nbReaders = 1;
                SCard_ReaderState[] readerState = new SCard_ReaderState[nbReaders];

                readerState[0].m_dwCurrentState = (UInt32) CARD_STATE.UNAWARE;
                readerState[0].m_szReader = (string)Reader;

                UInt32 eventState;
                UInt32 currentState = readerState[0].m_dwCurrentState;

                // Card detection loop
                do
                {
                    if (SCardGetStatusChange(hContext, WAIT_TIME
                        , readerState, nbReaders) == 0)
                    {
                        eventState = readerState[0].m_dwEventState;
                        currentState = readerState[0].m_dwCurrentState;

                        // Check state
                        if (((eventState & (uint) CARD_STATE.CHANGED) == (uint) CARD_STATE.CHANGED) && !bFirstLoop)    
                        {
                            // State has changed
                            if ((eventState & (uint) CARD_STATE.EMPTY) == (uint) CARD_STATE.EMPTY)
                            {
                                // There is no card, card has been removed -> Fire CardRemoved event
                                CardRemoved((string)Reader);
                            }

                            if (((eventState & (uint)CARD_STATE.PRESENT) == (uint)CARD_STATE.PRESENT) && 
                                ((eventState & (uint) CARD_STATE.PRESENT) != (currentState & (uint) CARD_STATE.PRESENT)))
                            {
                                // There is a card in the reader -> Fire CardInserted event
                                CardInserted((string)Reader);
                            }

                            if ((eventState & (uint) CARD_STATE.ATRMATCH) == (uint) CARD_STATE.ATRMATCH)
                            {
                                // There is a card in the reader and it matches the ATR we were expecting-> Fire CardInserted event
                                CardInserted((string)Reader);
                            }
                        }

                        // The current stateis now the event state
                        readerState[0].m_dwCurrentState = eventState;

                        bFirstLoop = false;
                    }

                    Thread.Sleep(100);

                    if (m_bRunCardDetection == false)
                        break;
                }
                while (true);    // Exit on request
            }
            else
            {
                Marshal.FreeHGlobal(phContext);
                throw new Exception("PC/SC error");
            }

            SCardReleaseContext(hContext);
        }

        public void Dispose()
        {
            Disconnect(DISCONNECT.Unpower);

            ReleaseContext();
        }
    }
}