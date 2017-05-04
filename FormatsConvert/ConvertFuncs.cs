using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatsConvert
{
    public static class ConvertFuncs
    {
        static string ToFloatStr(this double Value, int DigitCount)
        {
            return (Value.ToString("F" + DigitCount.ToString()));
        }
        public static ConvertFormats ToConvertFormats(this string Value)
        {
            switch (Value)
            {
                case "Percent": { return ConvertFormats.Percent; }
                case "Uint16": { return ConvertFormats.Uint16; }
                case "StandartFrequency": { return ConvertFormats.StandartFrequency; }
                case "I8_8": { return ConvertFormats.I8_8; }

                case "U0_16": { return ConvertFormats.U0_16; }
                case "StandartSlide": { return ConvertFormats.StandartSlide; }
                case "Digits": { return ConvertFormats.Digits; }
                case "RegulMode": { return ConvertFormats.RegulMode; }

                case "Int16": { return ConvertFormats.Int16; }
                case "AVRType": { return ConvertFormats.AVRType; }
                case "IntDiv10": { return ConvertFormats.IntDiv10; }
                case "Hex": { return ConvertFormats.Hex; }

                case "UfOldStyle": { return ConvertFormats.UfOldStyle; }
                case "FineFrequency": { return ConvertFormats.FineFrequency; }
                case "CurrentTrans": { return ConvertFormats.CurrentTrans; }
                case "TECurrentAlarm": { return ConvertFormats.TECurrentAlarm; }

                case "IntDiv8": { return ConvertFormats.IntDiv8; }
                case "UintDiv1000": { return ConvertFormats.UintDiv1000; }
                case "Freq90000": { return ConvertFormats.Freq90000; }
                case "PercentUpp": { return ConvertFormats.PercentUpp; }

                case "Freq16000": { return ConvertFormats.Freq16000; }
                case "FCFrequency": { return ConvertFormats.FCFrequency; }

                default:
                    {
                        throw new Exception("Unkonwn data format!");
                    }
            }
        }

        public static short StrToShort(string Value)
        {
            short s = 0;
            if (((Value.Contains("0x")) || (Value.Contains("0X"))) && (!Value.Contains("-")))
            {
                try
                {
                    s = Convert.ToInt16(Value, 16);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else if ((Value.Contains("-0x")) || (Value.Contains("-0X")))
            {
                try
                {
                    string str = Value.Substring(1);
                    s = Convert.ToInt16(str, 16);
                    s = (short)(-s);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else if (Value.Contains("-"))
            {
                try
                {
                    s = Convert.ToInt16(Value, 10);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else
            {
                try
                {
                    s = (short)Convert.ToUInt16(Value, 10);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }

            }
            return s;
        }
        public static ulong StrToULong(string Value)
        {
            ulong s = 0;
            if (((Value.Contains("0x")) || (Value.Contains("0X"))) && (!Value.Contains("-")))
            {
                try
                {
                    s = Convert.ToUInt64(Value, 16);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }
            }
            else
            {
                try
                {
                    s = Convert.ToUInt64(Value, 10);
                }
                catch
                {
                    throw new Exception("Invalid format");
                }

            }
            return s;
        }

        public static List<ushort> LineToUshorts(string symFileLine)
        {
            string[] paramListstr;
            char[] delimiterChars = { ' ', ',', '.', ':', '\t' };
            List<ushort> paramsList = new List<ushort>();
            if (symFileLine != null)
            {
                paramListstr = symFileLine.Split(delimiterChars);
                foreach (string str in paramListstr)
                {
                    try
                    {
                        ushort u = (ushort)StrToShort(str);
                        paramsList.Add(u);
                    }
                    catch
                    {

                    }
                }
            }
            return paramsList;
        }

        public static bool GetBit(this ushort Value, int BitNumber)
        {
            if (BitNumber > 15)
            {
                throw new Exception("Invalid bit number");
            }
            return (Value & (1 << BitNumber)) != 0;
        }
        public static bool GetBit(this byte Value, int BitNumber)
        {
            if (BitNumber > 15)
            {
                throw new Exception("Invalid bit number");
            }
            return (Value & (1 << BitNumber)) != 0;
        }

        public static ushort IntToBCD(int Value)
        {
            ushort u = (ushort)Value;
            int i = (int)(u & 0xFF);
            i = (i / 10) * 16 + (i - (i / 10) * 10);
            return (ushort)i;
        }
        public static ushort ToBCD(this int Value)
        {
            return IntToBCD(Value);
        }

        public static int BCDToInt(ushort Value)
        {
            ushort u = (ushort)(Value & 0xFF);
            int i = (int)(u & 0x000F);
            i += 10 * (int)((u >> 4) & 0x000F);
            return i;
        }

        public static int FromBCD(this ushort Value)
        {
            return BCDToInt(Value);
        }

        public static double WordToPercent(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) / 40.96;
            return d;
        }
        public static double ToPercent(this ushort Value)
        {
            return WordToPercent(Value);
        }
        public static string WordToPercentStr(ushort Value)
        {
            return WordToPercent(Value).ToFloatStr(1);
        }
        public static string WordToPercentStr(ushort Value, int DigitCount)
        {
            return WordToPercent(Value).ToFloatStr(DigitCount);
        }
        public static ushort PercentToWord(double Value)
        {
            if (Math.Abs(Value) > 799)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value * 40.96);
        }
        public static ushort PercentToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return PercentToWord(d);
        }

        public static ushort UInt16ToWord(string Value)
        {
            return (ushort)(StrToShort(Value));
        }

        public static short WordToInt16(ushort Value)
        {
            return (short)Value;
        }
        public static short ToInt16(this ushort Value)
        {
            return WordToInt16(Value);
        }
        public static string WordToInt16Str(ushort Value)
        {
            return Value.ToInt16().ToString();
        }
        public static ushort Int16ToWord(short Value)
        {
            return (ushort)Value;
        }
        public static ushort Int16ToWord(string Value)
        {
            short i = StrToShort(Value);
            return Int16ToWord(Value);
        }

        public static double WordToStandartFreq(ushort Value)
        {
            if (Value == 0) { return 0; }
            if (Value >= 0x4000) { return 0; }

            double d = 8000.0 / ((double)Value);
            return d;
        }
        public static double ToStandartFreq(this ushort Value)
        {
            return WordToStandartFreq(Value);
        }
        public static string WordToStandartFreqStr(ushort Value, int DigitCount)
        {
            return Value.ToStandartFreq().ToFloatStr(DigitCount);
        }
        public static string WordToStandartFreqStr(ushort Value)
        {
            return Value.ToStandartFreq().ToFloatStr(1);
        }
        public static ushort StandartFreqToWord(double Value)
        {
            if (Value == 0) { return 0; }
            if ((Value >= 8000) || (Value < 0))
            {
                throw new Exception("Invalid Value!");
            }

            double d = Math.Round(8000.0 / Value);
            return (ushort)d;
        }
        public static ushort StandartFreqToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return StandartFreqToWord(d);
        }

        public static double WordToI8_8(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) / 256.0;
            return d;
        }
        public static double ToI8_8(this ushort Value)
        {
            return WordToI8_8(Value);
        }
        public static string WordToI8_8Str(ushort Value, int DigitCount)
        {
            return WordToI8_8(Value).ToFloatStr(DigitCount);
        }
        public static string WordToI8_8Str(ushort Value)
        {
            return WordToI8_8(Value).ToFloatStr(1);
        }
        public static ushort I8_8ToWord(double Value)
        {
            if (Math.Abs(Value) > 255.99)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value * 256.0);
        }
        public static ushort I8_8ToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return I8_8ToWord(d);
        }

        public static double WordToU0_16(ushort Value)
        {
            double d = ((double)Value) / 65536.0;
            return d;
        }
        public static double ToU0_16(this ushort Value)
        {
            return WordToU0_16(Value);
        }
        public static string WordToU0_16Str(ushort Value, int DigitCount)
        {
            return WordToU0_16(Value).ToFloatStr(DigitCount);
        }
        public static string WordToU0_16Str(ushort Value)
        {
            return WordToU0_16(Value).ToFloatStr(3);
        }
        public static ushort U0_16ToWord(double Value)
        {
            if ((Value > 1) || (Value < 0))
            {
                throw new Exception("Invalid Value!");
            }

            if (Value > 0.99999)
            {
                return 1;
            }

            double d = Value * 65536;
            if (d > 65535) { d = 65535; }
            return (ushort)(d);
        }
        public static ushort U0_16ToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return U0_16ToWord(d);
        }

        public static double WordToStandartSlide(ushort Value)
        {
            if (Value == 0) { return 0; }
            double d = ((double)Value) / 327.68;
            return d;
        }
        public static double ToStandartSlide(this ushort Value)
        {
            return WordToStandartSlide(Value);
        }
        public static string WordToStandartSlideStr(ushort Value, int DigitCount)
        {
            return ToStandartSlide(Value).ToFloatStr(DigitCount);
        }
        public static string WordToStandartSlideStr(ushort Value)
        {
            return ToStandartSlide(Value).ToFloatStr(1);
        }
        public static ushort StandartSlideToWord(double Value)
        {

            if ((Value > 99.99) || (Value < 0))
            {
                throw new Exception("Invalid Value!");
            }
            double d = Value * 327.68;
            return (ushort)(d);
        }
        public static ushort StandartSlideToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return StandartSlideToWord(d);
        }

        public static string WordToDigits(ushort Value)
        {
            string str = "";
            for (int i = 0; i < 15; i++)
            {
                if (((i % 4) == 0) && (i != 0)) { str = " " + str; }
                if (GetBit(Value, i)) { str = "1" + str; } else { str = "0" + str; }
            }
            return str;
        }

        public static string WordToRegulMode(ushort Value, string ManString, string AutoString, string TestString)
        {
            string str;
            switch (Value)
            {
                case 0: { str = AutoString; } break;
                case 1: { str = ManString; } break;
                default: { str = TestString; } break;
            }
            return (str);
        }

        public static string WordToAVRType(ushort Value)
        {
            string str;
            switch (Value)
            {
                case 1: { str = "cosФ"; } break;
                case 2: { str = "Ire"; } break;
                case 3: { str = "If"; } break;
                default: { str = "U"; } break;
            }
            return (str);
        }

        public static double WordToIntDiv10(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) / 10.0;
            return d;
        }
        public static double ToIntDiv10(this ushort Value)
        {
            return WordToIntDiv10(Value);
        }
        public static string WordToIntDiv10Str(ushort Value, int DigitCount)
        {
            return WordToIntDiv10(Value).ToFloatStr(DigitCount);
        }
        public static string WordToIntDiv10Str(ushort Value)
        {
            return WordToIntDiv10(Value).ToFloatStr(1);
        }
        public static ushort IntDiv10ToWord(double Value)
        {
            if (Math.Abs(Value) > 3275)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value * 10.0);
        }
        public static ushort IntDiv10ToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return IntDiv10ToWord(d);
        }


        public static double WordToUfOldStyle(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) * 0.135;
            return d;
        }
        public static double ToUfOldStyle(this ushort Value)
        {
            return WordToUfOldStyle(Value);
        }
        public static string WordToUfOldStyleStr(ushort Value, int DigitCount)
        {
            return WordToUfOldStyle(Value).ToFloatStr(DigitCount);
        }
        public static string WordToUfOldStyleStr(ushort Value)
        {
            return WordToUfOldStyle(Value).ToFloatStr(1);
        }
        public static ushort UfOldStyleToWord(double Value)
        {
            if (Math.Abs(Value) > 4000)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value / 0.135);
        }
        public static ushort UfOldStyleToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return UfOldStyleToWord(d);
        }


        public static double WordToFineFrequency(ushort Value)
        {
            double d = ((float)Value) / 500.0;
            return d;
        }
        public static double ToFineFrequency(this ushort Value)
        {
            return WordToFineFrequency(Value);
        }
        public static string WordToFineFrequencyStr(ushort Value, int DigitCount)
        {
            return WordToFineFrequency(Value).ToFloatStr(DigitCount);
        }
        public static string WordToFineFrequencyStr(ushort Value)
        {
            return WordToFineFrequency(Value).ToFloatStr(2);
        }
        public static ushort FineFrequencyToWord(double Value)
        {
            if (Math.Abs(Value) > 100.0)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value * 500.0);
        }
        public static ushort FineFrequencyToWord(string Value)
        {
            double d = 0;
            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return FineFrequencyToWord(d);
        }


        public static double WordToCurrentTrans(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) * 0.009765625;
            return d;
        }
        public static double ToCurrentTrans(this ushort Value)
        {
            return WordToCurrentTrans(Value);
        }
        public static string WordToCurrentTransStr(ushort Value, int DigitCount)
        {
            return WordToCurrentTrans(Value).ToFloatStr(DigitCount);
        }
        public static string WordToCurrentTransStr(ushort Value)
        {
            return WordToCurrentTrans(Value).ToFloatStr(1);
        }
        public static ushort CurrentTransToWord(double Value)
        {
            if (Math.Abs(Value) > 20.0)
            {
                throw new Exception("Invalid Value!");
            }

            if (Math.Abs(Value) < 0.1)
            {
                throw new Exception("Invalid Value!");
            }


            return (ushort)(Value / 0.009765625);
        }
        public static ushort CurrentTransToWord(string Value)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid Value!");
            }
            return CurrentTransToWord(f);
        }


        public static double WordToTECurrentAlarm(ushort Value, bool AtomSTSFormat)
        {
            short i = (short)(Value);
            if (!AtomSTSFormat)
            {
                double d = ((double)i) * 0.00172633491500621954199424893092;
                return d;
            }
            else
            {
                double d = ((double)i) * 0.00244498777506112469437652811736;
                return d;
            }
        }
        public static double ToTECurrentAlarm(this ushort Value, bool AtomSTSFormat)
        {
            return WordToTECurrentAlarm(Value, AtomSTSFormat);
        }
        public static string WordToTECurrentAlarmStr(ushort Value, bool AtomSTSFormat, int DigitCount)
        {
            return WordToTECurrentAlarm(Value, AtomSTSFormat).ToFloatStr(DigitCount);
        }
        public static string WordToTECurrentAlarmStr(ushort Value, bool AtomSTSFormat)
        {
            return WordToTECurrentAlarm(Value, AtomSTSFormat).ToFloatStr(1);
        }
        public static ushort TECurrentAlarmToWord(double Value, bool AtomSTSFormat)
        {
            double f = Value;
            if (f > 70) { throw new Exception("Invalid Value!"); }
            if (f < 0) { throw new Exception("Invalid Value!"); }

            if (!AtomSTSFormat)
            {
                f = f / 0.00172633491500621954199424893092;
            }
            else
            {
                f = f / 0.00244498777506112469437652811736;
            }
            return ((ushort)f);
        }
        public static ushort TECurrentAlarmToWord(string Value, bool AtomSTSFormat)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid Value!");
            }
            return TECurrentAlarmToWord(f, AtomSTSFormat);
        }


        public static double WordToIntDiv8(ushort Value)
        {
            double f = (short)Value / 8.0;
            return f;
        }
        public static double ToIntDiv8(this ushort Value)
        {
            return WordToIntDiv8(Value);
        }
        public static string WordToIntDiv8Str(ushort Value, int DigitCount)
        {
            return WordToIntDiv8(Value).ToFloatStr(DigitCount);
        }
        public static string WordToIntDiv8Str(ushort Value)
        {
            return WordToIntDiv8(Value).ToFloatStr(1);
        }
        public static ushort IntDiv8ToWord(double Value)
        {
            double f = Value * 8.0;
            return (ushort)Math.Round(f);
        }
        public static double IntDiv8ToWord(string Value)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid Value!");
            }
            return IntDiv8ToWord(f);
        }


        public static double WordToUintDiv1000(ushort value)
        {
            double f = ((double)value) / 1000.0;
            return f;
        }
        public static double ToUintDiv1000(this ushort Value)
        {
            return WordToUintDiv1000(Value);
        }
        public static string WordToUintDiv1000Str(ushort Value, int DigitCount)
        {
            return WordToUintDiv1000(Value).ToFloatStr(DigitCount);
        }
        public static string WordToUintDiv1000Str(ushort Value)
        {
            return WordToUintDiv1000(Value).ToFloatStr(3);
        }
        public static ushort UintDiv1000ToWord(double Value)
        {
            if (Value < 0)
            {
                throw new Exception("Invalid value!");
            }
            if (Value > 65)
            {
                throw new Exception("Invalid value!");
            }
            double f = Value * 1000;
            return (ushort)Math.Round(f);
        }
        public static double UintDiv1000ToWord(string Value)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid value!");
            }
            return UintDiv1000ToWord(f);
        }

        public static double WordToPercent4(ushort Value)
        {
            double f = (short)Value / 10.24;
            return f;
        }
        public static double ToPercent4(this ushort Value)
        {
            return WordToPercent4(Value);
        }
        public static string WordToPercent4Str(ushort Value, int DigitCount)
        {
            return WordToPercent4(Value).ToFloatStr(DigitCount);
        }
        public static string WordToPercent4Str(ushort Value)
        {
            return WordToPercent4(Value).ToFloatStr(1);
        }
        public static ushort Percent4ToWord(double Value)
        {
            if (Value < -3100)
            {
                throw new Exception("Invalid value!");
            }
            if (Value > 3100)
            {
                throw new Exception("Invalid value!");
            }
            double f = Value * 10.24;
            return (ushort)Math.Round(f);
        }
        public static ushort Percent4ToWord(string Value)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid value!");
            }
            return Percent4ToWord(f);
        }

        public static double WordToFreq90000(ushort Value)
        {
            if (Value == 0) { throw new Exception("Invalid value!"); }
            else if (Value == 3600) { throw new Exception("Invalid value!"); }
            else
            {
                double f = 90000.0 / Value;
                return f;
            }
        }
        public static double ToFreq90000(this ushort Value)
        {
            return WordToFreq90000(Value);
        }
        public static string WordToFreq90000Str(ushort Value, int DigitCount)
        {
            return Value.ToFreq90000().ToFloatStr(DigitCount);
        }
        public static string WordToFreq90000Str(ushort Value)
        {
            return Value.ToFreq90000().ToFloatStr(3);
        }
        public static ushort Freq90000ToWord(double Value)
        {
            if (Value <= 0)
            {
                throw new Exception("Invalid value!");
            }
            if (Value > 80)
            {
                throw new Exception("Invalid value!");
            }
            double f = 90000.0 / Value;
            return (ushort)Math.Round(f);
        }
        public static ushort Freq90000ToWord(string Value)
        {
            double f = 0;
            if (!double.TryParse(Value, out f))
            {
                throw new Exception("Invalid value!");
            }
            return Freq90000ToWord(f);
        }

        public static double WordToPercentUpp(ushort Value)
        {
            short i = (short)(Value);
            double d = ((double)i) / 20.48;
            return d;
        }
        public static double ToPercentUpp(this ushort Value)
        {
            return WordToPercentUpp(Value);
        }
        public static string WordToPercentUppStr(ushort Value, int DigitCount)
        {
            return WordToPercentUpp(Value).ToFloatStr(DigitCount);
        }
        public static string WordToPercentUppStr(ushort Value)
        {
            return WordToPercentUpp(Value).ToFloatStr(1);
        }
        public static ushort PercentUppToWord(double Value)
        {
            if (Math.Abs(Value) > 1599)
            {
                throw new Exception("Invalid Value!");
            }
            return (ushort)(Value * 20.48);
        }
        public static ushort PercentUppToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return PercentUppToWord(d);
        }

        public static double WordToFreq16000(ushort Value)
        {
            if (Value == 0) { throw new Exception("Invalid value!"); }
            else if (Value == 0x4000) { throw new Exception("Invalid value!"); }
            else
            {
                double f = 16000.0 / Value;
                return f;
            }
        }
        public static double ToFreq16000(this ushort Value)
        {
            return WordToFreq16000(Value);
        }
        public static string WordToFreq16000Str(ushort Value, int DigitCount)
        {
            return WordToFreq16000(Value).ToFloatStr(DigitCount);
        }
        public static string WordToFreq16000Str(ushort Value)
        {
            return WordToFreq16000(Value).ToFloatStr(2);
        }
        public static ushort Freq16000ToWord(double Value)
        {
            if (Value <= 0)
            {
                throw new Exception("Invalid value!");
            }
            if (Value > 80)
            {
                throw new Exception("Invalid value!");
            }
            double f = 16000 / Value;
            return (ushort)Math.Round(f);
        }
        public static ushort Freq16000ToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return Freq16000ToWord(d);
        }


        public static double WordToFCFreq(ushort Value)
        {
            double f = ((double)((short)Value)) / 156.24;
            return f;
        }
        public static double ToFCFreq(this ushort Value)
        {
            return WordToFCFreq(Value);
        }
        public static string WordToFCFreqStr(ushort Value, int DigitCount)
        {
            return Value.ToFCFreq().ToFloatStr(DigitCount);
        }
        public static string WordToFCFreqStr(ushort Value)
        {
            return Value.ToFCFreq().ToFloatStr(1);
        }
        public static ushort FCFreqToWord(double Value)
        {
            if (Value < -100)
            {
                throw new Exception("Invalid value!");
            }
            if (Value > 100)
            {
                throw new Exception("Invalid value!");
            }
            double f = Value * 156.24;
            return (ushort)Math.Round(f);
        }
        public static ushort FCFreqToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return FCFreqToWord(d);
        }

        /// <summary>
        /// Функция возвращает уставку угла Фи (в системах возбуждения)
        /// </summary>
        public static double WordToRefPF(ushort Value)
        {
            double d = (short)Value;
            d = Math.Atan2(Value, 32768.0) * 180.0 / Math.PI;
            return d;
        }
        public static double ToRefPF(this ushort Value)
        {
            return WordToRefPF(Value);
        }
        public static string WordToRefPFStr(ushort Value, int DigitCount)
        {
            return Value.ToRefPF().ToFloatStr(DigitCount);
        }
        public static string WordToRefPFStr(ushort Value)
        {
            return Value.ToRefPF().ToString("F1");
        }
        public static ushort RefPFToWord(double Value)
        {
            double f = Math.Tan(Value * 3.141592 / 180.0) * 32768.0;
            return (ushort)((short)f);
        }
        public static ushort RefPFToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return RefPFToWord(d);
        }


        public static double WordToMksCycles(ushort Value)
        {
            double d = Value;
            return d * 125;
        }
        public static double ToMksCycles(this ushort Value)
        {
            return WordToMksCycles(Value);
        }
        public static string WordToMksCyclesStr(ushort Value, int DigitCount)
        {
            return Value.ToMksCycles().ToFloatStr(DigitCount);
        }
        public static string WordToMksCyclesStr(ushort Value)
        {
            return Value.ToMksCycles().ToString("F0");
        }
        public static ushort MksCyclesToWord(double Value)
        {
            double d = Math.Round(Value / 125);
            if (d<0) d = 0;
            return (ushort)d;
        }
        public static ushort MksCyclesToWord(string Value)
        {
            double d = 0;

            if (!double.TryParse(Value, out d))
            {
                throw new Exception("Invalid Value!");
            }
            return MksCyclesToWord(d);
        }

        public static double ToFormat(this ushort Value, ConvertFormats DataFormat)
        {
            switch (DataFormat)
            {
                case ConvertFormats.Percent:
                    {
                        return Value.ToPercent();
                    }

                case ConvertFormats.Uint16:
                    {

                        return Value;
                    }

                case ConvertFormats.Int16:
                    {

                        return Value.ToInt16();
                    }

                case ConvertFormats.StandartFrequency:
                    {

                        return Value.ToStandartFreq();
                    }

                case ConvertFormats.I8_8:
                    {
                        return Value.ToI8_8();
                    }

                case ConvertFormats.U0_16:
                    {
                        return Value.ToU0_16();
                    }

                case ConvertFormats.StandartSlide:
                    {
                        return Value.ToStandartSlide();
                    }

                case ConvertFormats.IntDiv10:
                    {
                        return Value.ToIntDiv10();
                    }

                case ConvertFormats.UfOldStyle:
                    {
                        return Value.ToUfOldStyle();
                    }

                case ConvertFormats.FineFrequency:
                    {
                        return Value.ToFineFrequency();
                    }


                case ConvertFormats.CurrentTrans:
                    {
                        return Value.ToCurrentTrans();
                    }


                case ConvertFormats.TECurrentAlarm:
                    {
                        return Value.ToTECurrentAlarm(false);
                    }

                case ConvertFormats.IntDiv8:
                    {
                        return Value.ToIntDiv8();
                    }


                case ConvertFormats.UintDiv1000:
                    {
                        return Value.ToUintDiv1000();
                    }

                case ConvertFormats.Percent4:
                    {
                        return Value.ToPercent4();
                    }

                case ConvertFormats.Freq90000:
                    {
                        return Value.ToFreq90000();
                    }

                case ConvertFormats.PercentUpp:
                    {
                        return Value.ToPercentUpp();
                    }

                case ConvertFormats.Freq16000:
                    {
                        return Value.ToFreq16000();
                    }

                case ConvertFormats.FCFrequency:
                    {
                        return Value.ToFCFreq();
                    }

                case ConvertFormats.RefPF:
                    {
                        return Value.ToRefPF();
                    }

                case ConvertFormats.MksCycles:
                    {
                        return Value.ToMksCycles();
                    }

                default:
                    {
                        throw new Exception("Uknown data format!");
                    }


            }

        }

        public static double ToFormat(this ushort Value, int DataFormat)
        {
            ConvertFormats cf = (ConvertFormats)DataFormat;
            return Value.ToFormat(cf);
        }

        public static string ToFormatStr(this ushort Value, ConvertFormats DataFormat)
        {
            switch (DataFormat)
            {
                case ConvertFormats.Percent:
                    {
                        return WordToPercentStr(Value);
                    }

                case ConvertFormats.Int16:
                    {

                        return WordToInt16Str(Value);
                    }

                case ConvertFormats.Uint16:
                    {

                        return Value.ToString();
                    }

                case ConvertFormats.StandartFrequency:
                    {

                        return WordToStandartFreqStr(Value);
                    }

                case ConvertFormats.I8_8:
                    {
                        return WordToI8_8Str(Value);
                    }


                case ConvertFormats.U0_16:
                    {
                        return WordToU0_16Str(Value);
                    }

                case ConvertFormats.StandartSlide:
                    {
                        return WordToStandartSlideStr(Value);
                    }

                case ConvertFormats.Digits:
                    {
                        return WordToDigits(Value);
                    }

                case ConvertFormats.RegulMode:
                    {
                        return WordToRegulMode(Value, "MAN", "AUTO", "TEST");
                    }

                case ConvertFormats.AVRType:
                    {
                        return WordToAVRType(Value);
                    }
                case ConvertFormats.IntDiv10:
                    {
                        return WordToIntDiv10Str(Value);
                    }

                case ConvertFormats.Hex:
                    {
                        return ("0x" + Value.ToString("X4"));
                    }

                case ConvertFormats.UfOldStyle:
                    {
                        return WordToUfOldStyleStr(Value);
                    }

                case ConvertFormats.FineFrequency:
                    {
                        return WordToFineFrequencyStr(Value);
                    }

                case ConvertFormats.CurrentTrans:
                    {
                        return WordToCurrentTransStr(Value);
                    }
                case ConvertFormats.TECurrentAlarm:
                    {
                        return WordToTECurrentAlarmStr(Value, false);
                    }

                case ConvertFormats.IntDiv8:
                    {
                        return WordToIntDiv8Str(Value);
                    }

                case ConvertFormats.UintDiv1000:
                    {
                        return WordToUintDiv1000Str(Value);
                    }

                case ConvertFormats.Percent4:
                    {
                        return WordToPercent4Str(Value);
                    }
                case ConvertFormats.Freq90000:
                    {
                        return WordToFreq90000Str(Value);
                    }

                case ConvertFormats.PercentUpp:
                    {
                        return WordToPercentUppStr(Value);
                    }

                case ConvertFormats.Freq16000:
                    {
                        return WordToFreq16000Str(Value);
                    }
                case ConvertFormats.FCFrequency:
                    {
                        return WordToFCFreqStr(Value);
                    }

                case ConvertFormats.RefPF:
                    {
                        return WordToRefPFStr(Value);



                    }
                case ConvertFormats.MksCycles:
                    {
                        return WordToMksCyclesStr(Value);
                    }

                default:
                    {
                        throw new Exception("Uknown data format!");
                    }
            }

        }

        public static ushort FromFormat(this double Value, ConvertFormats DataFormat)
        {
            switch (DataFormat)
            {
                case ConvertFormats.Percent:
                    {
                        return PercentToWord(Value);
                    }

                case ConvertFormats.Uint16:
                    {

                        return (ushort)Value;
                    }

                case ConvertFormats.Int16:
                    {

                        return Int16ToWord((short)Value);
                    }

                case ConvertFormats.StandartFrequency:
                    {

                        return StandartFreqToWord(Value);
                    }

                case ConvertFormats.I8_8:
                    {
                        return I8_8ToWord(Value);
                    }

                case ConvertFormats.U0_16:
                    {
                        return U0_16ToWord(Value);
                    }

                case ConvertFormats.StandartSlide:
                    {
                        return StandartSlideToWord(Value);
                    }

                case ConvertFormats.IntDiv10:
                    {
                        return IntDiv10ToWord(Value);
                    }

                case ConvertFormats.UfOldStyle:
                    {
                        return UfOldStyleToWord(Value);
                    }

                case ConvertFormats.FineFrequency:
                    {
                        return FineFrequencyToWord(Value);
                    }


                case ConvertFormats.CurrentTrans:
                    {
                        return CurrentTransToWord(Value);
                    }


                case ConvertFormats.TECurrentAlarm:
                    {
                        return TECurrentAlarmToWord(Value, false);
                    }

                case ConvertFormats.IntDiv8:
                    {
                        return IntDiv8ToWord(Value);
                    }


                case ConvertFormats.UintDiv1000:
                    {
                        return UintDiv1000ToWord(Value);
                    }

                case ConvertFormats.Percent4:
                    {
                        return Percent4ToWord(Value);
                    }

                case ConvertFormats.Freq90000:
                    {
                        return Freq90000ToWord(Value);
                    }

                case ConvertFormats.PercentUpp:
                    {
                        return PercentUppToWord(Value);
                    }

                case ConvertFormats.Freq16000:
                    {
                        return Freq16000ToWord(Value);
                    }

                case ConvertFormats.FCFrequency:
                    {
                        return FCFreqToWord(Value);
                    }

                case ConvertFormats.RefPF:
                    {
                        return RefPFToWord(Value);
                    }

                case ConvertFormats.MksCycles:
                    {
                        return MksCyclesToWord(Value);
                    }

                default:
                    {
                        throw new Exception("Uknown data format!");
                    }

            }
        }

    }
}
