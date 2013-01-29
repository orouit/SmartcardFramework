using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GemCard.Service
{
    /// <summary>
    /// This class represents an APDU Response from a card command.
    /// 
    /// It is a DataContract to be serializd by a WCF service request
    /// </summary>
    [DataContract]
    public class APDUResponse
    {
        /// <summary>
        /// Response data get property. Contains the data sent by the card minus the 2 status bytes (SW1, SW2)
        /// null if no data were sent by the card
        /// </summary>
        [DataMember]
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// SW1 byte get property
        /// </summary>
        [DataMember]
        public byte SW1
        {
            get;
            set;
        }

        /// <summary>
        /// SW2 byte get property
        /// </summary>
        [DataMember]
        public byte SW2
        {
            get;
            set;
        }
    }
}
