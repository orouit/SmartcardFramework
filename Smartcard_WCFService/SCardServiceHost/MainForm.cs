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
    /// <summary>
    /// Windows form to host the SCardService
    /// If NET_TCP is defined, the service is using a NetTCPBinding,
    /// otherwise it is using NamedPipeBinding
    /// </summary>
    public partial class MainForm : Form
    {
        const string 
#if NET_TCP
            NET_TCP_BASE_ADDRESS = "net.tcp://localhost:8001/",
#else
            NET_NAMED_PIPE_BASE_ADDRESS = "net.pipe://localhost/",
#endif
            SERVCICE_ADDRESS = "SCardService";

#if NET_TCP
        private TCPServiceHostManager<IRemoteCard, RemoteCard> 
#else
        private NamedPipeServiceHostManager<IRemoteCard, RemoteCard> 
#endif
            scardService = null;

        public MainForm()
        {
            InitializeComponent();

#if NET_TCP
            scardService = new TCPServiceHostManager<IRemoteCard, RemoteCard>(NET_TCP_BASE_ADDRESS, SERVCICE_ADDRESS);
#else
            scardService = new NamedPipeServiceHostManager<IRemoteCard, RemoteCard>(NET_NAMED_PIPE_BASE_ADDRESS, SERVCICE_ADDRESS);
#endif

            scardService.Closed += ServiceHost_Closed;
            scardService.Opened += ServiceHost_Opened;

            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        void ServiceHost_Opened(object sender, EventArgs e)
        {
            listBoxStatus.Items.Add("Smart card service started and running...");
            btnStop.Enabled = true;
            btnStart.Enabled = false;
        }

        void ServiceHost_Closed(object sender, EventArgs e)
        {
            listBoxStatus.Items.Add("Smart card service stopped.");
            btnStart.Enabled = true;
            btnStop.Enabled = false;
        }

        protected override void OnClosed(EventArgs e)
        {
            if (scardService != null)
            {
                scardService.Closed -= ServiceHost_Closed;
                scardService.Opened -= ServiceHost_Opened;
                scardService.Dispose();
            }
            base.OnClosed(e);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            try
            {
                scardService.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                scardService.Stop();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
