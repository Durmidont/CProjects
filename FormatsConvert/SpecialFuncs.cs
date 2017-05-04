using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatsConvert
{
    public static class SpecialFuncs
    {
        public static double ToAbsolute(this ushort Value, double Nominal)
        {
            double d = Value.ToPercent();
            return d * Nominal / 100.0;
        }

        public static double CalcPowerKWt(ushort NomU, ushort NomI, ushort P)
        {
            double S = Math.Sqrt(3) * NomU * NomI / 1000.0;
            double Pf = (int)P;
            Pf = ((short)P) / 4096.0;
            return (S * Pf);
        }

        public static double CalcPowerMWt(ushort NomU, ushort NomI, ushort P)
        {
            double S = Math.Sqrt(3) * NomU * NomI / 1000000.0;
            double Pf = (int)P;
            Pf = ((short)P) / 4096.0;
            return (S * Pf);
        }

        public static double CalcAngleFiCTC(ushort P, ushort Q)
        {
            double Pf = (short)P;
            double Qf = (short)Q;

            double d;
            d = Math.Atan2(Qf, Pf);

            return (180.0 * d / Math.PI);
        }

        public static string CalcPowerFactorCTC(ushort P, ushort Q, bool Absolute)
        {
            double Pf = (short)P;
            double Qf = (short)Q;

            double d;
            d = Math.Atan2(Qf, Pf);

            if (!Absolute) { return ((180.0 * d / Math.PI).ToString("F0")); }

            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;
        }

        public static string CalcPowerFactorEXSR(ushort P, ushort Q, bool Absolute)
        {
            double Pf = (short)P;
            double Qf = (short)Q;

            Pf = -Pf;

            double d;
            d = Math.Atan2(Qf, Pf);

            if (!Absolute) { return ((180.0 * d / Math.PI).ToString("F0")); }

            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;

        }

        public static double CalcNomIe(ushort KIe, ushort NomIe)
        {

            double f = (double)((short)NomIe) * 512.0 / (double)((short)KIe);
            return f / 10.0;
        }

        public static ushort CalcKIe(double Value, ushort NomIe)
        {
            double d = (double)NomIe;
            d = Math.Round(d * 512.0 / Value);
            return (ushort)((short)d);
        }

        /// <summary>
        /// Возвращает вторичный ток трансформатора тока
        /// </summary>
        public static double ToSecondCurrent(this ushort Ktt)
        {
            if (Ktt == 0) { return 2.5; }
            double f = (double)Ktt;
            f = 512.0 * 5.0 / f;
            return f;
        }

        /// <summary>
        /// Возвращает вторичный ток трансформатора тока
        /// </summary>
        public static ushort ToSecondCurrent(this double Value)
        {
            if (Value == 0) {return 0x0400;}
            double d = Math.Round(512 * 5.0 / Value);
            short s = (short)d;
            return (ushort)d;
        }

        public static double ToOverHeatTi(this ushort Ti)
        {
            if (Ti == 0) { return 1; }
            double d = Ti;
            d = Math.Round(67109.0 / d);
            return d;
        }
        public static ushort ToOverHeatTi(this double Ti)
        {
            double d = Math.Round(67109.0 / Ti);
            return (ushort)d;
        }

        public static double ToRegulatorTi(this ushort Ti)
        {
            if (Ti == 0) { return 0; }
            double d = Ti;
            d = Math.Round(65535.0 / d);
            return d;
        }
        public static ushort ToRegulatorTi(this double Ti)
        {
            if (Ti == 0) { return 0; }
            double d = Math.Round(65535.0 / Ti);
            return (ushort)d;
        }

        public static double ToVHzRef(this ushort[] Value)
        {
            if (Value.Length != 2) { return 0; }
            double d = Value[0] + Value[1].ToU0_16();
            d = Math.Round(80000.0 / d) / 10.0;
            return d;
        }
        public static ushort[] ToVHzRef(this double Value)
        {
            ushort[] us = new ushort[2];
            double d = 8000.0 / Value;
            double d1 = Math.Truncate(d);
            ulong u = (ulong)d;

            us[0] = (ushort)d;
            us[1] = ConvertFuncs.U0_16ToWord(d - d1);
            return us;
        }


        public static double WordToSlowRegSpeed(ushort Value)
        {
            double d = (short)(Value + 1);
            d = 1000 / (d * 40.96);
            return d;
        }

        public static ushort SlowRegSpeedToWord(double Value)
        {
            if (Value == 0) { return 0; }
            if (Value < 0.1) { Value = 0.1; }
            if (Value > 1) { Value = 1; }
            double d = Math.Round(1000.0 / (Value * 40.96));
            return (ushort)d;
        }


        public static double WordToKVTrans(ushort Value)
        {
            double d = Value;
            d = 16384.0 * 105.0 / Value;
            return d;
        }

        public static ushort KVTransToWord(double Value)
        {
            double d = Value;
            d = 16384.0 * 105.0 / Value;
            return (ushort)(d + 0.5);

        }


        public static ushort CalcTermoInt(ushort termoV, int TermoFormat)
        {
            ushort[] termoVolt = { 1747, 2061, 2421, 2599, 2781, 3110, 3390, 3613, 3702, 3781, 3846, 3901, 3949, 3988, 4022, 4052 };
            ushort[] temps = { 0, 10, 20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 85, 90, 95, 100 };
            double tlo, thi;
            double tvlo, tvhi;
            double tgrad;
            ushort termoIndex;
            ushort TermoV;

            TermoV = termoV;

            if (TermoFormat != 0) { TermoV = (ushort)(TermoV & 0xFF); TermoV = (ushort)(TermoV << 4); }

            if (TermoV <= 1747) { return (0); }
            if (TermoV >= 4052) { return (100); }

            termoIndex = 1;

            while (termoVolt[termoIndex] < TermoV)
            {
                termoIndex++;
            }

            thi = (double)(temps[termoIndex]); tlo = (double)(temps[termoIndex - 1]);
            tvhi = (double)(termoVolt[termoIndex]); tvlo = (double)(termoVolt[termoIndex - 1]);

            tgrad = TermoV * (thi - tlo) - thi * tvlo + tlo * tvhi;
            tgrad = tgrad / (tvhi - tvlo);
            return (((ushort)(tgrad + 0.5)));
        }

        public static string CalcTermo(ushort termoV, int TermoFormat)
        {
            ushort t = CalcTermoInt(termoV, TermoFormat);
            return (t.ToString());
        }

        public static ushort TermoToHex(double Temp, int TermoFormat)
        {
            ushort[] TermoVolt = { 1747, 2061, 2421, 2599, 2781, 3110, 3390, 3613, 3702, 3781, 3846, 3901, 3949, 3988, 4022, 4052 };


            ushort[] Temps = { 0, 10, 20, 25, 30, 40, 50, 60, 65, 70, 75, 80, 85, 90, 95, 100 };

            double tlo, thi;
            double tvlo, tvhi;

            double TermoV;


            ushort TermoIndex;
            ushort TermoV1;

            if (Temp <= 10) { if (TermoFormat != 0) { return (2061 >> 4); } else { return 2061; } }
            if (Temp >= 100) { if (TermoFormat != 0) { return (4052 >> 4); } else { return 4052; } }

            TermoIndex = 1;

            while (Temps[TermoIndex] < Temp)
            {
                TermoIndex++;
            }


            thi = (double)(Temps[TermoIndex]); tlo = (double)(Temps[TermoIndex - 1]);
            tvhi = (double)(TermoVolt[TermoIndex]); tvlo = (double)(TermoVolt[TermoIndex - 1]);



            TermoV = Temp * (tvhi - tvlo) - tvhi * tlo + tvlo * thi;
            TermoV = TermoV / (thi - tlo);

            TermoV1 = (ushort)(TermoV + 0.5);

            if (TermoFormat != 0) { TermoV1 = (ushort)((TermoV1 + 0x08) >> 4); }

            return (TermoV1);
        }


        public static string HexToFCVoltage(ushort value, ushort cellsCount, ushort digitCount)
        {
            if (digitCount > 5) { digitCount = 5; }
            double f;
            if (cellsCount == 8) { f = (short)value * 2.44140625; }
            else { f = (short)value * 1.46484375; }
            return (f.ToString("F" + digitCount.ToString()));
        }

        public static string HVFCCalcCosFi(ushort P, ushort Q)
        {
            double Pf = (int)P;
            double Qf = (int)Q;

            double d;
            d = Math.Atan2(Qf, Pf);
            d = Math.Cos(d);
            string st = d.ToString("F2");
            if (Qf < 0) { st = st + " C"; } else { st = st + " L"; }

            return st;
        }


        public static string HexToFCVoltageZero(ushort value)
        {
            double f = (short)value / 39.06;
            return (f.ToString("F1"));
        }
    }


}
