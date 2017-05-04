using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPorts
{
    /// <summary>
    /// Данный класс занимается приемом запросов от клиентов,
    /// постановкой их в очередь, отправкой в ISerialPort, 
    /// получение ответа, отправка ответа клиенту
    /// </summary>
    public class SerialDataProvider
    {
        /// <summary>
        /// Возникает при ошибке работы с портом
        /// </summary>
        public event EventHandler CrashSerialPort;

        //Используется для синхронизация чтения-записи в очередь из разных потоков
        private object locker = new object();

        private bool portBusy = false;

        //Очередь запросов
        Queue<RequestUnit> Requests = new Queue<RequestUnit>();

        //Текущий запрос
        RequestUnit currentRequest;

        private ISerialPort serialPort;



        //**********************КОНСТРУКТОРЫ КЛАССА****************************************************************//
        //*********************************************************************************************************//
        //*********************************************************************************************************//
        public SerialDataProvider(ISerialPort SerialPort)
        {
            serialPort = SerialPort;
            serialPort.CrashSerialPort += new EventHandler(serialPort_CrashSerialPort);
        }
        public SerialDataProvider(string PortName, byte SlaveAddr, int BaudRate, System.IO.Ports.Parity Parity, System.IO.Ports.StopBits StopBits)
        {
            serialPort = new ModbusRTUPort(PortName, SlaveAddr, BaudRate, Parity, StopBits);
            serialPort.CrashSerialPort += new EventHandler(serialPort_CrashSerialPort);
        }

        //**********************СОБЫТИЯ***********(****************************************************************//
        //*********************************************************************************************************//
        //*********************************************************************************************************//
        private void serialPort_CrashSerialPort(object sender, EventArgs e)
        {
            if (CrashSerialPort != null)
            {
                CrashSerialPort(sender, e);
            }
        }

        /// <summary>
        /// SerialDataProvider готов к работе
        /// </summary>
        public bool IsOpen
        {
            get
            {
                if (serialPort == null) { return false; }
                return serialPort.IsOpen;
            }
        }


        //**********************ОТКРЫТИЕ ЗАКРЫТИЕ ПОРТА************************************************************//
        //*********************************************************************************************************//
        //*********************************************************************************************************//
        public bool TryOpen()
        {
            if (serialPort == null) { return false; }
            return serialPort.TryOpen();
        }
        public void Close()
        {
            if (serialPort != null) { serialPort.Close(); }
        }

        /// <summary>
        /// Чтение данных от заданного Slave
        /// </summary>
        /// <param name="SlaveAddr"></param>
        /// <param name="StartAddr"></param>
        /// <param name="Count"></param>
        /// <param name="DataReceived">Делегат вызывается после получения данных от Slave</param>
        public void GetData(byte SlaveAddr, ushort StartAddr, int Count, Action<bool, ushort[]> DataReceived)
        {
            if (serialPort == null) { return; }
            if (!serialPort.IsOpen) { return; }

            RequestUnit reqUnit = new RequestUnit(SlaveAddr,StartAddr,Count,DataReceived);
            lock (locker)
            {
                if (portBusy) { Requests.Enqueue(reqUnit); }
                else SendRequest(reqUnit);
            }
        }

        /// <summary>
        /// Запись данных в указанный Slave
        /// </summary>
        /// <param name="SlaveAddr"></param>
        /// <param name="StartAddr"></param>
        /// <param name="DataSent"></param>
        /// <param name="Data"></param>
        public void SetData(byte SlaveAddr, ushort StartAddr, Action<bool> DataSent, params ushort[] Data)
        {
            if (serialPort == null) { return; }
            if (!serialPort.IsOpen) { return; }

            RequestUnit reqUnit = new RequestUnit(SlaveAddr, StartAddr, DataSent, Data);
            lock (locker)
            {
                if (portBusy) { Requests.Enqueue(reqUnit); }
                else SendRequest(reqUnit);
            }
        }

        /// <summary>
        /// Отправка нестандартного запроса в Slave
        /// </summary>
        /// <param name="SlaveAddr"></param>
        /// <param name="Buffer">Отправляемые данные</param>
        /// <param name="ExceptByteCount">ожидаемое количество байтов в ответе</param>
        /// <param name="DataReceived">делегат, вызываемый после получения ответа от Slave</param>
        public void Request(byte SlaveAddr, byte[] Buffer,byte ExpectedByteCount, Action<bool, byte[]> DataReceived)
        {
            if (serialPort == null) { return; }
            if (!serialPort.IsOpen) { return; }
            
            if (!(serialPort is INotStandartFunc))
            {
                throw new Exception("Invalid request for current instance of ISerialPort");
            }

            int l = 0;
            if (Buffer != null) { l = Buffer.Length; }

            byte[] buff = new byte[l + 1];
            buff[0] = SlaveAddr;
            Buffer.CopyTo(buff, 1);

            RequestUnit reqUnit = new RequestUnit(buff, ExpectedByteCount, DataReceived);
            lock (locker)
            {
                if (portBusy) { Requests.Enqueue(reqUnit); }
                else SendRequest(reqUnit);
            }

        }

        private void SendRequest(RequestUnit RequestUnit)
        {
            portBusy = true;
            if (RequestUnit.RequestType == RequestType.GetData)
            {
                serialPort.GetData(RequestUnit.SlaveAddr, RequestUnit.StartAddr, RequestUnit.Count, DataReceived);
                currentRequest = RequestUnit;
            }
            else if (RequestUnit.RequestType == RequestType.SetData)
            {
                serialPort.SetData(RequestUnit.SlaveAddr, RequestUnit.StartAddr, DataSent, RequestUnit.DataToSent);
                currentRequest = RequestUnit;
            }
            else if (RequestUnit.RequestType == RequestType.NotStandartFunc)
            {
                (serialPort as INotStandartFunc).Request(RequestUnit.TxBuffer, RequestUnit.ExpectByteCount, DataReceivedByte);
                currentRequest = RequestUnit;
            }
        }

        private void DataReceived(bool DataOk, ushort[] Data)
        {
            if (currentRequest.DataReceived != null)
            {
                currentRequest.DataReceived(DataOk, Data);
            }

            lock (locker)
            {
                if (Requests.Count != 0)
                {
                    SendRequest(Requests.Dequeue());
                }
                else
                {
                    portBusy = false;
                }
            }
        }

        private void DataSent(bool DataOk)
        {
            if (currentRequest.DataSent != null)
            {
                currentRequest.DataSent(DataOk);
            }

            lock (locker)
            {
                if (Requests.Count != 0)
                {
                    SendRequest(Requests.Dequeue());
                }
                else
                {
                    portBusy = false;
                }
            }
        }

        private void DataReceivedByte(bool DataOk, byte[] Data)
        {
            if (currentRequest.DataReceivedByte != null)
            {
                currentRequest.DataReceivedByte(DataOk, Data);
            }

            lock (locker)
            {
                if (Requests.Count != 0)
                {
                    SendRequest(Requests.Dequeue());
                }
                else
                {
                    portBusy = false;
                }
            }

        }
    }

    public class RequestUnit
    {
        public RequestType RequestType { get; private set; }
        public Action<bool, ushort[]> DataReceived { get; private set; }
        public Action<bool> DataSent { get; private set; }
        public Action<bool, byte[]> DataReceivedByte { get; private set; }
        public ushort StartAddr { get; private set; }
        public int Count { get; private set; }
        public ushort[] DataToSent { get; private set; }
        public byte SlaveAddr {get;private set;}
        public byte ExpectByteCount { get; private set; }
        public byte[] TxBuffer { get; private set; }

        public RequestUnit(byte SlaveAddr, ushort StartAddr,int Count, Action<bool, ushort[]> DataReceived)
        {
            RequestType = SerialPorts.RequestType.GetData;
            this.StartAddr = StartAddr;
            this.SlaveAddr = SlaveAddr;
            this.Count = Count;
            this.DataReceived = DataReceived;
        }

        public RequestUnit(byte SlaveAddr, ushort StartAddr, Action<bool> DataSent, params ushort[] Data)
        {
            RequestType = SerialPorts.RequestType.SetData;
            this.StartAddr = StartAddr;
            this.SlaveAddr = SlaveAddr;
            DataToSent = Data;
            this.DataSent = DataSent;
        }

        public RequestUnit(byte[] TxBuffer, byte ExpectByteCount, Action<bool, byte[]> DataReceived)
        {
            RequestType = SerialPorts.RequestType.NotStandartFunc;
            this.ExpectByteCount = ExpectByteCount;
            DataReceivedByte = DataReceived;
            this.TxBuffer = TxBuffer; 
        }
    }

    public enum RequestType : int
    {
        GetData = 0,
        SetData = 1,
        NotStandartFunc = 2
    }
}
