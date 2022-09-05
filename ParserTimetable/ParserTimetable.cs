﻿using System;
using System.IO;

namespace ParserTimetable
{
    public class ParserTimetable
    {
        private static Timetable _timetable;

        private const string PATH = "url.json";
        private const string URL = @"https://mgupp.ru/obuchayushchimsya/raspisanie/GetShedule.php?MyRes=0x80F1000C299AE95F11EACE571BA3156F_0x80F1000C299AE95F11EAF1064FC71B75_0x80C4000C299AE95511E6FFDE22A08A7E";

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
            if (_timetable == null)
            {
                Start();
            }

            return _timetable.GetLessonsOrEmpty(dateTime);
        }

        /// <summary>
        /// Показывает следующее занятие
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string ShowNextLesson(System.DateTime time)
        {
            return _timetable.GetNextLesson(time);
        }
    }
}
