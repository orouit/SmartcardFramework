using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GemCard.Service.Fault
{
    public interface IFault<T>
        where T : Exception
    {
        T SourceException { get; }
    }

    [DataContract]
    public class ApduCommandFault : IFault<ApduCommandException> //: BaseFault<ApduCommandException>
    {
        [DataMember]
        public ApduCommandException SourceException { get; private set; }

        public ApduCommandFault(ApduCommandException ex)
            //: base(ex)
        {
            SourceException = ex;
        }
    }
}
