using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

using GemCard;

namespace SmartCardPlayer
{
    /// <summary>
    /// List of sequence name. It is just a Dictionary of string indexed by string
    /// </summary>
    public class SequenceParameter : Dictionary<string, string> {
    }


	/// <summary>
	/// This class provides a set of functions to process APDU commands described in
	/// an XML format
	/// 
	/// The format is the following
	/// 
	/// <CommandList>
	///		<Apdu Name="VerifyCHV" Class="A0" Ins="20" P1="0" P2="1" P3="8" Data="31323334FFFFFFFF" />
	///		<Apdu Name="Get Response" Class="A0" Ins="C0" P1="0" P2="0" P3="SW2" Data="" />
	///		<Apdu Name="Select 6F3A" Class="A0" Ins="A4" P1="0" P2="0" P3="2" Data="6F3A" />
	///		<Apdu Name="Read Record" Class="A0" Ins="B2" P1="1" P2="4" P3="D15" Data="" />
	///	</CommandList>
	/// </summary>
	public class APDUPlayer
	{
		private const	string	
			xmlNodeApdu = "Apdu",
            xmlNodeSequence = "Sequence",
            xmlNodeCommand = "Command",
            xmlAttrApdu = "Apdu",
            xmlAttrSequence = "Sequence",
			xmlAttrName = "Name",
			xmlAttrClass = "Class",
			xmlAttrIns = "Ins",
			xmlAttrP1 = "P1",
			xmlAttrP2 = "P2",
			xmAttrLe = "Le",
			xmlAttrLc = "Lc",
			xmlAttrData = "Data";

		private	const	string
			paramSW1 = "SW1",
			paramSW2 = "SW2";

		private	bool	m_bLeSW2 = false;
        private bool    m_bLeData = false;
        private short   m_nDataId = -1;
		private	byte	m_bSW1Cond = 0;
		private	bool	m_bCheckSW1 = false;
        private bool    m_bReplay = false;

        private XmlNodeList m_xmlApduList = null;
        private XmlNodeList m_xmlSequenceList = null;
		private	ICard		m_iCard = null;
		private	APDUResponse	m_apduResp = null;

        private APDULogList m_logList = new APDULogList();

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="apduFileName">XML file name with the description of the commands</param>
		/// <param name="iCard">Card interface to process the APDUs</param>
		public APDUPlayer(string apduFileName, ICard iCard)
		{
			XmlDocument	apduDoc = new XmlDocument();

			try
			{
				// Load the document
				apduDoc.Load(apduFileName);

				// Get the list of APDUs
				m_xmlApduList = apduDoc.GetElementsByTagName(xmlNodeApdu);	

                // Get the list of sequences
                m_xmlSequenceList = apduDoc.GetElementsByTagName(xmlNodeSequence);

				m_iCard = iCard;
			}
			catch
			{
                throw new ApduCommandException(ApduCommandException.NotValidDocument);
			}
		}

        public APDULogList Log
        {
            get
            {
                return m_logList;
            }
        }


        /// <summary>
        /// Constructs an APDU player
        /// </summary>
        /// <param name="iCard">ICard interface to the Smartcard</param>
        public APDUPlayer(ICard iCard)
        {
            m_iCard = iCard;
        }


        /// <summary>
        /// Constructs an APDU player
        /// </summary>
        /// <param name="apduFileName">APDU file name</param>
        /// <param name="seqFileName">Sequence file name</param>
        /// <param name="iCard">ICard interface to the Smartcard</param>
        public APDUPlayer(string apduFileName, string seqFileName, ICard iCard)
        {
            LoadAPDUFile(apduFileName);
            LoadSequenceFile(seqFileName);

            m_iCard = iCard;
        }


        /// <summary>
        /// Loads a Sequence file
        /// </summary>
        /// <param name="fileName">Sequence file name</param>
        public void LoadAPDUFile(string fileName)
        {
            XmlDocument apduDoc = new XmlDocument();

            try
            {
                // Load the document
                apduDoc.Load(fileName);

                // Get the list of sequences
                m_xmlApduList = apduDoc.GetElementsByTagName(xmlNodeApdu);
            }
            catch
            {
                throw new ApduCommandException(ApduCommandException.NotValidDocument);
            }
        }


