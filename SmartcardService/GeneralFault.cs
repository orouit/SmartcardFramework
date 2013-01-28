using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GemCard.Service.Fault
{
    [DataContract]
    public class GeneralFault : IFault<Exception> //: BaseFault<Exception>
    {
        [DataMember]
        public Exception SourceException { get; private set; }

        public GeneralFault(Exception ex)
            //: base(ex)
        {
            SourceException = ex;
        }
    }
}
