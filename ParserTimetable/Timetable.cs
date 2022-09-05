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
        //четная
        private const string _weekDateStart = "2022.09.05 00:00";
        private const string _weekDateEnd = "2022.09.11 23:59";

        private struct EvenWeek
        {
            public DateTime dateStart;
            public DateTime dateEnd;
            public bool isEven;
        }

        private List<EvenWeek> _evenWeeks { get; set; }

        public List<DayOfWeekWithLesson> DayOfWeekWithLessons { get; private set; }

        public Timetable(string url)
        {
            Console.WriteLine("Загружаем расписание");
            Parser parser = new Parser(url);
            DayOfWeekWithLessons = parser.GetLessonsWithDays();

            CalculateWeeks();
        }

        private void CalculateWeeks()
        {
            _evenWeeks = new List<EvenWeek>();
            EvenWeek firstWeek = new EvenWeek();
            firstWeek.dateStart = DateTime.Parse(_weekDateStart);
            firstWeek.dateEnd = DateTime.Parse(_weekDateEnd);
            firstWeek.isEven = false;
            _evenWeeks.Add(firstWeek);

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
                week.dateStart = firstWeek.dateEnd.AddDays((i * 7) + 1);
                week.dateEnd = week.dateStart.AddDays(6);

                _evenWeeks.Add(week);
            }
        }

        /// <summary>
        /// Выдает все занятия на конкретный день
        /// </summary>
        /// <param name="day"></param>
        public void ShowDayWithLessons(int day)
        {
            Console.WriteLine($"{DayOfWeekWithLessons[day].Day}");
            Console.WriteLine(GetDaysWithLessonsInLine(day));
        }

        /// <summary>
        /// Получаем форматированную строку с занятиями, которые зависят от передаваемого дня недели
        /// </summary>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        public string GetLessonsOrEmpty(System.DateTime dateTime)
        {
            int day = ConvertDate(dateTime);

            string result = string.Empty;

            if (DayOfWeekWithLessons[day].Lessons != null)
            {
                result += $"{DayOfWeekWithLessons[day].Day}\n{GetDaysWithLessonsInLine(day)}";
                //result += $"{GetDaysWithLessonsWriteLine(day)}";
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
        public string GetNextLesson(System.DateTime dateTime, bool isNotification = true)
        {
            string result = string.Empty;
            int minute = dateTime.Minute;
            int hour = dateTime.Hour;

            int day = ConvertDate(dateTime);

            DayOfWeekWithLesson currentDay = DayOfWeekWithLessons[day];

            if (currentDay.Lessons.Count == 0 || currentDay.Lessons == null)
            {
                return string.Empty;
            }

            string[] lastLessonTimeParse = currentDay.Lessons[currentDay.Lessons.Count - 1].TimeEnd.Split(':');
            Time lastLesson = new Time()
            {
                Hour = Convert.ToInt32(lastLessonTimeParse[0]),
                Minute = Convert.ToInt32(lastLessonTimeParse[1])
            };

            string[] firstLessonTimeParse = currentDay.Lessons[0].TimeEnd.Split(':');
            Time firstLesson = new Time()
            {
                Hour = Convert.ToInt32(firstLessonTimeParse[0]),
                Minute = Convert.ToInt32(firstLessonTimeParse[1])
            };

            //первая пара
            var timeFrom = new TimeSpan(0, 0, 0);
            var timeTo = new TimeSpan(firstLesson.Hour, firstLesson.Minute, 0);
            var timeCurrent = new TimeSpan(hour,minute,0);
            int numberLes = 0;
            //больше занятий нет
            if (hour >= lastLesson.Hour)
            {
                if (minute >= lastLesson.Minute)
                {
                    result = string.Empty;
                }
            }
            //первая пара на дню
            else if (timeCurrent > timeFrom && timeCurrent < timeTo)
            {
                result = $"{currentDay.Lessons[0].Name}";
            }
            else
            {
                for (int i = 0; i < currentDay.Lessons.Count-1; i++)
                {
                    string[] timeStartParse = currentDay.Lessons[i].TimeStart.Split(':');
                    Time startLesson = new Time()
                    {
                        Hour = Convert.ToInt32(timeStartParse[0]),
                        Minute = Convert.ToInt32(timeStartParse[1])
                    };

                    string[] timeEndParse = currentDay.Lessons[i+1].TimeStart.Split(':');
                    Time endLesson = new Time()
                    {
                        Hour = Convert.ToInt32(timeEndParse[0]),
                        Minute = Convert.ToInt32(timeEndParse[1])
                    };

                    //если в интервале текущего занятия, то выдаем следующее
                    if (InInterval(dateTime.TimeOfDay, startLesson, endLesson) && i < currentDay.Lessons.Count-2)
                    {
                        result = $"{currentDay.Lessons[i + 1].Name}";
                        numberLes = i + 1;

                        if (result.Contains("I"))
                        {
                            if (!IsEvenWeek(dateTime))
                            {
                                break;
                            }
                            else
                            {
                                result = $"{currentDay.Lessons[i + 2].Name}";
                                numberLes = i + 2;
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        
                    }
                    //последнее
                    else
                    {
                        result = $"{currentDay.Lessons[currentDay.Lessons.Count - 1].Name}";
                    }
                }
            }
            if (!isNotification)
            {
                result += $"-{currentDay.Lessons[numberLes].Classroom}";
            }

            return result;
        }

        private string GetDaysWithLessonsInLine(int day)
        {
            string result = string.Empty;

            DayOfWeekWithLesson dayOfWeek = DayOfWeekWithLessons[day];

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
                    case 6:
                        currentLess += "6 пара ";
                        break;
                }

                if (les.Name.Contains("(II)"))  //это четная неделя
                {
                    if (IsEvenWeek(DateTime.Now))
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
                    if (IsEvenWeek(DateTime.Now) == false)
                    {
                        result += currentLess;
                        result += $"({les.TimeStart}-{les.TimeEnd})\n{les.Name}\n{les.Classroom}\n{les.Lecturer}\n{les.Link}\n";
                        result += "\n";
                        result = result.Replace("(I)", "");
                        i++;
                    }
                }
                else //а это если каждую неделю
                {
                    result += currentLess;
                    result += $"({les.TimeStart}-{les.TimeEnd})\n{les.Name}\n{les.Classroom}\n{les.Lecturer}\n{les.Link}\n";
                    result += "\n";
                    i++;
                }
            }
            return result;
        }

        private bool InInterval(TimeSpan timeNow, Time timeStart, Time timeEnd)
        {
            var timeFrom = new TimeSpan(timeStart.Hour, timeStart.Minute, 0);
            var timeTo = new TimeSpan(timeEnd.Hour, timeEnd.Minute, 0);

            if (timeNow > timeFrom && timeNow < timeTo)
            {
                return true;
            }

            return false;
        }

        private bool IsEvenWeek(DateTime dateTime)
        {
            foreach (EvenWeek evenWeek in _evenWeeks)
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
