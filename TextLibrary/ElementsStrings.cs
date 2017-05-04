using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace TextLibrary
{

    /// <summary>
    /// Содержит функцию загрузки из файла данных о "текстах"
    /// для всех элементов окна
    /// </summary>
    public class ElementsStrings
    {
        List<ElementString> parameters;
        List<string> languages;

        /// <summary>
        /// Список название языков содержащихся в файле
        /// </summary>
        public List<string> Languages
        {
            get { return languages; }
            set { languages = value; }
        }

        public List<ElementString> Parameters
        {
            get { return parameters; }
            private set { parameters = value; }
        }

        public ElementsStrings(string FileName)
        {
            parameters = new List<ElementString>();

            var doc = new XmlDocument();
            try
            {
                doc.Load(FileName);
            }
            catch(Exception e)
            {
                parameters = new List<ElementString>();
                
                throw new Exception("Ошибка при открытии файла: " + FileName + "!\n"+ e.Message);
            }

            XmlNodeList xmls;
            XmlNode xmlline;
            int count = 0;
            int i, i1;
            string st;

            xmls = doc.GetElementsByTagName("Languages");
            if (xmls.Count != 1)
            {
                parameters = new List<ElementString>();
                throw new Exception("Ошибка при открытии файла: " + FileName + "!");
            }
            xmlline = xmls[0];

            try
            {
                count = Convert.ToInt32(xmlline.Attributes["Count"].Value);
            }
            catch (Exception e)
            {
                parameters = new List<ElementString>();
                throw new Exception("Ошибка при открытии файла: " + FileName + "!\n" + e.Message);
            }

            List<string> languagestmp = new List<string>();
            for (i = 0; i < count; i++)
            {
                try
                {
                    st = Convert.ToString(xmlline.Attributes["Language_" + i.ToString()].Value);
                }
                catch (Exception e)
                {
                    parameters = new List<ElementString>();
                    throw new Exception("Ошибка при открытии файла: " + FileName + "!\n" + e.Message);
                }
                languagestmp.Add(st);
            }

            languages = languagestmp;

            xmls = doc.GetElementsByTagName("Parameters");
            if (xmls.Count != 1)
            {
                parameters = new List<ElementString>();
                throw new Exception("Ошибка при открытии файла: " + FileName + "!");
            }
            xmlline = xmls[0];

            try
            {
                count = Convert.ToInt32(xmlline.Attributes["Count"].Value);
            }
            catch (Exception e)
            {
                parameters = new List<ElementString>();
                throw new Exception("Ошибка при открытии файла: " + FileName + "!\n" + e.Message);
            }


            for (i = 0; i < count; i++)
            {

                object[] values = new object[languages.Count + 3];

                xmls = doc.GetElementsByTagName("Parameter_" + i.ToString());
                if (xmls.Count != 1)
                {
                    parameters = new List<ElementString>();
                    throw new Exception("Ошибка при открытии файла: " + FileName + "!");
                }

                parameters.Add(new ElementString());
                
                
                
                                xmlline = xmls[0];
                                for (i1 = 0; i1 < languages.Count; i1++)
                                {
                                    try
                                    {
                                        st = Convert.ToString(xmlline.Attributes["InLanguage_" + i1.ToString()].Value);
                                    }
                                    catch
                                    {
                                        st = "";
                                    }
                                    parameters[i].ParamNames.Add(st);
                                }


                                bool limEna = true;
                                double d1 = 0;
                                double d2 = 0;
                                int i5 = 0;
                                try
                                {
                                    i5 = Convert.ToInt32(xmlline.Attributes["Limits"].Value);
                                    if (i5 == 0) { limEna = false; }
                                }
                                catch
                                {
                                    limEna = false;
                                }

                                if (limEna)
                                {
                                    try
                                    {
                                        d1 = Convert.ToDouble(xmlline.Attributes["Min"].Value);
                                        d2 = Convert.ToDouble(xmlline.Attributes["Max"].Value);
                                        if (d2 < d1) { limEna = false; }
                                        i5 = Convert.ToInt32(xmlline.Attributes["Point"].Value);
                                    }
                                    catch
                                    {
                                        limEna = false;
                                    }
                                }

                                parameters[i].LimitsEnabled = limEna;
                                if (limEna)
                                {
                                    parameters[i].Minimum = d1;
                                    parameters[i].Maximum = d2;
                                    parameters[i].PointCount = i5;
                                }
                                else
                                {
                                    parameters[i].Minimum = 0;
                                    parameters[i].Maximum = 0;
                                    parameters[i].PointCount = 0;
                                }
            }//count
        }

    }
}
