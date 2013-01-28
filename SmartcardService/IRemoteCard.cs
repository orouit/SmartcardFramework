using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GemCard;
using GemCard.Service.Fault;

namespace GemCard.Service
{
    /// <summary>
    /// Contract to manage a smart card connected to a smart card reader
    /// </summary>
    [ServiceContract]
    public interface IRemoteCard
    {
        /// <summary>
        /// Gets the list of readers
        /// </summary>
        /// <returns>A string array of the readers</returns>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        string[] ListReaders();

        /// <summary>
        /// Connects to a card. Establishes a card session
        /// </summary>
        /// <param name="Reader">Reader string</param>
        /// <param name="ShareMode">Session share mode</param>
        /// <param name="PreferredProtocols">Session preferred protocol</param>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]        
        void Connect(string Reader, SHARE ShareMode, PROTOCOL PreferredProtocols);

        /// <summary>
        /// Disconnect the current session
        /// </summary>
        /// <param name="Disposition">Action when disconnecting from the card</param>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        void Disconnect(DISCONNECT Disposition);

        /// <summary>
        /// Transmit an APDU command to the card
        /// </summary>
        /// <param name="ApduCmd">APDU Command to send to the card</param>
        /// <returns>An APDU Response from the card</returns>
        [OperationContract]
        //[FaultContract(typeof(ApduCommandFault))]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        APDUResponse Transmit(APDUCommand ApduCmd);

        /// <summary>
        /// Begins a card transaction
        /// </summary>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        void BeginTransaction();

        /// <summary>
        /// Ends a card transaction
        /// </summary>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        void EndTransaction(DISCONNECT Disposition);

        /// <summary>
        /// Gets the attributes of the card
        /// 
        /// This command can be used to get the Answer to reset
        /// </summary>
        /// <param name="AttribId">Identifier for the Attribute to get</param>
        /// <returns>Attribute content</returns>
        [OperationContract]
        //[FaultContract(typeof(SmartcardFault))]
        //[FaultContract(typeof(GeneralFault))]
        byte[] GetAttribute(UInt32 AttribId);
    }
}
