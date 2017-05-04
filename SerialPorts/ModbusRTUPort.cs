using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;

namespace SerialPorts
{
    /// <summary>
    /// Отправка данных по последовательному порту по протоколу Modbus RTU
    /// </summary>
    public class ModbusRTUPort : ISerialPort, INotStandartFunc
    {
        public event EventHandler CrashSerialPort;

        public bool IsOpen
        {
            get
            {
                if (serialPort != null)
                {
                    return serialPort.IsOpen;
                }
                return false;
            }


        }
        private int Timeout;
        private SerialPort serialPort;
        public byte DefaultSlaveAddr { get; private set; }

        public ModbusRTUPort(string PortName, byte DefaultSlaveAddr, int BaudRate, Parity Parity, StopBits StopBits, int Timeout = 500)
        {
            this.DefaultSlaveAddr = DefaultSlaveAddr;

            serialPort = new SerialPort();
            serialPort.PortName = PortName;
            serialPort.BaudRate = BaudRate;
            serialPort.Parity = Parity;
            serialPort.StopBits = StopBits;
            serialPort.DataReceived +=new SerialDataReceivedEventHandler(serialPort_DataReceived);
            this.Timeout = Timeout;
        }
        public bool TryOpen()
        {
            try
            {
                serialPort.Open();
                serialPort.DiscardInBuffer();
            }
            catch
            {
                return false;
            }
            return true;
        }
        public void Close()
        {
            try
            {
                serialPort.Close();
            }
            catch
            {

            }
        }


        Action<bool, ushort[]> DataReceived;
        Action<bool> DataSent;
        Action<bool, byte[]> DataReceivedByte;
        int waitByteCount = 0;
        int waitWordCount = 1;

        public void GetData(byte SlaveAddr, ushort StartAddr, int Count, Action<bool, ushort[]> DataReceived)
        {
            byte[] txBuffer = new byte[8];
            txBuffer[0] = SlaveAddr;
            txBuffer[1] = 0x03;
            BitConverter.GetBytes(StartAddr).Reverse().ToArray().CopyTo(txBuffer, 2);
            BitConverter.GetBytes((ushort)Count).Reverse().ToArray().CopyTo(txBuffer, 4);

            byte[] txBufferCRC;
            ModbusCRC.CalcCRC(txBuffer, 6, out txBufferCRC);

            this.DataReceived = DataReceived;
            waitByteCount = 5 + 2*Count;
            waitWordCount = Count;

            Action<byte[]> a = new Action<byte[]>(SendReadCoils);
            a.BeginInvoke(txBufferCRC, null, null);
        }

        public void GetData(ushort StartAddr, int Count, Action<bool, ushort[]> DataReceived)
        {
            GetData(DefaultSlaveAddr, StartAddr, Count, DataReceived);
        }

        public void SetData(byte SlaveAddr, ushort StartAddr, Action<bool> DataSent, params ushort[] Data)
        {
            byte[] buffer = new byte[9 + Data.Length * 2];
            buffer[0] = SlaveAddr;
            buffer[1] = 0x10;
            buffer[2] = (byte)((StartAddr >> 8) & 0xFF);
            buffer[3] = (byte)(StartAddr & 0xFF);
            buffer[4] = (byte)((Data.Length >> 8) & 0xFF);
            buffer[5] = (byte)(Data.Length & 0xFF);
            buffer[6] = (byte)(2 * Data.Length);

            for (int i = 0; i < Data.Length; i++)
            {
                buffer[7 + i * 2] = (byte)((Data[i] >> 8) & 0xFF);
                buffer[8 + i * 2] = (byte)(Data[i] & 0xFF);
            }

            byte[] buffer1;
            ModbusCRC.CalcCRC(buffer, 7 + Data.Length * 2, out buffer1);

            this.DataSent = DataSent;
            waitByteCount = 8;
            Action<byte[]> a = new Action<byte[]>(SendWriteCoils);
            a.BeginInvoke(buffer1, null, null);
        }

        public void SetData(ushort StartAddr, Action<bool> DataSent, params ushort[] Data)
        {
            SetData(DefaultSlaveAddr, StartAddr, DataSent, Data);
        }

        public void Request(byte[] Buffer, byte ExpectByteCount, Action<bool, byte[]> DataReceived)
        {
            if (Buffer == null) { return; }
            if (Buffer.Length < 2) { return; }
            byte[] txBufferCRC;
            ModbusCRC.CalcCRC(Buffer, Buffer.Length, out txBufferCRC);
            waitByteCount = ExpectByteCount;
            this.DataReceivedByte = DataReceived;

            Action<byte[]> a = new Action<byte[]>(SendRequest);
            a.BeginInvoke(txBufferCRC, null, null);
        }

        EventWaitHandle waitSerialData = new AutoResetEvent(false);

        private void serialPort_DataReceived(object Sender, EventArgs e)
        {
            waitSerialData.Set();
        }

