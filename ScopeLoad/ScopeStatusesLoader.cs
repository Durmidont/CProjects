using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SerialPorts;
using System.ComponentModel;
using System.Drawing;

namespace ScopeLoad
{
    public class ScopeStatusesLoader
    {
        BackgroundWorker backgroundWorker;
        SerialDataProvider _dataProvider;
        AutoResetEvent waitResponce = new AutoResetEvent(false);
        ScopeConfig scopeConfig;

        public delegate void LoadingCompleter(bool LoadingOk);
        public event LoadingCompleter OnLoadingComplete;

        int loadTimeStampStep = 0;

        ushort[] oscilsStatus = new ushort[33];

        public ushort[] OscilsStatus
        {
            get { return oscilsStatus; }
        }
        string[] oscilTitls = new string[33];

        public string[] OscilTitls
        {
            get { return oscilTitls; }
        }
        string[] oscTimeDates = new string[33];

        public string[] OscTimeDates
        {
            get { return oscTimeDates; }
        }

      

        string oscillString = "Осциллограмма";
       
        byte address;

        public ScopeStatusesLoader(SerialDataProvider sdp, byte Address, ScopeConfig ScopeConfig)
        {
            _dataProvider = sdp;
            address = Address;

            if (_dataProvider == null)
            {
                throw new Exception("Invalid serial port!");
            }

            if (!_dataProvider.IsOpen)
            {
                throw new Exception("Serial port not open!");
            }

            scopeConfig = ScopeConfig;
            StartLoading();
        }

        public void StartLoading()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            //backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(null);
        }


        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            _dataProvider.GetData(address, ScopeSysType.OscilStatusAddr, 32, EndLoadStatuses);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            for (int i = 0; i < scopeConfig.ScopeCount; i++)
            {
                if (oscilsStatus[i] >= 4)
                {
                    loadTimeStampStep = i;
                    _dataProvider.GetData(address, (ushort)(ScopeSysType.TimeStampAddr + i * 8), 8, UpdateTimeStamp);
                    waitResponce.WaitOne();
                    if (backgroundWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            e.Result = true;
        }

        void UpdateTimeStamp(bool DataOk, ushort[] ParamRTU)
        {
            if (!DataOk)
            {
                backgroundWorker.CancelAsync();
                waitResponce.Set();
                return;
            }

            string str, str2;
            str = (ParamRTU[5] & 0x3F).ToString("X2") + "/" + (ParamRTU[6] & 0x1F).ToString("X2") + "/20" + (ParamRTU[7] & 0xFF).ToString("X2");
            str2 = (ParamRTU[3] & 0x3F).ToString("X2") + ":" + (ParamRTU[2] & 0x7F).ToString("X2") + ":" + (ParamRTU[1] & 0x7F).ToString("X2") + "." + ParamRTU[4].ToString("D3");

            oscilTitls[loadTimeStampStep] = oscillString + " N" + (loadTimeStampStep + 1).ToString() + " " + str + " " + str2;
            oscTimeDates[loadTimeStampStep] = str + " " + str2;
            waitResponce.Set();
        }

        void EndLoadStatuses(bool DataOk, ushort[] ParamRTU)
        {
            if (!DataOk)
            {
                backgroundWorker.CancelAsync();
                waitResponce.Set();
                return;
            }

            oscilsStatus = ParamRTU;
            
            waitResponce.Set();
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnLoadingComplete != null) { OnLoadingComplete(!e.Cancelled); }
        }
    }
}
