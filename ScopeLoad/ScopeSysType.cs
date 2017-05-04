using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Drawing;

namespace ScopeLoad
{
    public static class ScopeSysType
    {
        static string xmlFileName = "ScopeSysType.xml";
        public static List<string> ChannelNames = new List<string>();
        public static List<ushort> ChannelAddrs = new List<ushort>();
        public static List<Color> ChannelColors = new List<Color>();
        public static List<int> ChannelFormats = new List<int>();
        public static List<int> ChannelStepLines = new List<int>();

        public static ushort TimeStampAddr;
        public static ushort OscilStatusAddr;
        public static ushort ScopeCountAddr;
        public static ushort HystoryAddr;
        public static ushort ChannelCountAddr;
        public static ushort DataStartAddr;
        public static ushort OscilFreqAddr;
        public static ushort LoadOscilStartAddr;
        public static ushort ParamLoadConfigAddr;
        public static ushort ParamLoadDataAddr;

        static void LoadAddrFromXML(string paramName, XmlDocument doc, out ushort addr)
        {
            XmlNodeList xmls;
            XmlNode xmlline;


            xmls = doc.GetElementsByTagName(paramName);
            if (xmls.Count != 1)
            {
                addr = 0;
                throw new Exception("Ошибки в файле: " + xmlFileName + "!");
            }
            xmlline = xmls[0];

            try
            {
                addr = Convert.ToUInt16(xmlline.Attributes["Addr"].Value);
            }
            catch
            {
                addr = 0;
                throw new Exception("Ошибки в файле: " + xmlFileName + "!");
            }
        }
        public static void InitScopeSysType()
        {
            var doc = new XmlDocument();
            try
            {
                doc.Load(xmlFileName);
            }
            catch
            {
                throw new Exception("Не удалось открыть файл: " + xmlFileName + "!");
            }

            LoadAddrFromXML("TimeStamp", doc, out TimeStampAddr);
            LoadAddrFromXML("OscilStatus", doc, out OscilStatusAddr);
            LoadAddrFromXML("ScopeCount", doc, out ScopeCountAddr);
            LoadAddrFromXML("Hystory", doc, out HystoryAddr);
            LoadAddrFromXML("ChannelCount", doc, out ChannelCountAddr);
            LoadAddrFromXML("DataStart", doc, out DataStartAddr);
            LoadAddrFromXML("OscilFreq", doc, out OscilFreqAddr);
            LoadAddrFromXML("LoadOscilStart", doc, out LoadOscilStartAddr);
            LoadAddrFromXML("ParamLoadConfig", doc, out ParamLoadConfigAddr);
            LoadAddrFromXML("ParamLoadData", doc, out ParamLoadDataAddr);

            XmlNodeList xmls;
            XmlNode xmlline;
            ushort count = 0;

            xmls = doc.GetElementsByTagName("MeasureParams");
            if (xmls.Count != 1)
            {
                throw new Exception("Ошибки в файле: " + xmlFileName + "!");
            }
            xmlline = xmls[0];

            try
            {
                count = Convert.ToUInt16(xmlline.Attributes["Count"].Value);
            }
            catch
            {
                throw new Exception("Ошибки в файле: " + xmlFileName + "!");
            }

            ChannelNames = new List<string>();
            ChannelAddrs = new List<ushort>();
            ChannelColors = new List<Color>();
            ChannelFormats = new List<int>();
            ChannelStepLines = new List<int>();

            for (int i = 1; i < (count + 1); i++)
            {
                xmls = doc.GetElementsByTagName("MeasureParam" + i.ToString());
                if (xmls.Count != 1)
                {
                    throw new Exception("Ошибки в файле: " + xmlFileName + "!");
                }
                xmlline = xmls[0];

                try
                {
                    ChannelNames.Add(Convert.ToString(xmlline.Attributes["Name"].Value));
                    ChannelAddrs.Add(Convert.ToUInt16(xmlline.Attributes["Addr"].Value));
                    int ic = Convert.ToInt32(xmlline.Attributes["Color"].Value);
                    ChannelColors.Add(Color.FromArgb(ic));
                    ChannelFormats.Add(Convert.ToInt32(xmlline.Attributes["Format"].Value));
                    ChannelStepLines.Add(Convert.ToInt32(xmlline.Attributes["StepLine"].Value));
                }
                catch
                {
                    throw new Exception("Ошибки в файле: " + xmlFileName + "!");
                }
            }

        }
    }
}
