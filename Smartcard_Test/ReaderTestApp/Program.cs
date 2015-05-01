/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using Core.Smartcard;
using System;

namespace ReaderTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                string[] readerList = Reader.ListReaders();
                for (int nI = 0; nI < readerList.Length; nI++)
                {
                    Console.WriteLine(string.Format("Reader[{0}] = {1}", nI, readerList[nI]));  
                }

                if (readerList.Length > 0)
                {
                    Reader reader = new Reader(readerList[0]);
                    reader.CardInserted += reader_CardInserted;
                    reader.CardRemoved += reader_CardRemoved;
                    Console.WriteLine("Reader: " + readerList[0] + " connected");
                    reader.StartCardEvents();

                    Console.WriteLine("Press any key to stop the application...");
                    Console.ReadKey(true);

                    reader.StopCardEvents();
                    reader.Dispose();
                    Console.WriteLine("Card detection terminated");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception " + ex.Message);
            }
        }

        static void reader_CardRemoved(object sender, CardRemovedArgs args)
        {
            Console.WriteLine("A card has been removed from reader [" + args.Reader + "]");
        }

        static void reader_CardInserted(object sender, CardInsertedArgs args)
        {
            Console.WriteLine("A card has been inserted in reader [" + args.Reader + "]");
        }
    }
}
