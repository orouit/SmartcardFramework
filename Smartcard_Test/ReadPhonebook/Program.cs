/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

using Core.Smartcard;
using GSMHelper;
using SmartCardPlayer;

namespace TestSmartcard
{
    class Program
    {
        const string FileName = "CmdList.xml";
        const string ApduFile = "ApduList.xml";
        const string SequenceFile = "SequenceList.xml";

        /// <summary>
        /// Formats the PIN and pad with FF
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        static string FormatPIN(string pin)
        {
            StringBuilder formPin = new StringBuilder();

            for (int nI = 0; nI < 8; nI++)
            {
                if (nI < pin.Length)
                {
                    formPin.AppendFormat("3{0}", pin[nI]);
                }
                else
                    formPin.Append("FF");
            }

            return formPin.ToString();
        }


        static void Main(string[] args)
        {
            try
            {
                int nbRecords = 10;
                string PIN = "31323334FFFFFFFF";
                bool bPin = false;

                if (args.Length != 0)
                {
                    for (int nI = 0; nI < args.Length; nI++)
                    {
                        if (args[nI] == "P")
                        {
                            bPin = true;
                            PIN = FormatPIN(args[++nI]);
                        }
                        else
                            nbRecords = int.Parse(args[nI]);
                    }
                }

                APDUResponse apduResp = null;
                CardNative iCard = new CardNative();

                string[] readers = iCard.ListReaders();

                iCard.Connect(readers[0], SHARE.Shared, PROTOCOL.T0orT1);

                APDUPlayer player = new APDUPlayer(iCard);
                player.LoadAPDUFile(Properties.Settings.Default.DataPath + ApduFile);
                player.LoadSequenceFile(Properties.Settings.Default.DataPath + SequenceFile);

                SequenceParameter seqParam = new SequenceParameter();

                // Process Apdu: VerifyCHV
                if (bPin)
                {
                    Console.WriteLine("Sequence: Verify CHV1");
                    seqParam.Add("PIN", PIN);
                    apduResp = player.ProcessSequence("Verify CHV1", seqParam);
                    Console.WriteLine(apduResp.ToString());
                }
                 
                if (!bPin || (bPin && apduResp.Status == 0x9000))
                {
                    for (int nI = 1; nI <= nbRecords; nI++)
                    {
                        seqParam.Clear();
                        seqParam.Add("Record", nI.ToString());
                        //Console.WriteLine(string.Format("Read ADN, Record={0}", nI));
                        apduResp = player.ProcessSequence("Read ADN", seqParam);

                        PhoneNumber phone = new PhoneNumber(apduResp.Data);
                        Console.WriteLine("ADN n°" + nI.ToString());
                        Console.WriteLine(phone.ToString());
                        Console.WriteLine();

                        //Console.WriteLine(apduResp.ToString());
                    }
                }

                APDULogList log = player.Log;

                Console.WriteLine("Log:");
                APDULog[] arrayLog = log.ToArray();
                for (int nI = 0; nI < log.Count; nI++)
                    Console.WriteLine(arrayLog[nI].ToString());

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}