using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;

namespace GemCard.Service.Host
{
    /// <summary>
    /// This class handles the lifecycle of a WCF service
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="T"></typeparam>
    class TCPServiceHostManager<I, T> : IDisposable
        where T : class
        where I : class
    {
        private ServiceHost serviceHost = null;
        private string baseAddress;
        private string serviceAddress;

        public EventHandler Opened;
        public EventHandler Closed;

        public TCPServiceHostManager(string baseAddress, string serviceAddress)
        {
            this.baseAddress = baseAddress;
            this.serviceAddress = serviceAddress;
        }

        private void AddMetadataEchange(ServiceHost host, string baseAddress, string serviceAddress)
        {
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);

            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                Path.Combine(baseAddress, serviceAddress, "mex"));
        }

        public bool Start()
        {
            bool started = false;

            if (serviceHost == null)
            {
                serviceHost = new ServiceHost(typeof(T), new Uri(baseAddress));
                NetTcpBinding serviceBinding = new NetTcpBinding(SecurityMode.None);
                serviceBinding.TransactionFlow = true;
                serviceHost.AddServiceEndpoint(typeof(I), serviceBinding, serviceAddress);

                // Attach event handlers
                serviceHost.Opened += serviceHost_Opened;
                serviceHost.Closed += serviceHost_Closed;

                AddMetadataEchange(serviceHost, baseAddress, serviceAddress);
            }

            if (serviceHost != null)
            {
                switch (serviceHost.State)
                {
                    case CommunicationState.Closed:
                        {
                            serviceHost = null;
                            started = Start();
                            break;
                        }

                    case CommunicationState.Opened:
                    case CommunicationState.Opening:
                        {
                            started = true;
                            break;
                        }

                    case CommunicationState.Created:
                        {
                            serviceHost.Open();
                            started = true;
                            break;
                        }
                }
            }

            return started;
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            if (Opened != null)
            {
                Opened(this, e);
            }
        }

        public void Stop()
        {
            if (serviceHost != null)
            {
                switch (serviceHost.State)
                {
                    case CommunicationState.Opened:
                        {
                            serviceHost.Close();
                            while (serviceHost.State != CommunicationState.Closed)
                            {
                                // Wait!
                            }
                            serviceHost.Opened -= serviceHost_Opened;
                            serviceHost.Closed -= serviceHost_Closed;
                            serviceHost = null;
                            break;
                        }
                }
            }
        }

        public CommunicationState State
        {
            get
            {
                CommunicationState state = CommunicationState.Closed;

                if (serviceHost != null)
                {
                    state = serviceHost.State;
                }

                return state;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }

    /// <summary>
    /// This class handles the lifecycle of a WCF service
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="T"></typeparam>
    class NamedPipeServiceHostManager<I, T> : IDisposable
        where T : class
        where I : class
    {
        private ServiceHost serviceHost = null;
        private string baseAddress;
        private string serviceAddress;

        public EventHandler Opened;
        public EventHandler Closed;

        public NamedPipeServiceHostManager(string baseAddress, string serviceAddress)
        {
            this.baseAddress = baseAddress;
            this.serviceAddress = serviceAddress;
        }

        private void AddMetadataEchange(ServiceHost host, string baseAddress, string serviceAddress)
        {
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);

            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexNamedPipeBinding(),
                Path.Combine(baseAddress, serviceAddress, "mex"));
        }

        public bool Start()
        {
            bool started = false;

            if (serviceHost == null)
            {
                serviceHost = new ServiceHost(typeof(T), new Uri(baseAddress));
                NetNamedPipeBinding serviceBinding = new NetNamedPipeBinding();
                serviceBinding.TransactionFlow = true;
                serviceHost.AddServiceEndpoint(typeof(I), serviceBinding, serviceAddress);

                // Attach event handlers
                serviceHost.Opened += serviceHost_Opened;
                serviceHost.Closed += serviceHost_Closed;

                AddMetadataEchange(serviceHost, baseAddress, serviceAddress);
            }

            if (serviceHost != null)
            {
                switch (serviceHost.State)
                {
                    case CommunicationState.Closed:
                        {
                            serviceHost = null;
                            started = Start();
                            break;
                        }

                    case CommunicationState.Opened:
                    case CommunicationState.Opening:
                        {
                            started = true;
                            break;
                        }

                    case CommunicationState.Created:
                        {
                            serviceHost.Open();
                            started = true;
                            break;
                        }
                }
            }

            return started;
        }

        void serviceHost_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        void serviceHost_Opened(object sender, EventArgs e)
        {
            if (Opened != null)
            {
                Opened(this, e);
            }
        }

        public void Stop()
        {
            if (serviceHost != null)
            {
                switch (serviceHost.State)
                {
                    case CommunicationState.Opened:
                        {
                            serviceHost.Close();
                            while (serviceHost.State != CommunicationState.Closed)
                            {
                                // Wait!
                            }
                            serviceHost.Opened -= serviceHost_Opened;
                            serviceHost.Closed -= serviceHost_Closed;
                            serviceHost = null;
                            break;
                        }
                }
            }
        }

        public CommunicationState State
        {
            get
            {
                CommunicationState state = CommunicationState.Closed;

                if (serviceHost != null)
                {
                    state = serviceHost.State;
                }

                return state;
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}
