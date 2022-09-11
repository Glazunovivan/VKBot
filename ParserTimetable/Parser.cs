using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Net;
using Schedule;
using System.IO;

namespace ParserTimetable
{
    public class Parser
    {
        private HtmlAgilityPack.HtmlDocument _htmlDoc;
        private HtmlAgilityPack.HtmlWeb _web;

        private const string PATH = "url.json";
        private string _url;

        public Parser()
        {
            if (!File.Exists(PATH))
            {
                Console.WriteLine($"Файл настроек {PATH} не найден. Завершаем работу");
                return;
            }
            _url = @"" + File.ReadAllText(PATH);

            _web = new HtmlAgilityPack.HtmlWeb();
            _htmlDoc = new HtmlAgilityPack.HtmlDocument();
        }

        public IEnumerable<DayOfWeekWithLesson> ParseLearningDay()
        {
            List<DayOfWeekWithLesson> days = new List<DayOfWeekWithLesson>(0);

            _htmlDoc = _web.Load(_url);

            //учебные дни в расписании
            var localDays = _htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content-tab1\"]/h2");

            for (int i = 0, j = 0; i < 7; i++)
            {
                DayOfWeekWithLesson dayOfWeek = new DayOfWeekWithLesson();

                string dayName = Utils.Converter.NameOfDay[i];
                //именуем название дня
                dayOfWeek.Day = dayName;

                if (j < localDays.Count)
                {
                    if (dayName == localDays[j].InnerText)
                    {
                        //загружаем занятия на день
                        foreach (Lesson les in ParseLesson(j+1))
                        {
                            dayOfWeek.Lessons.Add(les);
                        }

                        j++;
                    }
                    else
                    {
                        dayOfWeek.Lessons = new List<Lesson>(0);
                    }
                }

                yield return dayOfWeek;
            }
        }

        /// <summary>
        /// Загрузка занятий на целый день
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private IEnumerable<Lesson> ParseLesson(int day)
        {
            string XPathInDay = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr";
            int itemCount = _htmlDoc.DocumentNode.SelectNodes(XPathInDay).Count;

            for (int j = 2; j <= itemCount; j++)
            {
                //string XPathLesson; 
                Lesson lesson = new Lesson();

                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[1]/div  -- это часы
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1] -- название занятия
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[3]/div -- аудитория
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[4]/div -- преподаватель

                string XPathTime = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[1]/div/text()";  //-время
                var time = _htmlDoc.DocumentNode.SelectSingleNode(XPathTime).InnerText;
                string[] splitTime = time.Split('-');

                lesson.TimeStart = splitTime[0];
                lesson.TimeEnd = splitTime[1];

                string XPathLessonName = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[2]/div[1]/text()[1]"; //-название занятия
                string nameLes = _htmlDoc.DocumentNode.SelectSingleNode(XPathLessonName).InnerText;
                lesson.Name = nameLes;

                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1]
                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[3]/td[2]/div[1]/text()[1]
                //XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[2]/div[1]/text()[1]";

                //-аудитория
                string XPathClassroom = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()";  
                string classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathClassroom).InnerText;
                lesson.Classroom = classroom;

                //-корпус
                string XPathKorp = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()[2]";  
                classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathKorp).InnerText;
                lesson.Classroom += " "+classroom;

                //-преподаватель
                string XPathLecturer = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[4]/div/text()";
                string lecture = String.Empty;
                var node = _htmlDoc.DocumentNode.SelectSingleNode(XPathLecturer);

                if (node != null)
                {
                    lecture = _htmlDoc.DocumentNode.SelectSingleNode(XPathLecturer).InnerText;
                }
                else
                {
                    lecture = "Никто";
                }
                lesson.Lecturer = lecture;

                //загружаем ссылку на занятие
                //lesson.Link = GetLinkInLesson(nameLes);

                yield return lesson;
            }
        }

        private List<LinkRemoteLesson> LoadLinks()
        {
            List<LinkRemoteLesson> linkLessons = new List<LinkRemoteLesson>();

            string url = @"https://raw.githubusercontent.com/Glazunovivan/LessonsXML/main/LinksForLessons.xml";
            string xml = new WebClient().DownloadString(url);

            //string xml = File.ReadAllText(@"..\\Data\\LinksForLessons.xml");
            var collection = XDocument.Parse(xml).Descendants("Links").Descendants("Lesson").ToList();

            foreach (var item in collection)
            {
                LinkRemoteLesson linkLesson = new LinkRemoteLesson()
                {
                    Name = item.Attribute("name").Value,
                    Link = item.Element("Link").Value
                };
                linkLessons.Add(linkLesson);
            }

            return linkLessons;
        }
    }
}
