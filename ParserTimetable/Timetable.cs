using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTimetable
{
    /// <summary>
    /// Содержит всю информацию по расписанию
    /// </summary>
    public class Timetable
    {
        public List<DayOfWeek> DaysOfWeek { get; private set; }
        
        //получаем ссылку на расписание конкретной группы
        public Timetable(string URL)
        {
            Console.WriteLine("Загружаем расписание!");
            Parser parser = new Parser(URL);
            DaysOfWeek = parser.LoadDays();
        }

        private string GetAllDaysWriteLine()
        {
            string result = "";
            foreach (DayOfWeek day in DaysOfWeek)
            {
                result += $"{day.Day}\n";
            }
            return result;
        }
        private string GetDaysWithLessonsWriteLine(int day)
        {
            string result = "";

            DayOfWeek dayOfWeek = DaysOfWeek[day];

            foreach (Lesson les in dayOfWeek.Lessons)
            {
                result += $"{les.Name}\t{les.Lecturer}\t({les.TimeStart}-{les.TimeEnd})\n";
            }

            return result;
        }

        public void ShowAllDays()
        {
            Console.WriteLine(GetAllDaysWriteLine());
        }

        public void ShowDayWithLessons(int day)
        {
            Console.WriteLine($"\t\t{DaysOfWeek[day].Day}");
            Console.WriteLine(GetDaysWithLessonsWriteLine(day));
        }

        public string GetDayWithLessons(int day)
        {
            string result = "";
            result += $"{DaysOfWeek[day].Day}\n";
            result += $"{GetDaysWithLessonsWriteLine(day)}";

            return result;

        }
    }
}
