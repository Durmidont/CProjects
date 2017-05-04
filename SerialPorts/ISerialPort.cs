using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPorts
{
    public interface ISerialPort
    {
        bool TryOpen();
        void Close();

        void GetData(ushort StartAddr, int Count, Action<bool, ushort[]> OnDataReceived);
        void GetData(byte   SlaveAddr, ushort StartAddr, int Count, Action<bool, ushort[]> OnDataReceived);
        void SetData(ushort StartAddr, Action<bool> OnDataSent, params ushort[] Data);
        void SetData(byte SlaveAddr, ushort StartAddr, Action<bool> OnDataSent, params ushort[] Data);

        event EventHandler CrashSerialPort;
        bool IsOpen { get; }
    }
}