        /// <summary>
        /// Loads an APDU file
        /// </summary>
        /// <param name="fileName">APDU file name</param>
        public void LoadSequenceFile(string fileName)
        {
            XmlDocument apduDoc = new XmlDocument();

            try
            {
                // Load the document
                apduDoc.Load(fileName);

                // Get the list of sequences
                m_xmlSequenceList = apduDoc.GetElementsByTagName(xmlNodeSequence);
            }
            catch
            {
                throw new ApduCommandException(ApduCommandException.NotValidDocument);
            }
        }


		/// <summary>
		/// APDUNames property, gets a list of the APDU Names
		/// </summary>
		public	string[] APDUNames
		{
			get
			{
				string[] apduNames = null;
				int	nCount = m_xmlApduList.Count;

				if (nCount != 0)
				{
					apduNames = new string[nCount];
					for (int nI = 0; nI < nCount; nI++)
					{
						XmlNode apdu = m_xmlApduList.Item(nI);

						apduNames[nI] = apdu.Attributes[xmlAttrName].Value;
					}
				}

				return apduNames;
			}
		}


        /// <summary>
        /// Process a simple APDU command, Parameters can be provided in the APDUParam object
        /// </summary>
        /// <param name="command">APDU command name</param>
        /// <param name="apduParam">Parameters for the command</param>
        /// <returns>An APDUResponse object with the response of the card </returns>
        public APDUResponse ProcessCommand(string apduName, APDUParam apduParam)
		{
			APDUCommand		apduCmd = null;
		
			// Get the base APDU
			apduCmd = APDUByName(apduName);
			if (apduCmd == null)
                throw new ApduCommandException(ApduCommandException.NoSuchCommand);

			apduCmd.Update(apduParam);
			
			return ExecuteApduCommand(apduCmd);
		}


		/// <summary>
		/// Process a simple APDU command, all parameters are included in the 
		/// XML description
		/// </summary>
		/// <param name="command">APDU command name</param>
		/// <returns>An APDUResponse object with the response of the card </returns>
		public	APDUResponse	ProcessCommand(string apduName)
		{
			APDUCommand		apduCmd = null;
		
			apduCmd = APDUByName(apduName);
			if (apduCmd == null)
                throw new ApduCommandException(ApduCommandException.NoSuchCommand);

			return ExecuteApduCommand(apduCmd);
		}


        /// <summary>
        /// Process an APDU sequence and execute each of its commands in the sequence order
        /// </summary>
        /// <param name="apduSequenceName">Name of the sequence to play</param>
        /// <returns>APDUResponse object of the last commad executed</returns>
        public APDUResponse ProcessSequence(string apduSequenceName)
        {
            return ProcessSequence(apduSequenceName, null);
        }


        /// <summary>
        /// Process an APDU sequence and execute each of its commands in the sequence order
        /// </summary>
        /// <param name="apduSequenceName">Name of the sequence to play</param>
        /// <param name="seqParam">An array of SequenceParam object used as parameters for the sequence</param>
        /// <returns>APDUResponse object of the last command executed</returns>
        //public APDUResponse ProcessSequence(string apduSequenceName,  Dictionary<string, string> seqParam)
        public APDUResponse ProcessSequence(string apduSequenceName, SequenceParameter seqParam)
        {
            APDUResponse apduResp = null;
            //Dictionary<string, string> l_seqParam = null;
            SequenceParameter l_seqParam = null;

            // Get the sequence
            XmlNode apduSeq = SequenceByName(apduSequenceName);

            if (apduSeq == null)
                throw new ApduCommandException(ApduCommandException.NoSuchSequence);

            // Process the params of the sequence
            l_seqParam = ProcessParams(seqParam, apduSeq.Attributes);

            // Get the list of commands to execute
            XmlNodeList xmlCmdList = apduSeq.ChildNodes;
            for (int nI = 0; nI < xmlCmdList.Count; nI++)
                apduResp = ProcessSeqCmd(xmlCmdList.Item(nI), l_seqParam);

            return apduResp;
        }


        /// <summary>
        /// Gets an APDU command by name
        /// </summary>
        /// <param name="apduName">APDU name</param>
        /// <returns>An APDUCommand object, null if the command was not found</returns>
        public APDUCommand APDUByName(string apduName)
        {
            APDUCommand apduCmd = null;

            if (m_xmlApduList != null)
            {
                for (int nI = 0; nI < m_xmlApduList.Count; nI++)
                {
                    XmlNode apdu = m_xmlApduList.Item(nI);

                    string sName = apdu.Attributes[xmlAttrName].Value;
                    if (sName == apduName)
                    {
                        apduCmd = APDUFromXml(apdu);
                        break;
                    }
                }
            }

            return apduCmd;
        }

