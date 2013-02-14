using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace GemCard.Service
{
    public interface ICardEventCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnCardInserted(string reader);

        [OperationContract(IsOneWay = true)]
        void OnCardRemoved(string reader);
    }
}
