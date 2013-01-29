/**
 * @author Olivier ROUIT
 * 
 * @license CPL, CodeProject license 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;

namespace GemCard.Service.Host
{
    /// <summary>
    /// Base class for the service host managers
    /// </summary>
    /// <typeparam name="I"></typeparam>
    /// <typeparam name="T"></typeparam>
    abstract class ServiceHostManager<I, T> : IDisposable
        where T : class
        where I : class
    {
        #region Fields

        private ServiceHost serviceHost = null;
        private string baseAddress;
        private string serviceAddress;

        #endregion

        #region Events

        public EventHandler Opened;
        public EventHandler Closed;

        #endregion

        protected ServiceHostManager(string baseAddress, string serviceAddress)
        {
            this.baseAddress = baseAddress;
            this.serviceAddress = serviceAddress;
        }

        /// <summary>
        /// This method must be implemented to create the service binding
        /// </summary>
        /// <returns></returns>
        protected abstract Binding CreateServiceBinding();

        /// <summary>
        /// This method must be implemented to create the MEX binding
        /// </summary>
        /// <returns></returns>
        protected abstract Binding CreateMetadataExchangeBinding();

        /// <summary>
        /// starts the service
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            bool started = false;

            if (serviceHost == null)
            {
                serviceHost = new ServiceHost(typeof(T), new Uri(baseAddress));
                serviceHost.AddServiceEndpoint(typeof(I), CreateServiceBinding(), serviceAddress);

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

        /// <summary>
        /// Stops the service
        /// </summary>
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

                            // Detach event handlers
                            serviceHost.Opened -= serviceHost_Opened;
                            serviceHost.Closed -= serviceHost_Closed;
                            serviceHost = null;
                            break;
                        }
                }
            }
        }

        /// <summary>
        /// Gets the current service state
        /// </summary>
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

        #region Private methods

        private void AddMetadataEchange(ServiceHost host, string baseAddress, string serviceAddress)
        {
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);

            host.AddServiceEndpoint(typeof(IMetadataExchange),
                CreateMetadataExchangeBinding(),
                Path.Combine(baseAddress, serviceAddress, "mex"));
        }

        private void serviceHost_Closed(object sender, EventArgs e)
        {
            if (Closed != null)
            {
                Closed(this, e);
            }
        }

        private void serviceHost_Opened(object sender, EventArgs e)
        {
            if (Opened != null)
            {
                Opened(this, e);
            }
        }

        #endregion
    }
}