        #region Private methods


		/// <summary>
		/// Builds an APDUCommand object from an XmlNode representing the command.
		/// 
		/// This command uses the result of a previous command to fill some paramaters
        /// P3 in this case represents the length expected by the command
		/// P3 = "R,0:SW1?xx"
        ///     m_bReplay is set to true
		///		m_bSW1Cond is set with the value xx
		///		m_bCheckSW1 is set to true
		///		When the command is called, if SW1 == xx, the command is played a 
		///		second time with Le = resp.SW2
		///
        /// P3 = "R,xx:DRyy"
        ///     m_bReplay is set to true
        ///     m_bLeData is set to true
        ///     m_nDataId is set to yy
        ///     Le is set to xx for the first call of the command
        ///     Then the command is replayed with Le = Le + resp.Data[m_nDataId - 1]
        /// 
		///	P3 = "SW2"
		///		if SW1 == 0x9F on the previous call to a command, m_bLeSW2 is set to true
		///		if m_bLeSW2 if true, Le is replaced with resp.SW2
		///		
		///	P3 = "Dxx"
		///		if resp.Data id not null from the previous command,
		///		xx is used as the index of the byte that gives Le in the response data
		///		Le = Data[xx]
		/// </summary>
		/// <param name="xmlApdu">XML representation of the command</param>
		/// <returns>An APDUCommand object build from the XML data</returns>
        private APDUCommand APDUFromXml(XmlNode xmlApdu)
        {
            XmlElement apduElt = (XmlElement)xmlApdu;

            m_bLeData = false;
            m_bCheckSW1 = false;
            m_bReplay = false;

            // Get command detail
            string sClass = (string)xmlApdu.Attributes["Class"].Value;
            string sIns = (string)xmlApdu.Attributes["Ins"].Value;
            string sP1 = (string)xmlApdu.Attributes["P1"].Value;
            string sP2 = (string)xmlApdu.Attributes["P2"].Value;
            string sP3 = (string)xmlApdu.Attributes["P3"].Value;

            sP3 = sP3.ToUpper();
            string sData = apduElt.GetAttribute("Data");

            byte bP1 = 0;
            byte bP2 = 0;
            byte bP3 = 0;
            byte bLe = 0;
            byte bClass = byte.Parse(sClass, NumberStyles.AllowHexSpecifier);
            byte bIns = byte.Parse(sIns, NumberStyles.AllowHexSpecifier);
            if (sP1 != "" && sP1 != "@")
                bP1 = byte.Parse(sP1, NumberStyles.AllowHexSpecifier);
            if (sP2 != "" && sP2 != "@")
                bP2 = byte.Parse(sP2, NumberStyles.AllowHexSpecifier);

            int nId = 0;
            int nId2 = 0;

            // Process P3 parameter
            if (sP3.IndexOf("DR") != -1)
            {
                // Use data byte of previous command, index value follows D
                try
                {
                    int nIdx = int.Parse(sP3.Substring(sP3.IndexOf("DR") + 2));
                    if ((m_apduResp != null) && (m_apduResp.Data != null))
                        bLe = m_apduResp.Data[nIdx - 1];
                    else
                        bLe = 0;
                }
                catch
                {
                    throw new ApduCommandException(ApduCommandException.ParamP3Format);
                }
            }
            else if (sP3.IndexOf("DL") != -1)
            {
                bP3 = (byte)(sData.Length / 2);
                bLe = 0;
            }
            else if (sP3.IndexOf(paramSW2) != -1)
            {
                if (m_bLeSW2)
                {
                    // Use SW2 parameter of previous command
                    bLe = m_apduResp.SW2;
                }
                else
                    bLe = 0;
            }
            else if ((nId = sP3.IndexOf('R')) == 0)
            {
                m_bReplay = true;
                if (sP3[++nId] == ',')
                {
                    nId2 = sP3.IndexOf(':');
                    if (nId2 != -1)
                    {
                        bLe = byte.Parse(sP3.Substring(nId + 1, nId2 - nId - 1));
                    }
                    else
                        throw new ApduCommandException(ApduCommandException.ParamP3Format);
                }
                else if (sP3[nId] == ':')
                {
                    bLe = 0;
                }

                if (sP3.IndexOf(paramSW1, nId) != -1)
                {
                    if ((nId2 = sP3.IndexOf('?', nId)) != -1)
                    {
                        m_bCheckSW1 = true;
                        m_bSW1Cond = byte.Parse(sP3.Substring(nId2 + 1), NumberStyles.AllowHexSpecifier);
                    }
                    else
                        throw new ApduCommandException(ApduCommandException.ParamP3Format);
                }
                else if ((nId2 = sP3.IndexOf("DR", nId)) != -1)
                {
                    m_bLeData = true;
                    m_nDataId = short.Parse(sP3.Substring(nId2 + 2));
                }
                else
                    throw new ApduCommandException(ApduCommandException.ParamP3Format);
            }
            else if (sP3 != "")
            {
                bP3 = byte.Parse(sP3);
                if (sData.Length == 0)
                    bLe = bP3;
            }

            byte[] baData = null;
            if (bP3 != 0 && sData.Length != 0)
            {
                baData = new byte[bP3];
                for (int nJ = 0; nJ < sData.Length; nJ += 2)
                    baData[nJ / 2] = byte.Parse(sData.Substring(nJ, 2), NumberStyles.AllowHexSpecifier);
                bLe = 0;
            }

            return new APDUCommand(bClass, bIns, bP1, bP2, baData, bLe);
        }


