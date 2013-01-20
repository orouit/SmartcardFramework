using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GemCard;

namespace GemCard.Service
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IRemoteCard
    {
        /// <summary>
        /// Gets the list of readers
        /// </summary>
        /// <returns>A string array of the readers</returns>
        [OperationContract]
        string[] ListReaders();

        /// <summary>
        /// Connects to a card. Establishes a card session
        ///	
        /// </summary>
        /// <param name="Reader"></param>
        /// <param name="ShareMode"></param>
        /// <param name="PreferredProtocols"></param>
        [OperationContract]
        void Connect(string Reader, SHARE ShareMode, PROTOCOL PreferredProtocols);

        /// <summary>
        /// Disconnect the current session
        /// </summary>
        /// <param name="Disposition"></param>
        [OperationContract]
        void Disconnect(DISCONNECT Disposition);

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
        [OperationContract]
        APDUResponse Transmit(APDUCommand ApduCmd);

        /// <summary>
        /// Wraps the PSCS function
        /// LONG SCardBeginTransaction(
        ///     SCARDHANDLE hCard
        //  );
        /// </summary>
        [OperationContract]
        void BeginTransaction();

        /// <summary>
        /// Wraps the PCSC function
        /// LONG SCardEndTransaction(
        ///     SCARDHANDLE hCard,
        ///     DWORD dwDisposition
        /// );
        /// </summary>
        [OperationContract]
        void EndTransaction(DISCONNECT Disposition);

        /// <summary>
        /// Gets the attributes of the card
        /// </summary>
        /// <param name="AttribId">Identifier for the Attribute to get</param>
        /// <returns>Attribute content</returns>
        [OperationContract]
        byte[] GetAttribute(UInt32 AttribId);
    }

    // Use a data contract as illustrated in the sample below to add composite types to service operations.
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
}
