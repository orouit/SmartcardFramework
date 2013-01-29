/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

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
            try
            {
                return card.ListReaders();
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public void Connect(string reader, SHARE shareMode, PROTOCOL preferredProtocols)
        {
            try
            {
                card.Connect(reader, shareMode, preferredProtocols);
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public void Disconnect(DISCONNECT disposition)
        {
            try
            {
                card.Disconnect(disposition);
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public APDUResponse Transmit(APDUCommand apduCmd)
        {
            try
            {
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
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public void BeginTransaction()
        {
            try
            {
                card.BeginTransaction();
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public void EndTransaction(DISCONNECT disposition)
        {
            try
            {
                card.EndTransaction(disposition);
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        public byte[] GetAttribute(uint attribId)
        {
            try
            {
                return card.GetAttribute(attribId); ;
            }
            catch (SmartCardException scEx)
            {
                SmartcardFault scFault = new SmartcardFault(scEx);
                throw new FaultException<SmartcardFault>(scFault);
            }
            catch (Exception ex)
            {
                GeneralFault genFault = new GeneralFault(ex);
                throw new FaultException<GeneralFault>(genFault);
            }
        }

        #endregion
    }
}
