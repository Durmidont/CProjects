using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace TextLibrary
{
    public sealed class AppTexts
    {
        private static readonly Object s_lock = new Object();
        private static AppTexts instance = null;

        private AppTexts()
        {
            elementStrings = new ElementsStrings("strings.xml");
        }

        public ElementsStrings elementStrings;
        public int Language { get; set; }
        public string ParameterName(int ParamNum)
        {
            try
            {
                return (elementStrings.Parameters[ParamNum].ParamNames[Language]);
            }
            catch
            {
                return ("Error");
            }
        }

        public string ParameterNameUpper(int ParamNum)
        {
            try
            {
                string str = elementStrings.Parameters[ParamNum].ParamNames[Language];
                str = str.ToUpper();
                return (str);
            }
            catch
            {
                return ("Error");
            }
        }
        public string ParameterNameMulty(int ParamNum)
        {
            string str;
            try
            {
                str = elementStrings.Parameters[ParamNum].ParamNames[Language];
            }
            catch
            {
                return ("Error");
            }
            string[] paramListstr;
            char[] delimiterChars = { ' ' };
            paramListstr = str.Split(delimiterChars);
            str = "";
            for (int i = 0; i < paramListstr.Length; i++)
            {
                str = str + paramListstr[i];
                if (i != (paramListstr.Length - 1)) { str = str + "\n"; }
            }
            return str;
        }
        public List<string> GetListParamNames(int StartIndex, int EndIndex)
        {
            List<string> strs = new List<string>();
            for (int i = StartIndex; i <= EndIndex; i++)
            {
                strs.Add(ParameterName(i));
            }
            return strs;
        }

        public static AppTexts Instance
        {
            get
            {
                if (instance != null) return instance;
                Monitor.Enter(s_lock);
                AppTexts temp = new AppTexts();
                Interlocked.Exchange(ref instance, temp);
                Monitor.Exit(s_lock);
                return instance;
            }
        }
    }
}
