using Schedule;
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
        private const string NO_LESSONS = "На сегодня занятий больше нет, отдыхайте ;)";

        private struct EvenWeek
        {
            public DateTime dateStart;
            public DateTime dateEnd;
            public bool isEven;
        }

        private List<EvenWeek> EvenWeeks { get; set; }

        public List<DayOfWeekLesson> DaysOfWeek { get; private set; }

        //получаем ссылку на расписание конкретной группы
        public Timetable(string URL)
        {
            Console.WriteLine("Загружаем расписание!");
            Parser parser = new Parser(URL);
            DaysOfWeek = parser.StartParse();

            EvenWeeks = new List<EvenWeek>();
            EvenWeek firstWeek = new EvenWeek();
            firstWeek.dateStart = DateTime.Parse("2022.01.03 00:00");
            firstWeek.dateEnd = DateTime.Parse("2022.01.09 23:59");
            firstWeek.isEven = false;
            EvenWeeks.Add(firstWeek);
            
            for (int i = 0; i < 25; i++)
            {
                EvenWeek week = new EvenWeek();

                if (i % 2 == 0)
                {
                    week.isEven = true;
                }
                else
                {
                    week.isEven = false;
                }
                week.dateStart = firstWeek.dateEnd.AddDays((i*7)+1);
                week.dateEnd = week.dateStart.AddDays(6);

                EvenWeeks.Add(week);
            }
        }

        /// <summary>
        /// Выдает все занятия на конкретный день
        /// </summary>
        /// <param name="day"></param>
        public void ShowDayWithLessons(int day)
        {
            Console.WriteLine($"{DaysOfWeek[day].Day}");
            Console.WriteLine(GetDaysWithLessonsWriteLine(day));
        }

        /// <summary>
        /// Получаем форматированную строку с занятиями, которые зависят от передаваемого дня недели
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public string GetLessons(System.DateTime dateTime)
        {
            int day = ConvertDate(dateTime);

            string result = "";

            if (DaysOfWeek[day].Lessons != null)
            {
                result += $"{DaysOfWeek[day].Day}\n";
                result += $"{GetDaysWithLessonsWriteLine(day)}";
            }
            else
            {
                result = "На сегодня занятий нет!";
            }

            return result;
        }

        /// <summary>
        /// Выводит следующее занятие
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public string GetNextLesson(System.DateTime dateTime)
        {
            string result = "";
            int minute = dateTime.Minute;
            int hour = dateTime.Hour;

            int day = ConvertDate(dateTime);

            DayOfWeekLesson currentDay = DaysOfWeek[day];

            if (currentDay.Lessons.Count == 0 || currentDay.Lessons == null)
            {
                return NO_LESSONS;
            }

            string[] lastLessonTimeEnd = currentDay.Lessons[currentDay.Lessons.Count - 1].TimeEnd.Split(':');
            Time timeLastLess = new Time();
            timeLastLess.Hour = Convert.ToInt32(lastLessonTimeEnd[0]);
            timeLastLess.Minute = Convert.ToInt32(lastLessonTimeEnd[1]);

            string[] firstLessonTimeStart = currentDay.Lessons[0].TimeEnd.Split(':');
            Time timeFirstLes = new Time();
            timeFirstLes.Hour = Convert.ToInt32(firstLessonTimeStart[0]);
            timeFirstLes.Minute = Convert.ToInt32(firstLessonTimeStart[1]);

            if (DateTime.Now.Hour >= timeLastLess.Hour)
            {
                if (DateTime.Now.Minute >= timeLastLess.Minute)
                {
                    result = NO_LESSONS;
                }
            }
            else if (DateTime.Now.Hour <= timeFirstLes.Hour && DateTime.Now.Minute <= timeFirstLes.Minute)
            {
                result = $"{currentDay.Lessons[0].Name}";
            }
            else
            {
                for (int i = 0; i < currentDay.Lessons.Count; i++)
                {
                    string[] timeStart = currentDay.Lessons[i].TimeStart.Split(':');
                    Time timeS = new Time()
                    {
                        Hour = Convert.ToInt32(timeStart[0]),
                        Minute = Convert.ToInt32(timeStart[1])
                    };

                    string[] timeEnd = currentDay.Lessons[i].TimeEnd.Split(':');
                    Time timeE = new Time()
                    {
                        Hour = Convert.ToInt32(timeEnd[0]),
                        Minute = Convert.ToInt32(timeEnd[1])
                    };

                    //если в интервале текущего занятия, то выдаем следующее
                    if (InInterval(DateTime.Now, timeE, timeS) && i < currentDay.Lessons.Count)
                    {
                        result = $"{currentDay.Lessons[i + 1].Name}";
                    }
                    else
                    {
                        result = NO_LESSONS;
                    }
                }
            }
            return result;
        }

        private string GetDaysWithLessonsWriteLine(int day)
        {
            string result = "";

            DayOfWeekLesson dayOfWeek = DaysOfWeek[day];

            int i = 1;
            foreach (Lesson les in dayOfWeek.Lessons)
            {
                string currentLess = "";
                switch (i)
                {
                    case 1:
                        currentLess += "1 пара ";
                        break;
                    case 2:
                        currentLess += "2 пара ";
                        break;
                    case 3:
                        currentLess += "3 пара ";
                        break;
                    case 4:
                        currentLess += "4 пара ";
                        break;
                    case 5:
                        currentLess += "5 пара ";
                        break;
                }

                if (les.Name.Contains("(II)"))  //это четная неделя
                {
                    if (WeekIsEven(DateTime.Now))
                    {
                        result += currentLess;
                        result += $"({les.TimeStart}-{les.TimeEnd})\n{les.Name}\n{les.Classroom}\n{les.Lecturer}\n{les.Link}\n";
                        result += "\n";
                        result = result.Replace("(II)","");
                        i++;
                    }
                }
                else if (les.Name.Contains("(I)"))  //это нечетная неделя
                {
                    if (WeekIsEven(DateTime.Now) == false)
                    {
                        result += currentLess;
                        result += $"({les.TimeStart}-{les.TimeEnd})\n{les.Name}\n{les.Classroom}\n{les.Lecturer}\n{les.Link}\n";
                        result += "\n";
                        result = result.Replace("(I)", "");
                        i++;
                    }
                }
                else    //а это если евривик
                {
                    result += currentLess;
                    result += $"({les.TimeStart}-{les.TimeEnd})\n{les.Name}\n{les.Classroom}\n{les.Lecturer}\n{les.Link}\n";
                    result += "\n";
                    i++;
                }
                
            }

            return result;
        }

        private bool InInterval(DateTime currentTime, Time TimeEnd, Time TimeStart)
        {
            if (currentTime.Hour <= TimeEnd.Hour &&
                currentTime.Hour > TimeStart.Hour &&
                currentTime.Minute <= TimeEnd.Minute &&
                currentTime.Minute > TimeStart.Minute)
            {

                return true;
            }
            return false;
        }

        private bool WeekIsEven(DateTime dateTime)
        {
            foreach (EvenWeek evenWeek in EvenWeeks)
            {
                if (evenWeek.dateStart >= dateTime && dateTime <= evenWeek.dateEnd)
                {
                    return evenWeek.isEven;
                }
            }
            return false;
        }

        private int ConvertDate(DateTime dateTime)
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
