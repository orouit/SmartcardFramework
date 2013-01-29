using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace GemCard.Service.Fault
{
    [DataContract]
    public class GeneralFault 
    {
        [DataMember]
        public string Message { get; private set; }

        public GeneralFault(Exception ex)
        {
            Message = ex.Message;
        }
    }
}
