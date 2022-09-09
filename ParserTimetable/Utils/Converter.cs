using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTimetable.Utils
{
    public class Converter
    {
        public static Dictionary<int, string> NameOfDay { get; private set; } = new Dictionary<int, string>()
        {
            { 0, "Понедельник"},
            { 1, "Вторник"},
            { 2, "Среда"},
            { 3, "Четверг"},
            { 4, "Пятница"},
            { 5, "Суббота"},
            { 6, "Воскресенье"},
        };
    }
}
