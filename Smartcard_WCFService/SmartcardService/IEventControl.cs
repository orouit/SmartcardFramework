using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace GemCard.Service
{

    [ServiceContract(SessionMode = SessionMode.Required,
        CallbackContract = typeof(ICardEventCallback))]
    public interface IEventControl
    {
        [OperationContract(IsOneWay = true)]
        void SubscribeCardEvents();

        [OperationContract(IsOneWay = true)]
        void UnsubscribeCardEvent();    
    }
}
