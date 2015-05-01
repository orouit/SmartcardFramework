/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Smartcard
{
    /// <summary>
    /// This class is used to update a set of parameters of an APDUCommand object
    /// </summary>
    public class APDUParam
    {
        byte
            bClass = 0,
            bChannel = 0,
            bP2 = 0,
            bP1 = 0;
        byte[] baData = null;
        short nLe = -1;
        bool 
            useP1 = false,
            useP2 = false,
            useChannel = false,
            useData = false,
            useClass = false,
            useLe = false;

        #region Constructors

        public APDUParam()
        {
        }

        /// <summary>
        /// Copy constructor (used for cloning)
        /// </summary>
        /// <param name="other"></param>
        public APDUParam(APDUParam other)
        {
            // Copy field
            if (other.baData != null)
            {
                baData = new byte[other.baData.Length];
                other.baData.CopyTo(baData, 0);
            }
            bClass = other.bClass;
            bChannel = other.bChannel;
            bP1 = other.bP1;
            bP2 = other.bP2;
            nLe = other.nLe;

            // Copy flags field
            useChannel = other.useChannel;
            useClass = other.useClass;
            useData = other.useData;
            useLe = other.useLe;
            useP1 = other.useP1;
            useP2 = other.useP2;
        }

        public APDUParam(byte bClass, byte bP1, byte bP2, byte[] baData, short nLe)
        {
            this.Class = bClass;
            this.P1 = bP1;
            this.P2 = bP2;
            this.Data = baData;
            this.Le = (byte)nLe;
        }

        #endregion

        /// <summary>
        /// Clones the current APDUParam instance
        /// </summary>
        /// <returns></returns>
        public APDUParam Clone()
        {
            return new APDUParam(this);
        }

        /// <summary>
        /// Resets the current instance, all flags are set to false
        /// </summary>
        public void Reset()
        {
            bClass = 0;
            bChannel = 0;
            bP2 = 0;
            bP1 = 0;

            baData = null;
            nLe = -1;

            useP1 = false;
            useP2 = false;
            useChannel = false;
            useData = false;
            useClass = false;
            useLe = false;
        }

        #region Flags properties

        public bool UseClass
        {
            get { return useClass; }
        }

        public bool UseChannel
        {
            get { return useChannel; }
        }

        public bool UseLe
        {
            get { return useLe; }
        }

        public bool UseData
        {
            get { return useData; }
        }

        public bool UseP1
        {
            get { return useP1; }
        }

        public bool UseP2
        {
            get { return useP2; }
        }

        #endregion

        #region Parameter properties

        public byte P1
        {
            get { return bP1; }

            set
            {
                bP1 = value;
                useP1 = true;
            }
        }

        public byte P2
        {
            get { return bP2; }
            set
            {
                bP2 = value;
                useP2 = true;
            }

        }

        public byte[] Data
        {
            get { return baData; }
            set
            {
                baData = value;
                useData = true;
            }
        }

        public byte Le
        {
            get { return (byte)nLe; }
            set
            {
                nLe = value;
                useLe = true;
            }
        }

        public byte Channel
        {
            get { return bChannel; }
            set
            {
                bChannel = value;
                useChannel = true;
            }
        }

        public byte Class
        {
            get { return bClass; }
            set
            {
                bClass = value;
                useClass = true;
            }
        }

        #endregion
    }
}
