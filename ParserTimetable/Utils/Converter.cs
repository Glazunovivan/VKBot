using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTimetable.Utils
{
    public abstract class Converter
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

        public static Dictionary<int, string> NameOfNumberLesson { get; private set; } = new Dictionary<int, string>()
        {
            {1, "1 пара "},
            {2, "2 пара "},
            {3, "3 пара "},
            {4, "4 пара "},
            {5, "5 пара "},
            {6, "6 пара "},
        };

        public static int ConvertDateToInt(DateTime dateTime)
        {
            int result = 0;
            switch (dateTime.DayOfWeek)
            {
                case System.DayOfWeek.Monday:
                    result = 0;
                    break;
                case System.DayOfWeek.Tuesday:
                    result = 1;
                    break;
                case System.DayOfWeek.Wednesday:
                    result = 2;
                    break;
                case System.DayOfWeek.Thursday:
                    result = 3;
                    break;
                case System.DayOfWeek.Friday:
                    result = 4;
                    break;
                case System.DayOfWeek.Saturday:
                    result = 5;
                    break;
                case System.DayOfWeek.Sunday:
                    result = 6;
                    break;
            }

            return result;
        }
    }
}
