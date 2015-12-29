/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using Core.Utility;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace Core.Smartcard
{
	/// <summary>
	/// Implementation of ICard using native (P/Invoke) interoperability for PC/SC
	/// </summary>
	public class CardNative : CardBase, IDisposable
	{
        private IntPtr context = IntPtr.Zero;
        private IntPtr cardHandle = IntPtr.Zero;
		private	UInt32	protocol = (uint) PROTOCOL.T0;
		private	int	lastError = 0;

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

			lastError = PCSC.SCardListReaders(context, null, szListReaders, out pchReaders);
			if (lastError == 0)
			{
				szListReaders = Marshal.AllocHGlobal((int) pchReaders);
				lastError = PCSC.SCardListReaders(context, null, szListReaders, out pchReaders);
				if (lastError == 0)
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

            ThrowSmartcardException("SCardListReaders", lastError);

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
			IntPtr hContext = Marshal.AllocHGlobal(Marshal.SizeOf(context));

            lastError = PCSC.SCardEstablishContext((uint)Scope, IntPtr.Zero, IntPtr.Zero, hContext);
			if (lastError != 0)
			{
				Marshal.FreeHGlobal(hContext);
                ThrowSmartcardException("SCardEstablishContext", lastError);
			}

            context = Marshal.ReadIntPtr(hContext);

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
            if (PCSC.SCardIsValidContext(context) == PCSC.SCARD_S_SUCCESS)
			{
                lastError = PCSC.SCardReleaseContext(context);
                ThrowSmartcardException("SCardReleaseContext", lastError);

                context = IntPtr.Zero;
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

			IntPtr	hCard = Marshal.AllocHGlobal(Marshal.SizeOf(cardHandle));
			IntPtr	pProtocol = Marshal.AllocHGlobal(Marshal.SizeOf(protocol));

            lastError = PCSC.SCardConnect(context, 
				Reader, 
				(uint) ShareMode, 
				(uint) PreferredProtocols, 
				hCard,
				pProtocol);

			if (lastError != 0)
			{
				Marshal.FreeHGlobal(hCard);
				Marshal.FreeHGlobal(pProtocol);
                ThrowSmartcardException("SCardConnect", lastError);
			}

            cardHandle = Marshal.ReadIntPtr(hCard);
			protocol = (uint) Marshal.ReadInt32(pProtocol);

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
            if (PCSC.SCardIsValidContext(context) == PCSC.SCARD_S_SUCCESS)
			{
                lastError = PCSC.SCardDisconnect(cardHandle, (uint)Disposition);
                cardHandle = IntPtr.Zero;

                try
                {
                    ThrowSmartcardException("SCardDisconnect", lastError);
                }
                finally
                {
                    ReleaseContext();
                }
			}
		}

        /// <summary>
        /// Wraps the PCSC function SCardControl
        /// </summary>
        /// <param name="apduCmd">Command to send</param>
        /// <returns>Command response</returns>
        public override ControlResponse Control(ControlCommand controlCmd)
        {
            return Control(cardHandle, controlCmd);
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
            // Must verify that it works with all type of cards
            uint outputLength =  (ApduCmd.Le == 0) 
                ? APDUResponse.MAX_LENGHT 
                :(ApduCmd.Le + APDUResponse.SW_LENGTH);
			byte[]	ApduBuffer = null;
            byte[] ApduResponse = new byte[outputLength];

			PCSC.SCard_IO_Request	ioRequest = new PCSC.SCard_IO_Request();
			ioRequest.Protocol = protocol;
            ioRequest.PciLength = (uint)Marshal.SizeOf(ioRequest);

			// Build the command APDU
			if (ApduCmd.Data == null)
			{
                ApduBuffer = new byte[APDUCommand.APDU_MIN_LENGTH + 1]; // Pass the Le = 0 as well
                ApduBuffer[4] = (byte)ApduCmd.Le;
			}
			else
			{
                int apduBufferLengh = (ApduCmd.Class == 0x90)
                    ? APDUCommand.APDU_MIN_LENGTH + 1 + ApduCmd.Data.Length + 1
                    : APDUCommand.APDU_MIN_LENGTH + 1 + ApduCmd.Data.Length;

				ApduBuffer = new byte[apduBufferLengh];
                Buffer.BlockCopy(ApduCmd.Data, 0, ApduBuffer, APDUCommand.APDU_MIN_LENGTH + 1, ApduCmd.Data.Length);
				ApduBuffer[APDUCommand.APDU_MIN_LENGTH] = (byte) ApduCmd.Data.Length;
                if (ApduCmd.Class == 0x90)
                {
                    ApduBuffer[apduBufferLengh - 1] = 0;
                }
			}

			ApduBuffer[0] = ApduCmd.Class;
			ApduBuffer[1] = ApduCmd.Ins;
			ApduBuffer[2] = ApduCmd.P1;
			ApduBuffer[3] = ApduCmd.P2;

            lastError = PCSC.SCardTransmit(cardHandle, ref ioRequest, ApduBuffer, (uint)ApduBuffer.Length, IntPtr.Zero, ApduResponse, out outputLength);
            ThrowSmartcardException("SCardTransmit", lastError);

            byte[] responseData = new byte[outputLength];
            Buffer.BlockCopy(ApduResponse, 0, responseData, 0, (int)outputLength); 
            string apduString = string.Format("APDU Buffer[{0}] => Response[{1}]", ByteArray.ToString(ApduBuffer), ByteArray.ToString(responseData));
            apduTrace.Add(apduString);
            Trace.WriteLine(apduString);

			return new APDUResponse(responseData);
		}

        /// <summary>
        /// Wraps the PSCS function
        /// LONG SCardBeginTransaction(
        ///     SCARDHANDLE hCard
        //  );
        /// </summary>
        public override void BeginTransaction()
        {
            if (PCSC.SCardIsValidContext(context) == PCSC.SCARD_S_SUCCESS)
            {
                lastError = PCSC.SCardBeginTransaction(cardHandle);
                ThrowSmartcardException("SCardBeginTransaction", lastError);
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
            if (PCSC.SCardIsValidContext(context) == PCSC.SCARD_S_SUCCESS)
            {
                lastError = PCSC.SCardEndTransaction(cardHandle, (UInt32)Disposition);
                ThrowSmartcardException("SCardEndTransaction", lastError);
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

            lastError = PCSC.SCardGetAttrib(cardHandle, AttribId, attr, out attrLen);
            ThrowSmartcardException("SCardGetAttr", lastError);

            if (attrLen != 0)
            {
                attr = new byte[attrLen];
                lastError = PCSC.SCardGetAttrib(cardHandle, AttribId, attr, out attrLen);
                ThrowSmartcardException("SCardGetAttr", lastError);
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
        protected override void RunCardDetection(string reader)
        {
            IntPtr hContext = IntPtr.Zero;    // Local context
            IntPtr phContext = IntPtr.Zero;

            try
            {
                phContext = Marshal.AllocHGlobal(Marshal.SizeOf(hContext));

                if (PCSC.SCardEstablishContext((uint)SCOPE.User, IntPtr.Zero, IntPtr.Zero, phContext) != PCSC.SCARD_S_SUCCESS)
                    return;

                hContext = Marshal.ReadIntPtr(phContext);
                Marshal.FreeHGlobal(phContext);

                UInt32 nbReaders = 1;
                PCSC.SCard_ReaderState[] readerState = new PCSC.SCard_ReaderState[nbReaders];

                readerState[0].CurrentState = (UInt32)PCSC.CARD_STATE.UNAWARE;
                readerState[0].Reader = reader;

                // Card detection loop
                while (true)
                {
                    if (PCSC.SCardGetStatusChange(hContext, WAIT_TIME, readerState, nbReaders) == PCSC.SCARD_S_SUCCESS)
                    {
                        UInt32 currentState = readerState[0].CurrentState;      // the old state (as known by the application)
                        UInt32 eventState = readerState[0].EventState;          // the new state (as known by Windows)

                        // fatal status codes, stop monitoring for insertion/removal
                        if (((eventState & (uint)PCSC.CARD_STATE.UNKNOWN) == (uint)PCSC.CARD_STATE.UNKNOWN) ||      // reader is unknown
                            ((eventState & (uint)PCSC.CARD_STATE.IGNORE) == (uint)PCSC.CARD_STATE.IGNORE) ||      // reader should be ignored
                            ((eventState & (uint)PCSC.CARD_STATE.UNAVAILABLE) == (uint)PCSC.CARD_STATE.UNAVAILABLE))    // reader is unavailable
                            m_bRunCardDetection = false;

                        if (readerState[0].CurrentState != (uint)PCSC.CARD_STATE.UNAWARE)
                        {
                            bool cardIsPresent = ((eventState & (uint)PCSC.CARD_STATE.PRESENT) == (uint)PCSC.CARD_STATE.PRESENT);
                            bool cardWasPresent = ((currentState & (uint)PCSC.CARD_STATE.PRESENT) == (uint)PCSC.CARD_STATE.PRESENT);

                            if (cardIsPresent && !cardWasPresent)
                                CardInserted(reader);

                            if (cardWasPresent && !cardIsPresent)
                                CardRemoved(reader);
                        }

                        // The current state is now the event state
                        readerState[0].CurrentState = eventState;
                    }

                    if (m_bRunCardDetection == false)
                        break;

                    Thread.Sleep(100);
                } // while
            }
            catch
            {
            }
            finally
            {
                if (phContext != IntPtr.Zero)
                    Marshal.FreeHGlobal(phContext);

                if (hContext != IntPtr.Zero)
                    PCSC.SCardReleaseContext(hContext);
            }
        }

        public override void Dispose()
        {
            Disconnect(DISCONNECT.Unpower);

            ReleaseContext();
        }
    }
}