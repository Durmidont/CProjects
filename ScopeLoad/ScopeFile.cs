using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormatsConvert;
using System.ComponentModel;
using System.IO;


namespace ScopeLoad
{
    public class ScopeFile
    {
        public delegate void CreatingCompleter(bool LoadingOk);
        public event CreatingCompleter OnCreateComplete;

        List<ushort[]> downloadedData;
        ScopeConfig scopeConfig;
        BackgroundWorker backgroundWorker;
        string FileHeaderLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + ScopeSysType.ChannelNames[scopeConfig.OscillParams[i]] + "\t";
                }
            }
            return str;
        }
        string FileColorLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + ScopeSysType.ChannelColors[scopeConfig.OscillParams[i]].ToArgb().ToString() + "\t";
                }
            }
            return str;
        }
        string FileOffsetLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + "0,00000" + "\t";
                }
            }
            return str;
        }
        string FileScaleLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + "1,00000" + "\t";
                }
            }
            return str;
        }
        string FileReserveLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + "0" + "\t";
                }
            }
            return str;
        }
        string FileStepLine()
        {
            string str = "";
            int i = 0;
            str = " " + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    str = str + ScopeSysType.ChannelStepLines[scopeConfig.OscillParams[i]].ToString() + "\t";
                }
            }
            return str;
        }
        string FileParamLine(ushort[] paramLine, int lineNum)
        {
            string str = "";
            int i = 0;
            str = lineNum.ToString() + "\t";
            for (i = 0; i < scopeConfig.ChannelCount; i++)
            {
                //Если параметр в списке известных, то пишем его в файл
                if (scopeConfig.OscillParams[i] < ScopeSysType.ChannelNames.Count)
                {
                    string str2 = "0";
                    try
                    {
                        str2 = paramLine[i].ToFormat(ScopeSysType.ChannelFormats[scopeConfig.OscillParams[i]]).ToString("F5");
                    }
                    catch
                    {
                        str2 = "0";
                    }
                    str = str + str2 + "\t";
                }
            }
            return str;
        }
        List<ushort[]> InitParamsLines()
        {
            ushort countInLine = (ushort)Math.Round(32.0 / scopeConfig.ChannelCount);
            List<ushort[]> paramsLines = new List<ushort[]>();

            for (int i3 = 0; i3 < downloadedData.Count; i3++)
            {
                for (int i = 0; i < countInLine; i++)
                {
                    paramsLines.Add(new ushort[scopeConfig.ChannelCount]);
                    for (int i2 = 0; i2 < scopeConfig.ChannelCount; i2++)
                    {
                        paramsLines[paramsLines.Count - 1][i2] = downloadedData[i3][i * scopeConfig.ChannelCount + i2];
                    }
                }
            }
            return paramsLines;
        }
        string fileName;
        string oscTitl;
        
        public ScopeFile(string FileName, List<ushort[]> DownloadedData, ScopeConfig ScopeConfig, string OscTitl)
        {
            scopeConfig = ScopeConfig;
            downloadedData = DownloadedData;
            fileName = FileName;
            oscTitl = OscTitl;

            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(null);
        }
        
        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            StreamWriter sw;
            try
            {
                sw = File.CreateText(fileName);
            }
            catch
            {

                e.Cancel = true;
                return;
            }

            try
            {
                sw.WriteLine(oscTitl);//;oscTimeDates[createFileNum]);
                sw.WriteLine(scopeConfig.ScopeFreq.ToString());
                sw.WriteLine(scopeConfig.Hystory.ToString());
                sw.WriteLine(FileHeaderLine());
                sw.WriteLine(FileColorLine());
                sw.WriteLine(FileOffsetLine());
                sw.WriteLine(FileScaleLine());
                sw.WriteLine(FileStepLine());

                sw.WriteLine(FileReserveLine());
                sw.WriteLine(FileReserveLine());
                sw.WriteLine(FileReserveLine());
                sw.WriteLine(FileReserveLine());

                List<ushort[]> lu = InitParamsLines();

                for (int i = 0; i < lu.Count; i++)
                {
                    sw.WriteLine(FileParamLine(lu[i], i));
                }
                
            }
            catch
            {
                sw.Close();
                e.Cancel = true;
                return;
            }

            sw.Close();
            e.Result = true;

        }
       
        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnCreateComplete != null) { OnCreateComplete(!e.Cancelled); }
        }


    }
}
