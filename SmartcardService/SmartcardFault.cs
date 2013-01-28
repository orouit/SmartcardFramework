using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GemCard.Service.Fault
{
    [DataContract]
    public class SmartcardFault : IFault<SmartCardException> // : BaseFault<SmartCardException>
    {
        [DataMember]
        public SmartCardException SourceException { get; private set; }

        public SmartcardFault(SmartCardException ex)
            //: base(ex)
        {
            SourceException = ex;
        }
    }
}
