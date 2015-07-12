/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Core.Smartcard
{
    internal class PCSC
    {
        private const string WINSCARD_DLL = "winscard.dll";

        public const int SCARD_S_SUCCESS = 0;

        /// <summary>
        /// CARD_STATE enumeration, used by the PC/SC function SCardGetStatusChanged
        /// </summary>
        public enum CARD_STATE
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
        public struct SCard_IO_Request
        {
            public UInt32 Protocol;
            public UInt32 PciLength;
        }

        /// <summary>
        /// Wraps theSCARD_READERSTATE structure of PC/SC
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct SCard_ReaderState
        {
            public string Reader;
            public IntPtr UserData;
            public UInt32 CurrentState;
            public UInt32 EventState;
            public UInt32 Atr;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            public byte[] AtrBytes;
        }

        /// <summary>
        /// Native SCardGetStatusChanged from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <param name="dwTimeout"></param>
        /// <param name="rgReaderStates"></param>
        /// <param name="cReaders"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardGetStatusChange(IntPtr hContext,
            UInt32 dwTimeout,
            [In, Out] SCard_ReaderState[] rgReaderStates,
            UInt32 cReaders);

        /// <summary>
        /// Native SCardListReaders function from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <param name="mszGroups"></param>
        /// <param name="mszReaders"></param>
        /// <param name="pcchReaders"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
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
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardEstablishContext(UInt32 dwScope,
            IntPtr pvReserved1,
            IntPtr pvReserved2,
            IntPtr phContext);

        /// <summary>
        /// Native SCardReleaseContext function from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardReleaseContext(IntPtr hContext);

        /// <summary>
        /// Native SCardIsValidContext function from winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
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
        [DllImport(WINSCARD_DLL, SetLastError = true, CharSet = CharSet.Auto)]
        internal static extern int SCardConnect(IntPtr hContext,
            [MarshalAs(UnmanagedType.LPTStr)] string szReader,
            UInt32 dwShareMode,
            UInt32 dwPreferredProtocols,
            IntPtr phCard,
            IntPtr pdwActiveProtocol);

        /// <summary>
        /// Native SCardDisconnect function from winscard.dll
        /// </summary>
        /// <param name="hCard"></param>
        /// <param name="dwDisposition"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardDisconnect(IntPtr hCard,
            UInt32 dwDisposition);

        /// <summary>
        /// The SCardControl function gives you direct control of the reader. You can call it any time after a successful call to SCardConnect and before a successful call to SCardDisconnect. The effect on the state of the reader depends on the control code.
        /// </summary>
        /// <param name="hCard">[in] Reference value returned from SCardConnect. </param>
        /// <param name="dwControlCode">[in] Control code for the operation. This value identifies the specific operation to be performed.</param>
        /// <param name="SendBuff">[in] Pointer to a buffer that contains the data required to perform the operation. This parameter can be NULL if the dwControlCode parameter specifies an operation that does not require input data. </param>
        /// <param name="SendBuffLen">[in] Size, in bytes, of the buffer pointed to by lpInBuffer. </param>
        /// <param name="RecvBuff">[out] Pointer to a buffer that receives the operation's output data. This parameter can be NULL if the dwControlCode parameter specifies an operation that does not produce output data. </param>
        /// <param name="RecvBuffLen">[in] Size, in bytes, of the buffer pointed to by lpOutBuffer. </param>
        /// <param name="pcbBytesReturned">[out] Pointer to a DWORD that receives the size, in bytes, of the data stored into the buffer pointed to by lpOutBuffer. </param>
        /// <returns>This function returns different values depending on whether it succeeds or fails.</returns>
        [DllImport(WINSCARD_DLL)]
        internal static extern int SCardControl(IntPtr hCard, 
            uint dwControlCode, 
            byte[] SendBuff, 
            int SendBuffLen, 
            [In, Out] byte[] RecvBuff, 
            int RecvBuffLen, 
            [In, Out] ref int pcbBytesReturned);

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
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardTransmit(IntPtr hCard,
            [In] ref SCard_IO_Request pioSendPci,
            byte[] pbSendBuffer,
            UInt32 cbSendLength,
            IntPtr pioRecvPci,
            [Out] byte[] pbRecvBuffer,
            out UInt32 pcbRecvLength);

        /// <summary>
        /// Native SCardBeginTransaction function of winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardBeginTransaction(IntPtr hContext);

        /// <summary>
        /// Native SCardEndTransaction function of winscard.dll
        /// </summary>
        /// <param name="hContext"></param>
        /// <returns></returns>
        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardEndTransaction(IntPtr hContext, UInt32 dwDisposition);

        [DllImport(WINSCARD_DLL, SetLastError = true)]
        internal static extern int SCardGetAttrib(IntPtr hCard,
            UInt32 dwAttribId,
            [Out] byte[] pbAttr,
            out UInt32 pcbAttrLen);
    }
}
