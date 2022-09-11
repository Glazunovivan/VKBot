using Schedule;
using System;
using System.Collections.Generic;

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

        private class EvenWeek
        {
            public DateTime dateStart;
            public DateTime dateEnd;
            public bool isEven;
        }

        private List<EvenWeek> _evenWeeks { get; set; }

        public List<DayOfWeekWithLesson> DayOfWeekWithLessons { get; private set; }

        public Timetable()
        {
            Console.WriteLine("Загружаем расписание");
            Parser parser = new Parser();

            foreach (DayOfWeekWithLesson dayOfWeekWithLesson in parser.ParseLearningDay())
            {
                DayOfWeekWithLessons.Add(dayOfWeekWithLesson);
            }

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
            int day = Utils.Converter.ConvertDateToInt(dateTime);

            string result = string.Empty;

            if (DayOfWeekWithLessons[day].Lessons.Count > 0)
            {
                result += $"{DayOfWeekWithLessons[day].Day}\n{GetDaysWithLessonsInLine(day)}";
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

            int day = Utils.Converter.ConvertDateToInt(dateTime);

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
                string currentLess = ""+Utils.Converter.NameOfNumberLesson[i];


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

            return (timeNow > timeFrom && timeNow < timeTo) ? true : false;
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
    }
}