        private void SendRequest(byte[] txBuffer)
        {
            if (serialPort == null)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            if (!serialPort.IsOpen)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            try
            {
                if (serialPort.BytesToRead != 0)
                {
                    byte[] buff = new byte[serialPort.BytesToRead + 1];
                    serialPort.Read(buff, 0, serialPort.BytesToRead);
                    //serialPort.DiscardInBuffer();
                }
                //Thread.Sleep(2);
                serialPort.Write(txBuffer, 0, txBuffer.Length);
            }
            catch
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }


            if(waitSerialData.WaitOne(TimeSpan.FromMilliseconds(Timeout)))
            { 
                for (int i = 0; i < waitByteCount; i++)
                {
                    if (serialPort.BytesToRead < waitByteCount)
                    {
                        waitSerialData.WaitOne(TimeSpan.FromMilliseconds(10));
                    }
                    else break;
                }
            }

            byte[] rxBuffer;
            if (serialPort.BytesToRead >= waitByteCount)
            {
                int count = serialPort.BytesToRead;
                rxBuffer = new byte[count];

                try
                {
                    serialPort.Read(rxBuffer, 0, count);
                }
                catch
                {
                    if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                    return;
                }
            }
            else //Данные так и не пришли
            {
                if (DataReceivedByte != null)
                {
                    DataReceivedByte(false, new byte[0]);
                }
                return;
            }


            if (!ModbusCRC.CheckCRC(rxBuffer, waitByteCount))
            {
                if (DataReceivedByte != null)
                {
                    DataReceivedByte(false, new byte[0]);
                }
                return;
            }
            else
            {
                if (DataReceivedByte != null)
                {
                    DataReceivedByte(true, rxBuffer);
                }
                return;
            }
        }


        private void SendReadCoils(byte[] txBuffer)
        {
            if (serialPort == null)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            if (!serialPort.IsOpen)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            try
            {
                if (serialPort.BytesToRead != 0)
                {
                    byte[] buff = new byte[serialPort.BytesToRead + 1];
                    serialPort.Read(buff, 0, serialPort.BytesToRead);
                    //serialPort.DiscardInBuffer();
                }
                //Thread.Sleep(2);
                serialPort.Write(txBuffer, 0, txBuffer.Length);
            }
            catch
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }


            if (waitSerialData.WaitOne(TimeSpan.FromMilliseconds(Timeout)))
            {

                for (int i = 0; i < waitByteCount; i++)
                {
                    if (serialPort.BytesToRead < waitByteCount)
                    {
                        waitSerialData.WaitOne(TimeSpan.FromMilliseconds(10));
                    }
                    else break;
                }
            }

            bool dataOk = false;
            byte[] rxBuffer = new byte[0];

            if (serialPort.BytesToRead >= waitByteCount)
            {
                int count = serialPort.BytesToRead;
                rxBuffer = new byte[count];

                try
                {
                    serialPort.Read(rxBuffer, 0, count);
                }
                catch
                {
                    if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                    return;
                }
            }
            else //Данные так и не пришли
            {
                if (DataReceived !=null)
                {
                    DataReceived(false,new ushort[0]);
                }
                return;
            }

            ushort[] ubuff = new ushort[0];

            if (!ModbusCRC.CheckCRC(rxBuffer, waitByteCount))
            {
                dataOk = false;
                ubuff = new ushort[0];
            }
            else
            {
                ModbusCRC.RemoveData(rxBuffer, 3, waitWordCount, out ubuff);
                dataOk = true;
            }

            if (DataReceived != null)
            {
                DataReceived(dataOk, ubuff);
            }
        }


        private void SendWriteCoils(byte[] txBuffer)
        {
            if (serialPort == null)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            if (!serialPort.IsOpen)
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            try
            {
                if (serialPort.BytesToRead != 0)
                {
                    byte[] buff = new byte[serialPort.BytesToRead + 1];
                    serialPort.Read(buff, 0, serialPort.BytesToRead);
                    //serialPort.DiscardInBuffer();
                }
                //Thread.Sleep(2);
                serialPort.Write(txBuffer, 0, txBuffer.Length);
            }
            catch
            {
                if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                return;
            }

            if (waitSerialData.WaitOne(TimeSpan.FromMilliseconds(Timeout)))
            {

                for (int i = 0; i < waitByteCount; i++)
                {
                    if (serialPort.BytesToRead < 8)
                    {
                        waitSerialData.WaitOne(TimeSpan.FromMilliseconds(10));
                    }
                    else break;
                }
            }

            byte[] rxBuffer;

            if (serialPort.BytesToRead == 8)
            {
                rxBuffer = new byte[8];
                try
                {
                    serialPort.Read(rxBuffer, 0, 8);
                }
                catch
                {
                    if (CrashSerialPort != null) { CrashSerialPort(this, null); }
                    return;
                }
            }
            else //Данные так и не пришли
            {
                if (DataSent != null)
                {
                    DataSent(false);
                }
                return;
            }

            bool dataOk = false;
            if (!ModbusCRC.CheckCRC(rxBuffer, 8))
            {
                dataOk = false;
            }
            else
            {
                dataOk = true;
            }

            if (DataSent != null)
            {
                DataSent(dataOk);
            }
        }

    }
}
