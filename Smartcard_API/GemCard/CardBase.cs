using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace GemCard 
{
    /// <summary>
    /// Values for AttrId of SCardGetAttrib
    /// </summary>
    public class SCARD_ATTR_VALUE
    {
        private const uint 
            SCARD_CLASS_COMMUNICATIONS = 2,
            SCARD_CLASS_PROTOCOL = 3,
            SCARD_CLASS_MECHANICAL = 6,
            SCARD_CLASS_VENDOR_DEFINED = 7,
            SCARD_CLASS_IFD_PROTOCOL = 8,
            SCARD_CLASS_ICC_STATE = 9,
            SCARD_CLASS_SYSTEM = 0x7fff;

        private static UInt32 SCardAttrValue(UInt32 attrClass, UInt32 val)
        {
            return (attrClass << 16) | val;
        }

        public static UInt32 CHANNEL_ID { get { return SCardAttrValue(SCARD_CLASS_COMMUNICATIONS, 0x0110); } }

        public static UInt32 CHARACTERISTICS { get { return SCardAttrValue(SCARD_CLASS_MECHANICAL, 0x0150); } }

        public static UInt32 CURRENT_PROTOCOL_TYPE { get { return SCardAttrValue(SCARD_CLASS_IFD_PROTOCOL, 0x0201); } }

        public static UInt32 DEVICE_UNIT { get { return SCardAttrValue(SCARD_CLASS_SYSTEM, 0x0001); } }
        public static UInt32 DEVICE_FRIENDLY_NAME { get { return SCardAttrValue(SCARD_CLASS_SYSTEM, 0x0003); } }
        public UInt32 DEVICE_SYSTEM_NAME { get { return SCardAttrValue(SCARD_CLASS_SYSTEM, 0x0004); } }

        public static UInt32 ICC_PRESENCE { get { return SCardAttrValue(SCARD_CLASS_ICC_STATE, 0x0300); } }
        public static UInt32 ICC_INTERFACE_STATUS { get { return SCardAttrValue(SCARD_CLASS_ICC_STATE, 0x0301); } }
        public static UInt32 ATR_STRING { get { return SCardAttrValue(SCARD_CLASS_ICC_STATE, 0x0303); } }
        public static UInt32 ICC_TYPE_PER_ATR { get { return SCardAttrValue(SCARD_CLASS_ICC_STATE, 0x0304); } }

        public static UInt32 PROTOCOL_TYPES { get { return SCardAttrValue(SCARD_CLASS_PROTOCOL, 0x0120); } }

        public static UInt32 VENDOR_NAME { get { return SCardAttrValue(SCARD_CLASS_VENDOR_DEFINED, 0x0100); } }
        public static UInt32 VENDOR_IFD_TYPE { get { return SCardAttrValue(SCARD_CLASS_VENDOR_DEFINED, 0x0101); } }
        public static UInt32 VENDOR_IFD_VERSION { get { return SCardAttrValue(SCARD_CLASS_VENDOR_DEFINED, 0x0102); } }
        public static UInt32 VENDOR_IFD_SERIAL_NO { get { return SCardAttrValue(SCARD_CLASS_VENDOR_DEFINED, 0x0103); } }
    }

    /// <summary>
    /// Abstract class that adds a basic event management to the ICard interface. 
    /// </summary>
    abstract public class CardBase : ICard, IDisposable
    {
        protected const uint INFINITE = 0xFFFFFFFF;
        protected const uint WAIT_TIME = 250;

        protected bool m_bRunCardDetection = true;
        protected Thread m_thread = null;

        /// <summary>
        /// Event handler for the card insertion
        /// </summary>
        public event CardInsertedEventHandler OnCardInserted = null;

        /// <summary>
        /// Event handler for the card removal
        /// </summary>
        public event CardRemovedEventHandler OnCardRemoved = null;

        ~CardBase()
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

        /// <summary>
        /// This method should start a thread that checks for card insertion or removal
        /// </summary>
        /// <param name="Reader"></param>
        public void StartCardEvents(string Reader)
        {
            if (m_thread == null)
            {
                m_bRunCardDetection = true;

                m_thread = new Thread(new ParameterizedThreadStart(RunCardDetection));
                m_thread.Start(Reader);
            }
        }

        /// <summary>
        /// Stops the card events thread
        /// </summary>
        public void StopCardEvents()
        {
            if (m_thread != null)
            {
                int
                    nTimeOut = 10,
                    nCount = 0;
                bool m_bStop = false;
                m_bRunCardDetection = false;

                do
                {
                    if (nCount > nTimeOut)
                    {
                        m_thread.Abort();
                        break;
                    }

                    if (m_thread.ThreadState == ThreadState.Aborted)
                        m_bStop = true;

                    if (m_thread.ThreadState == ThreadState.Stopped)
                        m_bStop = true;

                    Thread.Sleep(200);
                    ++nCount;           // Manage time out
                }
                while (!m_bStop);

                m_thread = null;
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
        abstract protected void RunCardDetection(object Reader);

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

        public void Dispose()
        {
            StopCardEvents();
        }
    }
}