using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;


namespace FormatsConvert
{
    [StructLayout(LayoutKind.Explicit)]
    public struct FloatStruct
    {
        [FieldOffset(0)]
        public ushort Word1;
        [FieldOffset(2)]
        public ushort Word2;
        [FieldOffset(0)]
        public float Float;
        [FieldOffset(0)]
        public ulong ULong;
    }
}
