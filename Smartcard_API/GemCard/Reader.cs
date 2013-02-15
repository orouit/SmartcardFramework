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
    public class Reader
    {
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
        private static IntPtr EstablishContext(SCOPE Scope)
        {
            int lastError = PCSC.SCARD_S_SUCCESS;
            //IntPtr hContextRet = IntPtr.Zero;
            IntPtr hContext = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(IntPtr)));

            lastError = PCSC.SCardEstablishContext((uint)Scope, IntPtr.Zero, IntPtr.Zero, hContext);
            if (lastError != 0)
            {
                Marshal.FreeHGlobal(hContext);
                ThrowSmartcardException("SCardEstablishContext", lastError);
            }

            //hContextRet = Marshal.ReadIntPtr(hContext);

            //Marshal.FreeHGlobal(hContext);

            return hContext;
        }

        /// <summary>
        /// Wraps the PCSC function
        /// LONG SCardReleaseContext(
        ///		IN SCARDCONTEXT hContext
        ///	);
        /// </summary>
        private static void ReleaseContext(IntPtr hContext)
        {
            if (PCSC.SCardIsValidContext(hContext) == PCSC.SCARD_S_SUCCESS)
            {
                int lastError = PCSC.SCardReleaseContext(hContext);
                ThrowSmartcardException("SCardReleaseContext", lastError);

                Marshal.FreeHGlobal(hContext);
                hContext = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Wraps the PCSC function
        /// LONG SCardListReaders(SCARDCONTEXT hContext, 
        ///		LPCTSTR mszGroups, 
        ///		LPTSTR mszReaders, 
        ///		LPDWORD pcchReaders 
        ///	);
        /// </summary>
        /// <returns>A string array of the readers</returns>
        public static string[] ListReaders()
        {
            IntPtr hContext = EstablishContext(SCOPE.User);

            string[] sListReaders = null;
            UInt32 pchReaders = 0;
            IntPtr szListReaders = IntPtr.Zero;

            int lastError = PCSC.SCardListReaders(hContext, null, szListReaders, out pchReaders);
            if (lastError == 0)
            {
                szListReaders = Marshal.AllocHGlobal((int)pchReaders);
                lastError = PCSC.SCardListReaders(hContext, null, szListReaders, out pchReaders);
                if (lastError == 0)
                {
                    char[] caReadersData = new char[pchReaders];
                    int nbReaders = 0;
                    for (int nI = 0; nI < pchReaders; nI++)
                    {
                        caReadersData[nI] = (char)Marshal.ReadByte(szListReaders, nI);

                        if (caReadersData[nI] == 0)
                            nbReaders++;
                    }

                    // Remove last 0
                    --nbReaders;

                    if (nbReaders != 0)
                    {
                        sListReaders = new string[nbReaders];
                        char[] caReader = new char[pchReaders];
                        int nIdx = 0;
                        int nIdy = 0;
                        int nIdz = 0;
                        // Get the nJ string from the multi-string

                        while (nIdx < pchReaders - 1)
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

            ReleaseContext(hContext);

            ThrowSmartcardException("SCardListReaders", lastError);

            return sListReaders;
        }

        private static void ThrowSmartcardException(string methodName, long errCode)
        {
            if (errCode != 0)
            {
                throw new SmartCardException(string.Format("{0} error: {1:X02}", methodName, errCode));
            }
        }
    }
}
