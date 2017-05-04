using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScopeLoad
{
    public class ScopeConfig
    {        
        //Было сделано изменение конфигурации
        public bool ChangeScopeConfig = false;

        //Скаченные параметры
        ushort[] loadParams = new ushort[32];
        public ushort[] LoadParams { get { return loadParams; } set { } }
        public void SetLoadParamsBlock(ushort[] newPartLoadParams, int startIndex, int paramCount)
        {
            int i;
            try
            {
                for (i = 0; i < paramCount; i++)
                {
                    loadParams[startIndex + i] = newPartLoadParams[i];
                }
            }
            catch { }
        }

        //Текущая предыстория
        ushort hystory = 0x8000;
        public ushort Hystory
        {
            get { return hystory; }
            set
            {
                switch (value)
                {
                    case 0xC000:
                    case 0x4000: { hystory = value; } break;
                    default: { hystory = 0x8000; } break;
                }
            }

        }

        //Количество осциллограмм 
        ushort scopeCount = 1;
        public ushort ScopeCount
        {
            get { return scopeCount; }
            set
            {
                switch (value)
                {
                    case 2:
                    case 4:
                    case 8:
                    case 16:
                    case 32:
                        {
                            scopeCount = value;
                        }
                        break;

                    default:
                        {
                            scopeCount = 1;
                        }
                        break;
                }
            }
        }

        //Количество каналов
        ushort channelCount = 1;
        public ushort ChannelCount
        {
            get { return channelCount; }
            set
            {
                switch (value)
                {
                    case 8:
                    case 16:
                        {
                            channelCount = value;
                        }
                        break;

                    default:
                        {
                            channelCount = 4;
                        }
                        break;
                }
            }
        }

        //Дискретность осциллографа
        ushort scopeFreq = 1;
        public ushort ScopeFreq
        {
            get { return scopeFreq; }
            set
            {
                switch (value)
                {
                    case 3:
                    case 7:
                        {
                            scopeFreq = value;
                        }
                        break;

                    default:
                        {
                            scopeFreq = 1;
                        }
                        break;
                }
            }

        }

        //Осциллограф включен
        public bool ScopeEnabled = true;

        //Осциллографирумые параметры
        List<int> oscillParams = new List<int>();
        public List<int> OscillParams
        {
            get { return oscillParams; }
            set
            {

            }
        }

        public int FindParamIndex(ushort paramAddr)
        {
            int i = 0;
            while ((i < ScopeSysType.ChannelAddrs.Count) && (paramAddr != ScopeSysType.ChannelAddrs[i]))
            {
                i++;
            }


            return i;
        }

        public void InitOscillParams(ushort[] loadParams)
        {
            int i;
            oscillParams.Clear();
            for (i = 0; i < channelCount; i++)
            {
                oscillParams.Add(FindParamIndex(loadParams[i]));
            }
        }

    }
}
