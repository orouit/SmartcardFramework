/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GemCard.Service.Fault
{
    [DataContract]
    public class SmartcardFault : GeneralFault
    {
        public SmartcardFault(SmartCardException ex)
            : base(ex)
        {
        }
    }
}
