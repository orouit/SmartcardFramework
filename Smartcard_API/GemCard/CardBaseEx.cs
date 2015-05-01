/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Core.Smartcard
{
    /// <summary>
    /// Abstract class that adds a basic event management to the ICard interface. 
    /// </summary>
    abstract public class CardBaseEx : ICard, IDisposable
    {
        protected const uint INFINITE = 0xFFFFFFFF;
        protected const uint WAIT_TIME = 250;

        protected bool m_bRunCardDetection = true;
        protected Task eventTask = null;
        protected CancellationToken cancellationToken;
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Event handler for the card insertion
        /// </summary>
        public event CardInsertedEventHandler OnCardInserted = null;

        /// <summary>
        /// Event handler for the card removal
        /// </summary>
        public event CardRemovedEventHandler OnCardRemoved = null;

        ~CardBaseEx()
        {
            // Stop any eventual card detection thread
            StopCardEvents();
        }

        #region Abstract method that implement the ICard interface

        abstract public string[] ListReaders();
        abstract public void Connect(string Reader, SHARE ShareMode, PROTOCOL PreferredProtocols);
        abstract public void Disconnect(DISCONNECT Disposition);
        abstract public APDUResponse Transmit(APDUCommand ApduCmd);
        abstract public void BeginTransaction();
        abstract public void EndTransaction(DISCONNECT Disposition);
        abstract public byte[] GetAttribute(UInt32 AttribId);

        #endregion

        public void Dispose()
        {
            Disposing();
        }

        /// <summary>
        /// This method should start a thread that checks for card insertion or removal
        /// </summary>
        /// <param name="Reader"></param>
        /// <returns>true if the events have been started, false if they are already running</returns>
        public bool StartCardEvents(string reader)
        {
            bool ret = false;
            if (eventTask == null)
            {
                cancellationToken = cancellationTokenSource.Token;
                eventTask = Task.Factory.StartNew(() => RunCardDetection(reader), cancellationToken);
                ret = true;
            }

            return ret;
        }

        /// <summary>
        /// Stops the card events thread
        /// </summary>
        public void StopCardEvents()
        {
            if (eventTask != null)
            {
                bool stopped = false;

                if (eventTask.Status == TaskStatus.Running)
                {
                    cancellationTokenSource.Cancel();

                    do
                    {
                        if (eventTask.Status == TaskStatus.Canceled)
                        {
                            stopped = true;
                        }

                        if (eventTask.Status == TaskStatus.RanToCompletion)
                        {
                            stopped = true;
                        }

                        Thread.SpinWait(50000);
                    }
                    while (!stopped);

                    eventTask.Dispose();
                    eventTask = null;
                }
            }
        }

        /// <summary>
        /// This function must implement a card detection mechanism.
        /// 
        /// When card insertion is detected, it must call the method CardInserted()
        /// When card removal is detected, it must call the method CardRemoved()
        /// 
        /// </summary>
        /// <param name="Reader">Name of the reader to scan for card event</param>
        abstract protected void RunCardDetection(string Reader);

        #region Event methods

        protected void CardInserted(string reader)
        {
            if (OnCardInserted != null)
                OnCardInserted(this, reader);
        }

        protected void CardRemoved(string reader)
        {
            if (OnCardRemoved != null)
                OnCardRemoved(this, reader);
        }

        #endregion

        protected virtual void Disposing()
        {
            StopCardEvents();
        }
    }
}
