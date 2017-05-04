using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SerialPorts;
using System.ComponentModel;

namespace ScopeLoad
{
    public class ScopeConfigLoader
    {
        BackgroundWorker backgroundWorker;
        SerialDataProvider _dataProvider;
        //AsynchSerialPort _dataProvider;
        AutoResetEvent waitResponce = new AutoResetEvent(false);
        ScopeConfig scopeConfig;

        public delegate void LoadingCompleter(bool LoadingOk);
        public event LoadingCompleter OnLoadingComplete;

        public ScopeConfig ScopeConfig
        {
            get { return scopeConfig; }
        }
        byte address;

        public ScopeConfigLoader(SerialDataProvider sdp, byte Address)
        {
            _dataProvider = sdp;
            address = Address;
            //serialPort = AsynchSerialPort;
            scopeConfig = new ScopeConfig();

            if (_dataProvider == null)
            {
                throw new Exception("Invalid serial port!");
            }

            if (!_dataProvider.IsOpen)
            {
                throw new Exception("Serial port not open!");
            }

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
            _dataProvider.GetData(address, ScopeSysType.ChannelCountAddr, 1, LoadConfigRecieveOne);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            _dataProvider.GetData(address, ScopeSysType.ScopeCountAddr, 1, LoadConfigRecieveTwo);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            _dataProvider.GetData(address, ScopeSysType.OscilFreqAddr, 1, LoadConfigRecieveThree);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            _dataProvider.GetData(address, ScopeSysType.HystoryAddr, 8, LoadConfigRecieveFour);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            _dataProvider.GetData(address, ScopeSysType.DataStartAddr, 16, LoadConfigRecieveFive);
            waitResponce.WaitOne();
            if (backgroundWorker.CancellationPending)
            {
                e.Cancel = true;
                return;
            }

            e.Result = true;
        }

        void LoadConfigRecieveOne(bool DataOk, ushort[] ParamRTU)
        {
            if (DataOk)
            {
                scopeConfig.ChannelCount = ParamRTU[0];
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
            waitResponce.Set();
        }

        void LoadConfigRecieveTwo(bool DataOk, ushort[] ParamRTU)
        {
            if (DataOk)
            {
                scopeConfig.ScopeCount = ParamRTU[0];
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
            waitResponce.Set();
        }

        void LoadConfigRecieveThree(bool DataOk, ushort[] ParamRTU)
        {
            if (DataOk)
            {
                scopeConfig.ScopeFreq = ParamRTU[0];
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
            waitResponce.Set();
        }

        void LoadConfigRecieveFour(bool DataOk, ushort[] ParamRTU)
        {
            if (DataOk)
            {
                scopeConfig.Hystory = ParamRTU[0];
                if (ParamRTU[6] != 0)
                {
                    scopeConfig.ScopeEnabled = true;
                }
                else
                {
                    scopeConfig.ScopeEnabled = false;
                }
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
            waitResponce.Set();
        }

        void LoadConfigRecieveFive(bool DataOk, ushort[] ParamRTU)
        {
            if (DataOk)
            {
                scopeConfig.InitOscillParams(ParamRTU);
            }
            else
            {
                backgroundWorker.CancelAsync();
            }
            waitResponce.Set();
        }

        public void CancelLoad()
        {
            backgroundWorker.CancelAsync();
        }

        void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (OnLoadingComplete != null) { OnLoadingComplete(!e.Cancelled); }
        }
    }
}
