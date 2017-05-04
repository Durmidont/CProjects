using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Xml;
using System.Xml.Linq;
using System.Windows.Forms;
using System.IO;

namespace UPTF_Scope
{
    public static class ComsetClass
    {
        
        public static string comPortXMLName = "Comset.xml";
        public static string UPTFXMLName = "UPTF.xml";
        public static string ArhivePath = "c:\\Архив";
        public static string AppName = "UPTF";
        public static string errAppExit = "Empty";
        public static string errRead = "Empty";
        public static string errWrite = "Empty";
        public static string errFoundFile = "Empty";
        public static string errCreateFolder = "Empty";
        public static bool ArhiveFlag = false;
        public static List<DataRecord> DataRecords;
        private static bool[] enaRequest = { };
        public static bool[] EnaRequest
        {
            get { return enaRequest; }
            set { enaRequest = value; }
        }

        private static byte[] slaveAddrs = { };
        public static byte[] SlaveAddrs
        {
            get { return slaveAddrs; }
            set { slaveAddrs = value; }
        }

        private static byte comPortIndex = 0;
        public static byte ComPortIndex
        {
            get { return comPortIndex; }
            set { comPortIndex = value; }
        }

        public static string ComPortName
        {
            get
            {
                if (PortList.Count != 0)
                {
                    if (comPortIndex >= PortList.Count) { return PortList[0]; }
                    return (PortList[comPortIndex]);
                }
                else
                {
                    return ("");
                }
            }

        }

        public static byte SerialPortParityIndex { get; set; }
        public static byte SerialPortSpeedIndex { get; set; }

        public static Parity SerialPortParity
        {
            get
            {
                switch (SerialPortParityIndex)
                {
                    case 0: { return Parity.Odd; }
                    case 1: { return Parity.Even; }
                    case 2: { return Parity.None; }
                }
                return (Parity.Odd);
            }
        }

        public static StopBits SerialPortStopBits
        {
            get
            {
                switch (SerialPortParityIndex)
                {
                    case 0: { return StopBits.One; }
                    case 1: { return StopBits.One; }
                    case 2: { return StopBits.Two; }
                }
                return (StopBits.One);
            }

        }

        public static int BaudRate
        {
            get
            {
                switch (SerialPortSpeedIndex)
                {
                    case 0: { return (9600); }
                    case 1: { return (19200); }
                    case 2: { return (38400); }
                    case 3: { return (57600); }
                    case 4: { return (115200); }
                    case 5: { return (230400); }
                }
                return (9600);
            }

        }

