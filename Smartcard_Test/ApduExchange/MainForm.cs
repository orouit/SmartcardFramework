using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;
using System.Xml;

using Core.Smartcard;
using SmartCardPlayer;
using Core.Utility;

namespace TestGemCard
{
    delegate void EnableButtonDelegate(Button btn, bool state);
    delegate void SetTextBoxTextDelegate(TextBox txtBox, string text);

	/// <summary>
	/// MainForm of the Application
	/// </summary>
	public class MainForm : System.Windows.Forms.Form
    {
		private System.Windows.Forms.Button btnConnect;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnDisconnect;
		private System.Windows.Forms.Button btnTransmit;
		private System.Windows.Forms.StatusBar statusBar;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel_Sw;
		private System.Windows.Forms.StatusBarPanel statusBarPanel_Info;
		private System.Windows.Forms.ComboBox comboApdu;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textData;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textClass;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textIns;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textP1;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textP2;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textLe;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textDOut;
		private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox comboReader;
        private System.Windows.Forms.TextBox txtboxATR;
        private System.Windows.Forms.CheckBox checkBoxEnterAPDUManually;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private	CardBase iCard = null;
		private	APDUPlayer apduPlayer = null;
		private	APDUParam apduParam = null;

		const string APDU_LIST_FILE = "ApduList.xml";
        const string DEFAULT_READER = "Gemplus USB Smart Card Reader 0";

		public MainForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Setup the panels
			statusBarPanel_Info.BorderStyle = StatusBarPanelBorderStyle.Sunken;
			statusBarPanel_Info.AutoSize = StatusBarPanelAutoSize.Spring;

			statusBarPanel_Sw.BorderStyle = StatusBarPanelBorderStyle.Raised;
			statusBarPanel_Sw.AutoSize = StatusBarPanelAutoSize.Spring;

			statusBar.ShowPanels = true;

            SelectICard();

