using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerialPorts
{
    public interface INotStandartFunc
    {
        void Request(byte[] buffer, byte ExpectByteCount, Action<bool, byte[]> DataReceived);
    }
}
