using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.WinRTSCard
{
    public sealed class APDUCommand
    {
        /// <summary>
        /// Get/set Class of the Coomqnd
        /// </summary>
        public byte Class
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the Instruction (Ins) of the command
        /// </summary>
        public byte Ins
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the Parameter P1 of the command
        /// </summary>
        public byte P1
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the Parameter P2 of the command
        /// </summary>
        public byte P2
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the Data of the command
        /// </summary>
        public byte[] Data
        {
            get;
            set;
        }

        /// <summary>
        /// Get/set the expected Length (Le) of the command
        /// </summary>
        public byte Le
        {
            get;
            set;
        }
    }
}
