using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace GemCard.Service.Host
{
    /// <summary>
    /// This class handles the lifecycle of a WCF service with NamedPipe binding
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="T"></typeparam>
    class NamedPipeServiceHostManager<I, T> : ServiceHostManager<I, T>
        where T : class
        where I : class
    {
        public NamedPipeServiceHostManager(string baseAddress, string serviceAddress)
            : base(baseAddress, serviceAddress)
        {
        }

        protected override Binding CreateServiceBinding()
        {
            NetNamedPipeBinding serviceBinding = new NetNamedPipeBinding();
            serviceBinding.TransactionFlow = true;

            return serviceBinding;
        }

        protected override Binding CreateMetadataExchangeBinding()
        {
            return MetadataExchangeBindings.CreateMexNamedPipeBinding();
        }
    }
}
