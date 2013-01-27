using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

using GemCard;

namespace GemCard.Service
{
    /// <summary>
    /// Implements the IRemoteCard interface as WCF service
    /// 
    /// This class uses the CardNative object that implements the ICard interface
    /// </summary>
    public class RemoteCard : IRemoteCard
    {
        private CardNative card = new CardNative();

        #region ICard interface

        public string[] ListReaders()
        {
            return card.ListReaders();
        }

        public void Connect(string reader, SHARE shareMode, PROTOCOL preferredProtocols)
        {
            card.Connect(reader, shareMode, preferredProtocols);
        }

        public void Disconnect(DISCONNECT disposition)
        {
            card.Disconnect(disposition);
        }

        public APDUResponse Transmit(APDUCommand apduCmd)
        {
            if (apduCmd.Data == null || apduCmd.Data.Length == 0)
            {

            }

            GemCard.APDUCommand apduCommand = new GemCard.APDUCommand(
                apduCmd.Class,
                apduCmd.Ins,
                apduCmd.P1,
                apduCmd.P2,
                (apduCmd.Data == null || apduCmd.Data.Length == 0) ? null : apduCmd.Data,
                apduCmd.Le);
 
            GemCard.APDUResponse apduResponse = card.Transmit(apduCommand);

            return new APDUResponse()
            {
                Data = apduResponse.Data,
                SW1 = apduResponse.SW1,
                SW2 = apduResponse.SW2
            };
        }

        public void BeginTransaction()
        {
            card.BeginTransaction(); 
        }

        public void EndTransaction(DISCONNECT disposition)
        {
            card.EndTransaction(disposition);
        }

        public byte[] GetAttribute(uint attribId)
        {
            return card.GetAttribute(attribId); ;
        }

        #endregion
    }
}
