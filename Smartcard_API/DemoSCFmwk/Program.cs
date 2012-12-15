using System;
using System.Collections.Generic;
using System.Text;

using GemCard;

namespace DemoSCFmwk
{
    class Program
    {
        static APDUCommand 
            apduVerifyCHV = new APDUCommand(0xA0, 0x20, 0, 1, null, 0),
            apduSelectFile = new APDUCommand(0xA0, 0xA4, 0, 0, null, 0),
            apduReadRecord = new APDUCommand(0xA0, 0xB2, 1, 4, null, 0),
            apduGetResponse = new APDUCommand(0xA0, 0xC0, 0, 0, null, 0);

        const ushort SC_OK = 0x9000;
        const byte SC_PENDING = 0x9F;

        /// <summary>
        /// This program tests the API with a SIM card. 
        /// If your PIN is activated be carefull when presenting the PIN to your card! 
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                APDUResponse apduResp;

                CardNative iCard = new CardNative();

                string[] readers = iCard.ListReaders();

                iCard.Connect(readers[0], SHARE.Shared, PROTOCOL.T0orT1);
                Console.WriteLine("Connects card on reader: " + readers[0]);

                // Verify the PIN (if necessary)
                byte[] pin = new byte[] { 0x31, 0x32, 0x33, 0x34, 0xFF, 0xFF, 0xFF, 0xFF};
                APDUParam apduParam = new APDUParam();
                apduParam.Data = pin;
                apduVerifyCHV.Update(apduParam);
                //apduResp = iCard.Transmit(apduVerifyCHV);

                // Select the MF (3F00)
                apduParam.Data = new byte[] { 0x3F, 0x00 };
                apduSelectFile.Update(apduParam);
                apduResp = iCard.Transmit(apduSelectFile);
                if (apduResp.Status != SC_OK && apduResp.SW1 != SC_PENDING)
                    throw new Exception("Select command failed: " + apduResp.ToString());
                Console.WriteLine("MF selected");

                // Select the EFtelecom (7F10)
                apduParam.Data = new byte[] { 0x7F, 0x10 };
                apduSelectFile.Update(apduParam);
                apduResp = iCard.Transmit(apduSelectFile);
                if (apduResp.Status != SC_OK && apduResp.SW1 != SC_PENDING)
                    throw new Exception("Select command failed: " + apduResp.ToString());
                Console.WriteLine("DFtelecom selected");

                // Select the EFadn (6F3A)
                apduParam.Data = new byte[] { 0x6F, 0x3A };
                apduSelectFile.Update(apduParam);
                apduResp = iCard.Transmit(apduSelectFile);
                if (apduResp.Status != SC_OK && apduResp.SW1 != SC_PENDING)
                    throw new Exception("Select command failed: " + apduResp.ToString());
                Console.WriteLine("EFadn (Phone numbers) selected");

                // Read the response
                if (apduResp.SW1 == SC_PENDING)
                {
                    apduParam.Reset();
                    apduParam.Le = apduResp.SW2;
                    apduParam.Data = null;
                    apduGetResponse.Update(apduParam);
                    apduResp = iCard.Transmit(apduGetResponse);
                    if (apduResp.Status != SC_OK)
                        throw new Exception("Select command failed: " + apduResp.ToString());
                }

                // Get the length of the record
                int recordLength = apduResp.Data[14];

                Console.WriteLine("Reading the Phone number 10 first entries");
                // Read the 10 first record of the file
                for (int nI = 0; nI < 10; nI++)
                {
                    apduParam.Reset();
                    apduParam.Le = (byte) recordLength;
                    apduParam.P1 = (byte) (nI + 1);
                    apduReadRecord.Update(apduParam);
                    apduResp = iCard.Transmit(apduReadRecord);

                    if (apduResp.Status != SC_OK)
                        throw new Exception("ReadRecord command failed: " + apduResp.ToString());

                    Console.WriteLine("Record #" + ((int) (nI + 1)).ToString());
                    Console.WriteLine(apduResp.ToString());
                }

                iCard.Disconnect(DISCONNECT.Unpower);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
