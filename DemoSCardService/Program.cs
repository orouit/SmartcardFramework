using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestSCardService
{
    class Program
    {
        const byte 
            SC_PENDING = 0x9F,
            SC_OK_HI = 0x90;

        static void Main(string[] args)
        {
#if NET_TCP
            DemoWithTCPService();
#else
            DemoWithNAMEDPIPEService();
#endif
        }

        static void DemoWithNAMEDPIPEService()
        {
            try
            {
                SCardNPService.IRemoteCard remoteCard = new SCardNPService.RemoteCardClient();

                string[] readers = remoteCard.ListReaders();
                Console.WriteLine("Readers:");

                foreach (string reader in readers)
                {
                    Console.WriteLine("    " + reader);
                }

                if (readers.Length > 0)
                {
                    remoteCard.Connect(readers[0], SCardNPService.SHARE.Shared, SCardNPService.PROTOCOL.T0orT1);
                    Console.WriteLine("Session opened with the remote card on reader " + readers[0]);

                    SCardNPService.APDUCommand
                        apduSelectFile = new SCardNPService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xA4,
                            P1 = 0,
                            P2 = 0,
                            Data = null,
                            Le = 0
                        },
                        apduReadRecord = new SCardNPService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xB2,
                            P1 = 1,
                            P2 = 4,
                            Data = null,
                            Le = 0
                        },
                        apduGetResponse = new SCardNPService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xC0,
                            P1 = 0,
                            P2 = 0,
                            Data = null,
                            Le = 0
                        };

                    // Select MF
                    apduSelectFile.Data = new byte[] { 0x3F, 0x00 };
                    SCardNPService.APDUResponse response = remoteCard.Transmit(apduSelectFile);
                    if (response.SW1 == SC_PENDING)
                    {
                        // Select EFtelecom
                        apduSelectFile.Data = new byte[] { 0x7F, 0x10 };
                        response = remoteCard.Transmit(apduSelectFile);
                        if (response.SW1 == SC_PENDING)
                        {
                            // Select EFadn
                            apduSelectFile.Data = new byte[] { 0x6F, 0x3A };
                            response = remoteCard.Transmit(apduSelectFile);
                            if (response.SW1 == SC_PENDING)
                            {
                                apduGetResponse.Le = response.SW2;
                                response = remoteCard.Transmit(apduGetResponse);
                                if (response.SW1 == SC_OK_HI)
                                {
                                    // Get the length of the record
                                    int recordLength = response.Data[14];

                                    Console.WriteLine("Reading the Phone number 10 first entries");
                                    // Read the 10 first record of the file
                                    for (int nI = 0; nI < 10; nI++)
                                    {
                                        apduReadRecord.Le = (byte)recordLength;
                                        apduReadRecord.P1 = (byte)(nI + 1);
                                        response = remoteCard.Transmit(apduReadRecord);

                                        if (response.SW1 == SC_OK_HI)
                                        {
                                            Console.WriteLine("Record #" + (nI + 1).ToString());
                                            Console.WriteLine(BufferToString(response.Data));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("Press a key to close the session...");

                    Console.ReadKey();

                    remoteCard.Disconnect(SCardNPService.DISCONNECT.Unpower);
                    Console.WriteLine("Session closed with the remote card");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static void DemoWithTCPService()
        {
            try
            {
                SCardService.IRemoteCard remoteCard = new SCardService.RemoteCardClient();

                string[] readers = remoteCard.ListReaders();
                Console.WriteLine("Readers:");

                foreach (string reader in readers)
                {
                    Console.WriteLine("    " + reader);
                }

                if (readers.Length > 0)
                {
                    remoteCard.Connect(readers[0], SCardService.SHARE.Shared, SCardService.PROTOCOL.T0orT1);
                    Console.WriteLine("Session opened with the remote card on reader " + readers[0]);

                    SCardService.APDUCommand
                        apduSelectFile = new SCardService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xA4,
                            P1 = 0,
                            P2 = 0,
                            Data = null,
                            Le = 0
                        },
                        apduReadRecord = new SCardService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xB2,
                            P1 = 1,
                            P2 = 4,
                            Data = null,
                            Le = 0
                        },
                        apduGetResponse = new SCardService.APDUCommand()
                        {
                            Class = 0xA0,
                            Ins = 0xC0,
                            P1 = 0,
                            P2 = 0,
                            Data = null,
                            Le = 0
                        };

                    // Select MF
                    apduSelectFile.Data = new byte[] { 0x3F, 0x00 };
                    SCardService.APDUResponse response = remoteCard.Transmit(apduSelectFile);
                    if (response.SW1 == SC_PENDING)
                    {
                        // Select EFtelecom
                        apduSelectFile.Data = new byte[] { 0x7F, 0x10 };
                        response = remoteCard.Transmit(apduSelectFile);
                        if (response.SW1 == SC_PENDING)
                        {
                            // Select EFadn
                            apduSelectFile.Data = new byte[] { 0x6F, 0x3A };
                            response = remoteCard.Transmit(apduSelectFile);
                            if (response.SW1 == SC_PENDING)
                            {
                                apduGetResponse.Le = response.SW2;
                                response = remoteCard.Transmit(apduGetResponse);
                                if (response.SW1 == SC_OK_HI)
                                {
                                    // Get the length of the record
                                    int recordLength = response.Data[14];

                                    Console.WriteLine("Reading the Phone number 10 first entries");
                                    // Read the 10 first record of the file
                                    for (int nI = 0; nI < 10; nI++)
                                    {
                                        apduReadRecord.Le = (byte)recordLength;
                                        apduReadRecord.P1 = (byte)(nI + 1);
                                        response = remoteCard.Transmit(apduReadRecord);

                                        if (response.SW1 == SC_OK_HI)
                                        {
                                            Console.WriteLine("Record #" + (nI + 1).ToString());
                                            Console.WriteLine(BufferToString(response.Data));
                                        }
                                    }
                                }
                            }
                        }
                    }

                    Console.WriteLine("Press a key to close the session...");

                    Console.ReadKey();

                    remoteCard.Disconnect(SCardService.DISCONNECT.Unpower);
                    Console.WriteLine("Session closed with the remote card");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static string BufferToString(byte[] data)
        {
            StringBuilder text = new StringBuilder();
            foreach (byte bData in data)
            {
                text.AppendFormat("{0:X02}", bData);
            }

            return text.ToString();
        }
    }
}