        /// <summary>
        /// Executes an APDU command
        /// </summary>
        /// <param name="apduCmd">APDUCommand object to execute</param>
        /// <returns>APDUResponse object of the response</returns>
        private APDUResponse ExecuteApduCommand(APDUCommand apduCmd)
        {
            byte bLe = 0;

            // Send the command
            m_apduResp = m_iCard.Transmit(apduCmd);
            AddLog(new APDULog(apduCmd, m_apduResp));

            // Check if SW2 can be used as Le for the next call
            if (m_apduResp.SW1 == 0x9F)
                m_bLeSW2 = true;
            else
                m_bLeSW2 = false;

            if (m_bReplay)
            {
                if (m_bCheckSW1 && (m_apduResp.SW1 == m_bSW1Cond))
                {
                    // Replay the command with Le = SW2 of response
                    bLe = m_apduResp.SW2;
                    m_bCheckSW1 = false;
                }
                else if (m_bLeData)
                {
                    // Replay the command with Le = Le + Data[m_nDataId - 1] of response
                    bLe = (byte)(m_apduResp.Data[m_nDataId - 1] + apduCmd.Le);
                    m_bLeData = false;
                }

                // Replay the command
                apduCmd = new APDUCommand(
                    apduCmd.Class,
                    apduCmd.Ins,
                    apduCmd.P1,
                    apduCmd.P2,
                    apduCmd.Data,
                    bLe);

                m_apduResp = m_iCard.Transmit(apduCmd);
                AddLog(new APDULog(apduCmd, m_apduResp));

                m_bReplay = false;
            }

            return m_apduResp;
        }


        /// <summary>
        /// Gets the XML node for a Sequence of APDUs
        /// </summary>
        /// <param name="name">Name of the sequence</param>
        /// <returns>XmlNode of the sequence</returns>
        private XmlNode SequenceByName(string name)
        {
            XmlNode apduSeq = null;

            if (m_xmlSequenceList != null)
            {
                for (int nI = 0; nI < m_xmlSequenceList.Count; nI++)
                {
                    apduSeq = m_xmlSequenceList.Item(nI);

                    string sName = apduSeq.Attributes[xmlAttrName].Value;
                    if (sName == name)
                        break;
                    else
                        apduSeq = null;
                }
            }

            return apduSeq;
        }


        /// <summary>
        /// Process the parameters of a Sequence. If a parameter of the same name is found in the list of parameters
        /// it overrides the XML parameter of the sequence
        /// </summary>
        /// <param name="seqParam">List of parameters</param>
        /// <param name="xmlSeqParam">Parameters of the XML sequence</param>
        /// <returns>The list of parameters to used to process the sequence of APDU commands</returns>
//        private Dictionary<string, string> ProcessParams(Dictionary<string, string> seqParam, XmlAttributeCollection xmlSeqParam)
        private SequenceParameter ProcessParams(SequenceParameter seqParam, XmlAttributeCollection xmlSeqParam)
        {
            //Dictionary<string, string> l_seqParam = new Dictionary<string, string>();
            SequenceParameter l_seqParam = new SequenceParameter();

            int nNbParam = xmlSeqParam.Count;

            for (int nI = 0; nI < nNbParam; nI++)
            {
                XmlNode xNode = xmlSeqParam.Item(nI);

                string name = xNode.Name;
                string val = xNode.Value;

                // Check if a val overrides the XML parameter of Sequence
                if (seqParam != null)
                {
                    try
                    {
                        val = seqParam[name];
                    }
                    catch
                    {
                    }
                }

                l_seqParam.Add(name, val);
            }

            return l_seqParam;
        }