        public static void LoadSettingsFromFile()
        {
            {
                int i1, i3, i4;
                
                XmlNodeList xmls;
                XmlNode xmlNode;

                if(!File.Exists(comPortXMLName))
                    throw new Exception(errFoundFile + " " + comPortXMLName + "!\n" + errAppExit); 

                var doc = new XmlDocument();
                try { doc.Load(comPortXMLName); }
                catch(Exception ex)
                {
                    throw new Exception(errRead + " " + comPortXMLName  + "!\n" + ex.Message);
                }

                xmls = doc.GetElementsByTagName("ComPort");

                if (xmls.Count != 1)
                {
                   
                    throw new Exception(errRead + " " + comPortXMLName + ".\n" + errAppExit);

                }

                xmlNode = xmls[0];

                i1 =  i3 = i4 = 0;

                             

                try
                {
                    i1 = Convert.ToInt32(xmlNode.Attributes["Name"].Value);
                    i3 = Convert.ToInt32(xmlNode.Attributes["Speed"].Value);
                    i4 = Convert.ToInt32(xmlNode.Attributes["Parity"].Value);

                    
                }
                catch (Exception ex)
                {
                    throw new Exception(errRead + " " + comPortXMLName + ".\n" + ex.Message);
                }
                           

                if ((PortList.Count - 1) < i1) { ComPortIndex = 0; } else { ComPortIndex = (byte)i1; }


               

                if ((i3 > 6) || (i3 < 0)) { SerialPortSpeedIndex = 0; } else { SerialPortSpeedIndex = (byte)i3; }
                if ((i4 > 2) || (i4 < 0)) { SerialPortParityIndex = 0; } else { SerialPortParityIndex = (byte)i4; }

           
                xmls = doc.GetElementsByTagName("Arhive");

                if (xmls.Count != 1)
                {
                    throw new Exception(errRead + " " + comPortXMLName + ".\n" + errAppExit);
                }

                xmlNode = xmls[0];
                string str="";
                try
                {
                    str = Convert.ToString(xmlNode.Attributes["Path"].Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(errRead + " " + comPortXMLName + ".\n" + ex.Message);
                }

                try
                {
                    System.IO.Directory.CreateDirectory(str);
                }
                catch (Exception ex)
                {
                    throw new Exception(errCreateFolder + " " + str  + ".\n" + ex.Message);
                }
                                
                ArhivePath = str;

                try
                {
                    ArhiveFlag = Convert.ToBoolean(xmlNode.Attributes["Ena"].Value);
                }
                catch (Exception ex)
                {
                    throw new Exception(errRead + " " + comPortXMLName + ".\n" + ex.Message);
                }


            }
        }

        public static void LoadUPTFFromFile()
        {
            {
           
                byte i=0;
                string str = "";
                DataRecords = new List<DataRecord>();

                if (!File.Exists(UPTFXMLName))
                    throw new Exception(errFoundFile + " " + UPTFXMLName + "!\n" + errAppExit);

                XDocument doc = new XDocument();
                try { doc = XDocument.Load(UPTFXMLName); }
                catch (Exception ex)
                {
                    throw new Exception(errRead + " " + UPTFXMLName + "!\n" + ex.Message);
                }

                foreach (XElement el in doc.Root.Elements())
                {
                    try
                    {
                        i = Convert.ToByte(el.Attribute("Addr").Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(errRead + " " + UPTFXMLName + "!\n" + ex.Message);
                    }
                                       
                    try
                    {
                        str = Convert.ToString(el.Attribute("Name").Value);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(errRead + " " + UPTFXMLName + "!\n" + ex.Message);
                    }

                    DataRecords.Add(new DataRecord(i, str));
                    /*
                    try
                    {
                        System.IO.Directory.CreateDirectory(ArhivePath + @"\" +str + @"\");
                    }
                    catch
                    {
                        MessageBox.Show(MainForm.ParameterName(230) + "\n" + MainForm.ParameterName(228),
                                        MainForm.ParameterName(0), MessageBoxButtons.OK, MessageBoxIcon.Error);
                        Application.Exit();
                    }

                    */

                }


            }
        }

        public static void SaveSettingsToFile()
        {
            try
            {
                var doc = new XmlDocument();
                doc.Load(comPortXMLName);

                XmlNodeList adds = doc.GetElementsByTagName("ComPort");
                foreach (XmlNode add in adds)
                {
                    add.Attributes["Name"].Value = ComPortIndex.ToString();
                    add.Attributes["Speed"].Value = SerialPortSpeedIndex.ToString();
                    add.Attributes["Parity"].Value = SerialPortParityIndex.ToString();
                    

                }
                adds = doc.GetElementsByTagName("Arhive");
                foreach (XmlNode add in adds)
                {
                    add.Attributes["Ena"].Value = ComsetClass.ArhiveFlag.ToString();
                    add.Attributes["Path"].Value = ComsetClass.ArhivePath;
                }
                doc.Save(comPortXMLName);
            }
            catch (Exception ex)
            {
                throw new Exception(errWrite + " " + comPortXMLName + "!\n" + ex.Message);
            }
           
        }

        //Список всех COM-портов на компьютере
        static List<string> portList = new List<string>();
        public static List<string> PortList
        {
            get
            {
                string[] portStrList;
                portStrList = SerialPort.GetPortNames();
                portList.Clear();
                foreach (string port in portStrList)
                {
                    portList.Add(port);
                }
                portList.Sort();
                return portList;
            }
            
        }
        
    }
}
