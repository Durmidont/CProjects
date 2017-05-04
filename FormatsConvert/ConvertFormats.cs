using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FormatsConvert
{
    public enum ConvertFormats :int
    {
        Percent = 0, 
        Uint16 = 1,  
        Int16 = 2,  
        /// <summary>
        /// Формат частоты 8000/период
        /// </summary>
        StandartFrequency = 3,
        I8_8 = 4,
        U0_16 = 5,
        StandartSlide = 6,
        Digits = 7,
        RegulMode = 8,
        AVRType = 9,
        IntDiv10 = 10,
        Hex = 11,
        /// <summary>
        /// Для систем, где Uf рассчитывается по Ute Uf = 0.135*Value
        /// </summary>
        UfOldStyle = 12,
        /// <summary>
        /// Формат частоты полученной с компаратора 25000 - 50 Гц
        /// </summary>
        FineFrequency = 13,
        CurrentTrans = 14,
        TECurrentAlarm = 15,
        IntDiv8 = 16,
        UintDiv1000 = 17,
        Percent4 = 18,
        Freq90000 = 19,
        PercentUpp = 20,

        /// <summary>
        /// Используется в УПТФ
        /// </summary>
        Freq16000 = 21,
        /// <summary>
        /// Формат приведенной частоты в ПЧ 0x1E84 соответствует 50 Гц
        /// </summary>
        FCFrequency = 22,

        RefPF = 23,

        /// <summary>
        /// Возвращает количество мкс из количества циклов
        /// </summary>
        MksCycles = 24,


        /// <summary>
        /// КОЛИЧЕСТВО ФОРМАТОВ ИСПОЛЬЗУЕМЫХ В ПРОЕКТЕ
        /// </summary>
        FormatCount = 25
    }
}
