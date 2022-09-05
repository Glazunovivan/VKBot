using System;
using System.IO;

namespace ParserTimetable
{
    public class ParserTimetable
    {
        private static Timetable _timetable;

        private const string PATH = "url.json";
       
        public static void Main()
        {
        }

        public static void Start()
        {
            if (!File.Exists(PATH))
            {
                Console.WriteLine($"Файл настроек {PATH} не найден");
                return;
            }

            string url = @""+File.ReadAllText(PATH);

            Console.WriteLine("Запускаем парсер...");
       
            _timetable = new Timetable(url);
            Console.WriteLine("Парсер работает :)");
        }


        /// <summary>
        /// Выдает строку с расписанием на конкретный день
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ShowTimetableOfDay(System.DateTime dateTime)
        {
            return _timetable.GetLessonsOrEmpty(dateTime);
        }

        /// <summary>
        /// Показывает следующее занятие
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static string ShowNextLesson(System.DateTime dateTime)
        {
            return _timetable.GetNextLesson(dateTime);
        }
    }
}
