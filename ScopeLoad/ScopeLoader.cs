using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialPorts;
using System.ComponentModel;
using System.Drawing;
using System.Threading;


namespace ScopeLoad
{
    public class ScopeLoader
    {
        public delegate void ProcessUpdater(int NowCompleted);
        public event ProcessUpdater OnProcessUpdate;

        public delegate void LoadingCompleter(bool LoadingOk);
        public event LoadingCompleter OnLoadingComplete;

        BackgroundWorker backgroundWorker;
        SerialDataProvider _dataProvider;
        AutoResetEvent waitResponce = new AutoResetEvent(false);
        ScopeConfig scopeConfig;
        
        byte address;

        int loadOscNum = 0;

        UInt16 loadOscilIndex = 0x0000;
        UInt16 loadOscilStart = 0x0000;
        UInt16 loadOscilCountRound = 0x0000;


        List<ushort[]> downloadedData = new List<ushort[]>();

        public  List<ushort[]> DownloadedData
        {
            get { return downloadedData; }
        }

        public ScopeLoader(SerialDataProvider sdp, byte Address, ScopeConfig ScopeConfig, int LoadOscNum)
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

            loadOscNum = LoadOscNum;
            scopeConfig = ScopeConfig;
            StartLoading();
        }

        UInt32 CalcMemoryAddr(int nowLoadOscNum)
        {
            UInt16 u;
            u = (UInt16)(loadOscilIndex + loadOscilStart - scopeConfig.Hystory);
            UInt32 m;
            m = (UInt32)(524288 * nowLoadOscNum + 8 * u);
            m = (UInt32)(Math.Round((double)m / scopeConfig.ScopeCount));
            return m;
        }

        void StartLoading()
        {
            loadOscilIndex = 0;
            downloadedData = new List<ushort[]>();
            loadOscilCountRound = (UInt16)(scopeConfig.ScopeCount * 8);


            backgroundWorker = new BackgroundWorker();
            backgroundWorker.WorkerReportsProgress = true;
            backgroundWorker.WorkerSupportsCancellation = true;
            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;
            backgroundWorker.RunWorkerAsync(null);
        }

        void SendSecondRequest()
        {
            ushort[] writeArr = new ushort[6];
            UInt32 m;
            writeArr[0] = 0x0001;
            writeArr[1] = ScopeSysType.ParamLoadDataAddr;
            m = CalcMemoryAddr(loadOscNum);
            writeArr[2] = (ushort)(m & 0x3FFF);
            m = m & 0x003FC000;
            m = m >> 6;
            m = m & 0xFFFF;
            m = m | 0x8001;
            writeArr[3] = (ushort)m;
            writeArr[4] = 32;
            writeArr[5] = 1;
            _dataProvider.SetData(address, ScopeSysType.ParamLoadConfigAddr, EndLoadStepTwo, writeArr);
        }

        void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                //************************************************************************************************************************************//
                _dataProvider.GetData(address, (ushort)(ScopeSysType.LoadOscilStartAddr + loadOscNum), 1, EndLoadStepOne);
                waitResponce.WaitOne();
                if (backgroundWorker.CancellationPending)
                {
                    e.Cancel = true;
                    return;
                }

                if (loadOscilIndex == 0)
                {
                    backgroundWorker.ReportProgress(65536);
                    break;
                }
                else
                {
                    backgroundWorker.ReportProgress(loadOscilIndex);
                }
            }

            e.Result = true;
        }

        void EndLoadStepOne(bool DataOk, ushort[] ParamRTU)
        {
            if (!DataOk)
            {
                backgroundWorker.CancelAsync();
                waitResponce.Set();
                return;
            }

            loadOscilStart = ParamRTU[0];
            SendSecondRequest();
        }

        void EndLoadStepTwo(bool DataOk)
        {
            if (!DataOk)
            {
                backgroundWorker.CancelAsync();
                waitResponce.Set();
                return;
            }
            _dataProvider.GetData(address, ScopeSysType.ParamLoadDataAddr, 32, EndLoadStepThree);
        }

        void EndLoadStepThree(bool DataOk, ushort[] ParamRTU)
        {
            if (!DataOk)
            {
                backgroundWorker.CancelAsync();
                waitResponce.Set();
                return;
            }

            downloadedData.Add(new ushort[32]);
            ParamRTU.CopyTo(downloadedData[downloadedData.Count - 1], 0);
            loadOscilIndex = (UInt16)(loadOscilIndex + loadOscilCountRound);
            waitResponce.Set();

        }

        void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (OnProcessUpdate != null) { OnProcessUpdate((int)e.ProgressPercentage); }
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnLoadingComplete != null) { OnLoadingComplete(!e.Cancelled); }
        }

        public void CancelLoad()
        {
            backgroundWorker.CancelAsync();
        }
    }
}
