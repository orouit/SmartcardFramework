using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.WinRTSCard
{
    public sealed class APDUResponse
    {
        /// <summary>
        /// Response data get property. Contains the data sent by the card minus the 2 status bytes (SW1, SW2)
        /// null if no data were sent by the card
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// SW1 byte get property
        /// </summary>
        public byte SW1
        {
            get;
            set;
        }

        /// <summary>
        /// SW2 byte get property
        /// </summary>
        public byte SW2
        {
            get;
            set;
        }
    }
}