			SetupReaderList();
			LoadApduList();
            checkBoxEnterAPDUManually.Checked = true;
            EnableEnterAPDUManually(true);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.btnConnect = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnDisconnect = new System.Windows.Forms.Button();
            this.btnTransmit = new System.Windows.Forms.Button();
            this.statusBar = new System.Windows.Forms.StatusBar();
            this.statusBarPanel_Sw = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel_Info = new System.Windows.Forms.StatusBarPanel();
            this.comboApdu = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textDOut = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.textLe = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.textP2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textP1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textIns = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textClass = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.textData = new System.Windows.Forms.TextBox();
            this.comboReader = new System.Windows.Forms.ComboBox();
            this.txtboxATR = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.checkBoxEnterAPDUManually = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel_Sw)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel_Info)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(11, 32);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 2;
            this.btnConnect.Text = "Connect";
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Reader name";
            // 
            // btnDisconnect
            // 
            this.btnDisconnect.Location = new System.Drawing.Point(92, 32);
            this.btnDisconnect.Name = "btnDisconnect";
            this.btnDisconnect.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnect.TabIndex = 5;
            this.btnDisconnect.Text = "Disconnect";
            this.btnDisconnect.Click += new System.EventHandler(this.btnDisconnect_Click);
            // 
            // btnTransmit
            // 
            this.btnTransmit.Location = new System.Drawing.Point(336, 72);
            this.btnTransmit.Name = "btnTransmit";
            this.btnTransmit.Size = new System.Drawing.Size(75, 23);
            this.btnTransmit.TabIndex = 6;
            this.btnTransmit.Text = "Transmit";
            this.btnTransmit.Click += new System.EventHandler(this.btnTransmit_Click);
            // 
            // statusBar
            // 
            this.statusBar.Location = new System.Drawing.Point(0, 268);
            this.statusBar.Name = "statusBar";
            this.statusBar.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel_Sw,
            this.statusBarPanel_Info});
            this.statusBar.Size = new System.Drawing.Size(500, 24);
            this.statusBar.TabIndex = 7;
            // 
            // statusBarPanel_Sw
            // 
            this.statusBarPanel_Sw.Name = "statusBarPanel_Sw";
            // 
            // statusBarPanel_Info
            // 
            this.statusBarPanel_Info.Name = "statusBarPanel_Info";
            // 
            // comboApdu
            // 
            this.comboApdu.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboApdu.FormattingEnabled = true;
            this.comboApdu.Location = new System.Drawing.Point(120, 72);
            this.comboApdu.Name = "comboApdu";
            this.comboApdu.Size = new System.Drawing.Size(208, 21);
            this.comboApdu.TabIndex = 8;
            this.comboApdu.SelectedIndexChanged += new System.EventHandler(this.comboApdu_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(20, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 21);
            this.label2.TabIndex = 9;
            this.label2.Text = "APDU Command";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.textDOut);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.textLe);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textP2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textP1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.textIns);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.textClass);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.textData);
            this.groupBox1.Location = new System.Drawing.Point(12, 134);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 128);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "APDU";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(272, 16);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 16);
            this.label9.TabIndex = 13;
            this.label9.Text = "Received Data";
            // 
            // textDOut
            // 
            this.textDOut.Location = new System.Drawing.Point(272, 32);
            this.textDOut.Multiline = true;
            this.textDOut.Name = "textDOut";
            this.textDOut.ReadOnly = true;
            this.textDOut.Size = new System.Drawing.Size(128, 88);
            this.textDOut.TabIndex = 12;
            // 
            // label8
            // 
            this.label8.Location = new System.Drawing.Point(136, 16);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(56, 16);
            this.label8.TabIndex = 11;
            this.label8.Text = "Sent Data";
            // 
            // textLe
            // 
            this.textLe.Location = new System.Drawing.Point(32, 64);
            this.textLe.MaxLength = 4;
            this.textLe.Name = "textLe";
            this.textLe.Size = new System.Drawing.Size(40, 20);
            this.textLe.TabIndex = 10;
            // 
            // label7
            // 
            this.label7.Location = new System.Drawing.Point(8, 64);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 23);
            this.label7.TabIndex = 9;
            this.label7.Text = "Le";
            // 
            // textP2
            // 
            this.textP2.Location = new System.Drawing.Point(104, 40);
            this.textP2.MaxLength = 2;
            this.textP2.Name = "textP2";
            this.textP2.Size = new System.Drawing.Size(24, 20);
            this.textP2.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(80, 40);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 23);
            this.label6.TabIndex = 7;
            this.label6.Text = "P2";
            // 
            // textP1
            // 
            this.textP1.Location = new System.Drawing.Point(48, 40);
            this.textP1.MaxLength = 2;
            this.textP1.Name = "textP1";
            this.textP1.Size = new System.Drawing.Size(24, 20);
            this.textP1.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(8, 40);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 23);
            this.label5.TabIndex = 5;
            this.label5.Text = "P1";
            // 
            // textIns
            // 
            this.textIns.Location = new System.Drawing.Point(104, 16);
            this.textIns.MaxLength = 2;
            this.textIns.Name = "textIns";
            this.textIns.Size = new System.Drawing.Size(24, 20);
            this.textIns.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(80, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 23);
            this.label4.TabIndex = 3;
            this.label4.Text = "Ins";
            // 
            // textClass
            // 
            this.textClass.Location = new System.Drawing.Point(48, 16);
            this.textClass.MaxLength = 2;
            this.textClass.Name = "textClass";
            this.textClass.Size = new System.Drawing.Size(24, 20);
            this.textClass.TabIndex = 2;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(8, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 23);
            this.label3.TabIndex = 1;
            this.label3.Text = "Class";
            // 
            // textData
            // 
            this.textData.Location = new System.Drawing.Point(136, 32);
            this.textData.Multiline = true;
            this.textData.Name = "textData";
            this.textData.Size = new System.Drawing.Size(128, 88);
            this.textData.TabIndex = 0;
            // 
            // comboReader
            // 
            this.comboReader.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboReader.FormattingEnabled = true;
            this.comboReader.Location = new System.Drawing.Point(94, 6);
            this.comboReader.Name = "comboReader";
            this.comboReader.Size = new System.Drawing.Size(322, 21);
            this.comboReader.TabIndex = 12;
            this.comboReader.SelectedIndexChanged += new System.EventHandler(this.comboReader_SelectedIndexChanged);
            // 
            // txtboxATR
            // 
            this.txtboxATR.Location = new System.Drawing.Point(205, 34);
            this.txtboxATR.Name = "txtboxATR";
            this.txtboxATR.ReadOnly = true;
            this.txtboxATR.Size = new System.Drawing.Size(283, 20);
            this.txtboxATR.TabIndex = 13;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(173, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(29, 13);
            this.label10.TabIndex = 14;
            this.label10.Text = "ATR";
            // 
            // checkBoxEnterAPDUManually
            // 
            this.checkBoxEnterAPDUManually.AutoSize = true;
            this.checkBoxEnterAPDUManually.Location = new System.Drawing.Point(23, 99);
            this.checkBoxEnterAPDUManually.Name = "checkBoxEnterAPDUManually";
            this.checkBoxEnterAPDUManually.Size = new System.Drawing.Size(129, 17);
            this.checkBoxEnterAPDUManually.TabIndex = 15;
            this.checkBoxEnterAPDUManually.Text = "Enter APDU Manually";
            this.checkBoxEnterAPDUManually.UseVisualStyleBackColor = true;
            this.checkBoxEnterAPDUManually.CheckedChanged += new System.EventHandler(this.CheckBoxEnterAPDUManually_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(500, 292);
            this.Controls.Add(this.checkBoxEnterAPDUManually);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txtboxATR);
            this.Controls.Add(this.comboReader);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboApdu);
            this.Controls.Add(this.statusBar);
            this.Controls.Add(this.btnTransmit);
            this.Controls.Add(this.btnDisconnect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnConnect);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "MainForm";
            this.Text = "Smart card Exchange APDU";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel_Sw)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel_Info)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new MainForm());
		}

		private void btnConnect_Click(object sender, System.EventArgs e)
		{
			try
			{
				iCard.Connect((string) comboReader.SelectedItem, SHARE.Shared, PROTOCOL.T0orT1);

                try
                {
                    // Get the ATR of the card
                    byte[] atrValue = iCard.GetAttribute(SCARD_ATTR_VALUE.ATR_STRING);
                    txtboxATR.Text = ByteArray.ToString(atrValue);
                }
                catch (Exception)
                {
                    txtboxATR.Text = "Cannot get ATR";
                }

				btnConnect.Enabled = false;
				btnDisconnect.Enabled = true;
				btnTransmit.Enabled = true;

				statusBarPanel_Info.Text = "Card connected";
			}
			catch(Exception ex)
			{
				btnConnect.Enabled = true;
				btnDisconnect.Enabled = false;
				btnTransmit.Enabled = false;

				statusBar.Text = ex.Message;
			}
		}

		private void btnDisconnect_Click(object sender, System.EventArgs e)
		{
			try
			{
				iCard.Disconnect(DISCONNECT.Unpower);

				btnConnect.Enabled = true;
				btnDisconnect.Enabled = false;
				btnTransmit.Enabled = false;
                txtboxATR.Text = string.Empty;

				statusBarPanel_Info.Text = "Card disconnected";
			}
			catch(Exception ex)
			{
				statusBar.Text = ex.Message;
			}
		}

		private void btnTransmit_Click(object sender, System.EventArgs e)
		{
			try
			{
                APDUResponse apduResp = null;
                if (checkBoxEnterAPDUManually.Checked)
                {
                    apduResp = apduPlayer.ProcessCommand(textClass.Text, textIns.Text, BuildAPDUParameters());
                }
                else
                {
                    apduResp = apduPlayer.ProcessCommand((string)comboApdu.SelectedItem, BuildAPDUParameters());
                }
				
                textDOut.Text = (apduResp.Data != null) 
                    ? ByteArray.ToString(apduResp.Data)
                    : string.Empty;

				statusBarPanel_Sw.Text = string.Format("{0:X04}", apduResp.Status);
				statusBarPanel_Info.Text = "Command sent";
			}
			catch(SmartCardException exSC)
			{
				statusBarPanel_Info.Text = exSC.Message;
			}
			catch(Exception ex)
			{
				statusBarPanel_Info.Text = ex.Message;
			}
		}

		private void SelectICard()
		{
			try
			{
                if (iCard != null)
                {
                    iCard.Disconnect(DISCONNECT.Unpower);
                }

				iCard = new CardNative();
				statusBarPanel_Info.Text = "CardNative implementation used";

                iCard.OnCardInserted += new CardInsertedEventHandler(iCard_OnCardInserted);
                iCard.OnCardRemoved += new CardRemovedEventHandler(iCard_OnCardRemoved);
			}
			catch(Exception ex)
			{
				btnConnect.Enabled = false;
				btnDisconnect.Enabled = false;
				btnTransmit.Enabled = false;

				statusBarPanel_Info.Text = ex.Message;
			}
		}

        /// <summary>
        /// CardRemovedEventHandler
        /// </summary>
        private void iCard_OnCardRemoved(object sender, string reader)
        {
            if (this.InvokeRequired)
            {
                btnConnect.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnConnect, false });
                btnDisconnect.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnDisconnect, false });
                btnTransmit.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnTransmit, false });
                txtboxATR.Invoke(new SetTextBoxTextDelegate(SetText), new object[] { txtboxATR, string.Empty });
            }
            else
            {
                btnConnect.Enabled = false;
                btnDisconnect.Enabled = false;
                btnTransmit.Enabled = false;
                txtboxATR.Text = string.Empty;
            }
        }

        /// <summary>
        /// CardInsertedEventHandler
        /// </summary>
        private void iCard_OnCardInserted(object sender, string reader)
        {
            if (this.InvokeRequired)
            {
                btnConnect.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnConnect, true });
                btnDisconnect.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnDisconnect, false });
                btnTransmit.Invoke(new EnableButtonDelegate(EnableButton), new object[] { btnTransmit, false });
            }
            else
            {
                btnConnect.Enabled = true;
                btnDisconnect.Enabled = false;
                btnTransmit.Enabled = false;
            }
        }

        #region Invoke methods

        private void EnableButton(Button btn, bool enable)
        {
            btn.Enabled = enable;
        }

        private void SetText(TextBox txtBox, string text)
        {
            txtBox.Text = text;
        }

        #endregion
        
        /// <summary>
        /// Loads the APDU list
        /// </summary>
		private void	LoadApduList()
		{
			try
			{
				// Create the APDU player
                apduPlayer = new APDUPlayer(APDU_LIST_FILE, iCard);

				// Get the list of APDUs and setup teh combo
				comboApdu.Items.AddRange(apduPlayer.APDUNames);
				comboApdu.SelectedIndex = 0;
			}
			catch(Exception ex)
			{
				statusBarPanel_Info.Text = ex.Message;
			}
		}

		private	APDUParam BuildAPDUParameters()
		{
			byte	bP1 = byte.Parse(textP1.Text, NumberStyles.AllowHexSpecifier);
			byte	bP2 = byte.Parse(textP2.Text, NumberStyles.AllowHexSpecifier);
			byte	bLe = byte.Parse(textLe.Text);

			APDUParam	apduParam = new APDUParam();
			apduParam.P1 = bP1;
			apduParam.P2 = bP2;
            apduParam.Le = bLe;

            byte[] data = ByteArray.Parse(textData.Text);
            if (data.Length > 0)
            {
                apduParam.Data = data;
            }

			// Update Current param
			apduParam = apduParam.Clone();

			return apduParam;
		}

		private void comboApdu_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			DisplayAPDUCommand(apduPlayer.APDUByName((string) comboApdu.SelectedItem));

			statusBarPanel_Info.Text = "Command Ready";
			statusBarPanel_Sw.Text = "";
		}

		private	void DisplayAPDUCommand(APDUCommand apduCmd)
		{
			if (apduCmd != null)
			{
				textClass.Text = string.Format("{0:X02}", apduCmd.Class);
				textIns.Text = string.Format("{0:X02}", apduCmd.Ins);
				textP1.Text = string.Format("{0:X02}", apduCmd.P1);
				textP2.Text = string.Format("{0:X02}", apduCmd.P2);
				textLe.Text = apduCmd.Le.ToString();

                textData.Text = (apduCmd.Data != null)
                    ? ByteArray.ToString(apduCmd.Data)
                    : string.Empty;

				apduParam = new APDUParam();
                
                apduParam.P1 = apduCmd.P1;
                apduParam.P2 = apduCmd.P2;
                apduParam.Le = apduCmd.Le;
			}
		}

		private	void SetupReaderList()
		{
			try
			{
				string[] sListReaders = iCard.ListReaders();
				comboReader.Items.Clear();

				if (sListReaders != null)
				{
                    for (int nI = 0; nI < sListReaders.Length; nI++)
                    {
                        comboReader.Items.Add(sListReaders[nI]);
                    }

					comboReader.SelectedIndex = 0;

                    btnConnect.Enabled = false;
                    btnDisconnect.Enabled = false;
                    btnTransmit.Enabled = false;
				}
			}
			catch(Exception ex)
			{
				statusBarPanel_Info.Text = ex.Message;
				btnConnect.Enabled = false;
			}
		}

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                iCard.Disconnect(DISCONNECT.Unpower);
                iCard.StopCardEvents();
            }
            catch
            {
            }
        }

        /// <summary>
        /// If the selection changes, Stop the current Reader event and start the new one
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboReader_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                iCard.StopCardEvents();

                // Get the current selection
                int idx = comboReader.SelectedIndex;
                if (idx != -1)
                {
                    // Start waiting for a card
                    string reader = (string)comboReader.SelectedItem;
                    iCard.StartCardEvents(reader);

                    statusBarPanel_Info.Text = "Waiting for a card";
                }
            }
            catch (Exception ex)
            {
                statusBarPanel_Info.Text = ex.Message;
                btnConnect.Enabled = false;
            }
        }

        private void CheckBoxEnterAPDUManually_CheckedChanged(object sender, EventArgs e)
        {
            EnableEnterAPDUManually(checkBoxEnterAPDUManually.Checked);
        }

        private void EnableEnterAPDUManually(bool enable)
        {
            comboApdu.Enabled = !enable;
            textClass.Enabled = enable;
            textIns.Enabled = enable;

            if (enable)
            {
                textIns.Text = string.Empty;
                textClass.Text = string.Empty;
                textData.Text = string.Empty;
                textP1.Text = string.Empty;
                textP2.Text = string.Empty;
                textLe.Text = "0";
            }
        }
    }
}
