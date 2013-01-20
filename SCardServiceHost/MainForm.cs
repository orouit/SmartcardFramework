using GemCard.Service;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GemCard.Service.Host
{
    public partial class MainForm : Form
    {
        const string 
            NET_TCP_BASE_ADDRESS = "net.tcp://localhost:8001/",
            SERVCICE_ADDRESS = "SCardService";

        private ServiceHost SCardServiceHost = null;

        public MainForm()
        {
            InitializeComponent();

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            StopService();
            base.OnClosed(e);
        }

        #region WCF service control
        private bool StartService()
        {
            bool started = false;

            try
            {
                if (SCardServiceHost == null)
                {
                    SCardServiceHost = new ServiceHost(typeof(RemoteCard), new Uri(NET_TCP_BASE_ADDRESS));
                    NetTcpBinding serviceBinding = new NetTcpBinding(SecurityMode.None);
                    serviceBinding.TransactionFlow = true;
                    SCardServiceHost.AddServiceEndpoint(typeof(IRemoteCard), serviceBinding, SERVCICE_ADDRESS);

                    AddMetadataEchange(SCardServiceHost);
                }

                switch (SCardServiceHost.State)
                {
                    case CommunicationState.Closed:
                        {
                            SCardServiceHost = null;
                            started = StartService();
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
                            SCardServiceHost.Open();
                            listBoxStatus.Items.Add("Smart card service started and running...");
                            started = true;
                            break;
                        }
                }

                SetButtonState();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return started;
        }

        private void AddMetadataEchange(ServiceHost host)
        {
            ServiceMetadataBehavior mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);

            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                Path.Combine(NET_TCP_BASE_ADDRESS, SERVCICE_ADDRESS, "mex"));
        }

        private void StopService()
        {
            if (SCardServiceHost != null)
            {
                switch (SCardServiceHost.State)
                {
                    case CommunicationState.Opened:
                        {
                            SCardServiceHost.Close();
                            while (SCardServiceHost.State != CommunicationState.Closed)
                            {
                                // Wait!
                            }
                            listBoxStatus.Items.Add("Smart card service stopped");
                            SCardServiceHost = null;
                            break;
                        }
                }
            }

            SetButtonState();
        }

        private void SetButtonState()
        {
            if (SCardServiceHost != null)
            {
                switch (SCardServiceHost.State)
                {
                    case CommunicationState.Closed:
                    case CommunicationState.Closing:
                    case CommunicationState.Created:
                        {
                            btnStart.Enabled = true;
                            break;
                        }

                    case CommunicationState.Opened:
                    case CommunicationState.Opening:
                        {
                            btnStart.Enabled = false;
                            break;
                        }
                }
            }
            else
            {
                btnStart.Enabled = true;
            }

            btnStop.Enabled = !btnStart.Enabled;
        }
        #endregion

        private void btnStart_Click(object sender, EventArgs e)
        {
            StartService();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopService();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
