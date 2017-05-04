using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UPTF_Scope
{
    public class DataRecord
    {
        public byte Address { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public int Progress { get; set; }
        public int Number { get; set; }
        public int NumberNew { get; set; }
        public DataRecord(byte address, string name)
        {
            Address = address;
            Name = name;
            Progress = 0;
            Number = 0;
            NumberNew = 0;
            Path = "";
        }
        public DataRecord(byte address): this(address,"")
        {
            
        }
    }
}
