using System;

namespace ParserTimetable
{
    public class ParserTimetable
    {
        private static Timetable timetable;

        public static void Main()
        {

        }

        public static void Start()
        {
            Console.WriteLine("Запускаем парсер...");
            string URL = @"https://mgupp.ru/obuchayushchimsya/raspisanie/GetShedule.php?MyRes=0x80F1000C299AE95F11EACE571BA3156F_0x80F1000C299AE95F11EAF106441DF5BF_0x80C4000C299AE95511E6FFDE22A08A7D";

            timetable = new Timetable(URL);
            Console.WriteLine("Парсер работает :)");
        }

        public static string ShowTimetableOfDay(System.DateTime dateTime)
        {
            if (timetable == null)
            {
                Start();
            }

            return timetable.GetLessons(dateTime);
        }

        public static string ShowNextLesson(System.DateTime time)
        {
            return timetable.GetNextLesson(time);
        }
    }
}