        /// <summary>
        /// Builds an APDUParam object from the parameters of a command and a set of parameter for a sequence
        /// </summary>
        /// <param name="xmlAttrs">List of parameters of the APDU</param>
        /// <param name="seqParam">List of parameters of the sequence</param>
        /// <returns>APDUParam object</returns>
//        private APDUParam BuildCommandParam(XmlAttributeCollection xmlAttrs, Dictionary<string, string> seqParam)
        private APDUParam BuildCommandParam(XmlAttributeCollection xmlAttrs, SequenceParameter seqParam)
        {
            APDUParam apduParam = null;
            byte[] baData = null;
            string sVal = null;

            apduParam = new APDUParam();

            for (int nI = 0; nI < xmlAttrs.Count; nI++)
            {
                XmlNode xmlParam = xmlAttrs.Item(nI);
                switch (xmlParam.Name)
                {
                    case xmlAttrP1:
                    {
                        try
                        {
                            sVal = seqParam[xmlParam.Value];
                        }
                        catch
                        {
                            sVal = xmlParam.Value;
                        }
                        finally
                        {
                            apduParam.P1 = byte.Parse(sVal, NumberStyles.AllowHexSpecifier);
                        }
                        break;
                    }

                    case xmlAttrP2:
                    {
                        try
                        {
                            sVal = seqParam[xmlParam.Value];
                        }
                        catch
                        {
                            sVal = xmlParam.Value;
                        }
                        finally
                        {
                            apduParam.P2 = byte.Parse(sVal, NumberStyles.AllowHexSpecifier);
                        }
                        break;
                    }

                    case xmlAttrData:
                    {
                        try
                        {
                            sVal = seqParam[xmlParam.Value];
                        }
                        catch
                        {
                            sVal = xmlParam.Value;
                        }
                        finally
                        {
                            int nLen = sVal.Length / 2;
                            if (nLen != 0)
                            {
                                baData = new byte[nLen];
                                for (int nJ = 0; nJ < nLen; nJ++)
                                    baData[nJ] = byte.Parse(sVal.Substring(nJ * 2, 2), NumberStyles.AllowHexSpecifier);

                                apduParam.Data = baData;
                            }
                        }
                        break;
                    }
                }
            }

            return apduParam;
        }


        /// <summary>
        /// Process a command of a sequence of APDU
        /// </summary>
        /// <param name="xmlCmd">XML node representing the command</param>
        /// <param name="seqParam">List of parameters of the sequence</param>
        /// <returns>APDUResponse object of command executed</returns>
//        private APDUResponse ProcessSeqCmd(XmlNode xmlCmd, Dictionary<string, string> seqParam)
        private APDUResponse ProcessSeqCmd(XmlNode xmlCmd, SequenceParameter seqParam)
        {
            bool bApdu = false;
            bool bSeq = false;
            string sApduName = null;
            string sSeqName = null;
            APDUResponse apduResp = null;

            // Get the APDU or Sequence name
            try
            {
                // Get the Apdu name to run
                sApduName = xmlCmd.Attributes[xmlAttrApdu].Value;
                bApdu = true;
            }
            catch
            {
            }
            finally
            {
                try
                {
                    sSeqName = xmlCmd.Attributes[xmlAttrSequence].Value;
                    bSeq = true;
                }
                catch
                {
                }

                if ((bSeq | bApdu) == false)
                    throw new ApduCommandException(ApduCommandException.MissingApduOrCommand);
            }

            if (bApdu)
            {
                APDUParam apduParams = BuildCommandParam(xmlCmd.Attributes, seqParam);
                apduResp = ProcessCommand(sApduName, apduParams);
            }

            if (bSeq)
            {
                // Process a sub sequence
                apduResp = ProcessSequence(sSeqName, seqParam);
            }

            return apduResp;
        }

        private void AddLog(APDULog apduLog)
        {
            m_logList.Add(apduLog);
        }
        #endregion
    }
}
