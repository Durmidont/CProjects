using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextLibrary
{
    public class ElementString
    {
        List<string> paramNames = new List<string>();

        /// <summary>
        /// Список названий одного и того же параметра на разных языках
        /// </summary>
        public List<string> ParamNames
        {
            get { return paramNames; }
            set { paramNames = value; }
        }

        /// <summary>
        /// Выдает название параметра на первых LanguageCount языках,
        /// если недостаточно данных, то последние строки пустые
        /// </summary>
        public List<string> GetParamNames(int LanguageCount)
        {
            if (LanguageCount == paramNames.Count) { return paramNames; }
            List<string> pn = new List<string>();

            int i;

            for (i = 0; ((i < paramNames.Count) && (i < LanguageCount)); i++)
            {
                pn.Add(paramNames[i]);
            }

            for (; i < LanguageCount; i++)
            {
                pn.Add("  ");
            }

            return (pn);
        }

        /// <summary>
        /// Указывает заданы ли ограничения для данного параметра
        /// </summary>
        public bool LimitsEnabled { get; set; }

        public double Minimum { get; set; }

        public double Maximum { get; set; }

        public int PointCount { get; set; }
    }
}
