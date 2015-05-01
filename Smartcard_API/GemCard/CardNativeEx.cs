/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Core.Smartcard
{
    class CardNativeEx : CardBaseEx, IDisposable
    {
        private IntPtr m_hContext = IntPtr.Zero;
        private IntPtr m_hCard = IntPtr.Zero;
		private	UInt32	m_nProtocol = (uint) PROTOCOL.T0;
		private	int	m_nLastError = 0;
        const int SCARD_S_SUCCESS = 0;

		/// <summary>
		/// Default constructor
		/// </summary>
		public CardNativeEx()
		{
		}

		/// <summary>
		/// Object destruction
		/// </summary>
		~CardNativeEx()
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

			m_nLastError = PCSC.SCardListReaders(m_hContext, null, szListReaders, out pchReaders);
			if (m_nLastError == 0)
			{
				szListReaders = Marshal.AllocHGlobal((int) pchReaders);
				m_nLastError = PCSC.SCardListReaders(m_hContext, null, szListReaders, out pchReaders);
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

            ThrowSmartcardException("SCardListReaders", m_nLastError);

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

			m_nLastError = PCSC.SCardEstablishContext((uint) Scope, IntPtr.Zero, IntPtr.Zero, hContext);
			if (m_nLastError != 0)
			{
				Marshal.FreeHGlobal(hContext);
                ThrowSmartcardException("SCardEstablishContext", m_nLastError);
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
			if (PCSC.SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
			{
				m_nLastError = PCSC.SCardReleaseContext(m_hContext);
                ThrowSmartcardException("SCardReleaseContext", m_nLastError);

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

			m_nLastError = PCSC.SCardConnect(m_hContext, 
				Reader, 
				(uint) ShareMode, 
				(uint) PreferredProtocols, 
				hCard,
				pProtocol);

			if (m_nLastError != 0)
			{
				Marshal.FreeHGlobal(hCard);
				Marshal.FreeHGlobal(pProtocol);
                ThrowSmartcardException("SCardConnect", m_nLastError);
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
            if (PCSC.SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
			{
				m_nLastError = PCSC.SCardDisconnect(m_hCard, (uint) Disposition);
                m_hCard = IntPtr.Zero;

                try
                {
                    ThrowSmartcardException("SCardDisconnect", m_nLastError);
                }
                finally
                {
                    ReleaseContext();
                }
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
			PCSC.SCard_IO_Request	ioRequest = new PCSC.SCard_IO_Request();
			ioRequest.Protocol = m_nProtocol;
			ioRequest.PciLength = 8;

			// Build the command APDU
			if (ApduCmd.Data == null)
			{
				ApduBuffer = new byte[APDUCommand.APDU_MIN_LENGTH + ((ApduCmd.Le != 0) ? 1 : 0)];

                if (ApduCmd.Le != 0)
                {
                    ApduBuffer[4] = (byte)ApduCmd.Le;
                }
			}
			else
			{
				ApduBuffer = new byte[APDUCommand.APDU_MIN_LENGTH + 1 + ApduCmd.Data.Length];
                Buffer.BlockCopy(ApduCmd.Data, 0, ApduBuffer, APDUCommand.APDU_MIN_LENGTH + 1, ApduCmd.Data.Length);
				ApduBuffer[APDUCommand.APDU_MIN_LENGTH] = (byte) ApduCmd.Data.Length;
			}

			ApduBuffer[0] = ApduCmd.Class;
			ApduBuffer[1] = ApduCmd.Ins;
			ApduBuffer[2] = ApduCmd.P1;
			ApduBuffer[3] = ApduCmd.P2;

			m_nLastError = PCSC.SCardTransmit(m_hCard, ref ioRequest, ApduBuffer, (uint) ApduBuffer.Length, IntPtr.Zero, ApduResponse, out RecvLength);
            ThrowSmartcardException("SCardTransmit", m_nLastError);
			
			byte[] ApduData = new byte[RecvLength];
            Buffer.BlockCopy(ApduResponse, 0, ApduData, 0, (int)RecvLength); 

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
            if (PCSC.SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
            {
                m_nLastError = PCSC.SCardBeginTransaction(m_hCard);
                ThrowSmartcardException("SCardBeginTransaction", m_nLastError);
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
            if (PCSC.SCardIsValidContext(m_hContext) == SCARD_S_SUCCESS)
            {
                m_nLastError = PCSC.SCardEndTransaction(m_hCard, (UInt32)Disposition);
                ThrowSmartcardException("SCardEndTransaction", m_nLastError);
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

            m_nLastError = PCSC.SCardGetAttrib(m_hCard, AttribId, attr, out attrLen);
            ThrowSmartcardException("SCardGetAttr", m_nLastError);

            if (attrLen != 0)
            {
                attr = new byte[attrLen];
                m_nLastError = PCSC.SCardGetAttrib(m_hCard, AttribId, attr, out attrLen);
                ThrowSmartcardException("SCardGetAttr", m_nLastError);
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
            bool bFirstLoop = true;
            IntPtr hContext = IntPtr.Zero;    // Local context
            IntPtr phContext;

            phContext = Marshal.AllocHGlobal(Marshal.SizeOf(hContext));

            if (PCSC.SCardEstablishContext((uint) SCOPE.User, IntPtr.Zero, IntPtr.Zero, phContext) == 0)
            {
                hContext = Marshal.ReadIntPtr(phContext);
                Marshal.FreeHGlobal(phContext);

                UInt32 nbReaders = 1;
                PCSC.SCard_ReaderState[] readerState = new PCSC.SCard_ReaderState[nbReaders];

                readerState[0].CurrentState = (UInt32)PCSC.CARD_STATE.UNAWARE;
                readerState[0].Reader = reader;

                UInt32 eventState;
                UInt32 currentState = readerState[0].CurrentState;

                // Card detection loop
                do
                {
                    if (PCSC.SCardGetStatusChange(hContext, WAIT_TIME
                        , readerState, nbReaders) == 0)
                    {
                        eventState = readerState[0].EventState;
                        currentState = readerState[0].CurrentState;

                        if (bFirstLoop)
                        {
                            // Check if a card is already inserted
                            if ((eventState & (uint)PCSC.CARD_STATE.PRESENT) == (uint)PCSC.CARD_STATE.PRESENT)
                            {
                                CardInserted(reader);
                            }
                        } 
                        else if (((eventState & (uint)PCSC.CARD_STATE.CHANGED) == (uint)PCSC.CARD_STATE.CHANGED) && !bFirstLoop)    
                        {
                            // State has changed
                            if ((eventState & (uint)PCSC.CARD_STATE.EMPTY) == (uint)PCSC.CARD_STATE.EMPTY)
                            {
                                // There is no card, card has been removed -> Fire CardRemoved event
                                CardRemoved((string)reader);
                            }

                            if (((eventState & (uint)PCSC.CARD_STATE.PRESENT) == (uint)PCSC.CARD_STATE.PRESENT) &&
                                ((eventState & (uint)PCSC.CARD_STATE.PRESENT) != (currentState & (uint)PCSC.CARD_STATE.PRESENT)))
                            {
                                // There is a card in the reader -> Fire CardInserted event
                                CardInserted(reader);
                            }

                            if ((eventState & (uint)PCSC.CARD_STATE.ATRMATCH) == (uint)PCSC.CARD_STATE.ATRMATCH)
                            {
                                // There is a card in the reader and it matches the ATR we were expecting-> Fire CardInserted event
                                CardInserted(reader);
                            }
                        }

                        // The current stateis now the event state
                        readerState[0].CurrentState = eventState;

                        bFirstLoop = false;
                    }

                    Thread.SpinWait(50000);

                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                }
                while (true);    // Exit on request
            }
            else
            {
                Marshal.FreeHGlobal(phContext);
                throw new Exception("PC/SC error");
            }

            PCSC.SCardReleaseContext(hContext);
            cancellationToken.ThrowIfCancellationRequested();
        }

        protected override void Disposing()
        {
            base.Disposing();

            Disconnect(DISCONNECT.Unpower);
            ReleaseContext();
        }

        private void ThrowSmartcardException(string methodName, long errCode)
        {
            if (errCode != 0)
            {
                throw new SmartCardException(string.Format("{0} error: {1:X02}", methodName, errCode));
            }
        }
    }
}
