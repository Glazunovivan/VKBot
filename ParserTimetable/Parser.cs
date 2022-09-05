using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Net;
using Schedule;

namespace ParserTimetable
{
    public class Parser
    {
        private HtmlAgilityPack.HtmlDocument _htmlDoc;
        private HtmlAgilityPack.HtmlWeb _web;
        
        private string _url;

        public Parser(string url)
        {
            _url = url;
            _web = new HtmlAgilityPack.HtmlWeb();
            _htmlDoc = new HtmlAgilityPack.HtmlDocument();
        }

        public List<DayOfWeekWithLesson> StartParse()
        {
            List<DayOfWeekWithLesson> days = new List<DayOfWeekWithLesson>();

            _htmlDoc = _web.Load(_url);

            //учебные дни в расписании
            var localDays = _htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content-tab1\"]/h2");

            for (int i = 0, j = 0; i < 7; i++)
            {
                DayOfWeekWithLesson dayOfWeek = new DayOfWeekWithLesson();

                string dayName = NameOfDay(i);

                if (j < localDays.Count)
                {
                    if (dayName == localDays[j].InnerText)
                    {
                        //загружаем занятия на день
                        dayOfWeek.Lessons = LoadLessons(j+1);
                        j++;
                    }
                    else
                    {
                        dayOfWeek.Lessons = new List<Lesson>(0);
                    }
                }
                //именуем название дня
                dayOfWeek.Day = dayName;
                days.Add(dayOfWeek);
            }
            return days;
        }

        private string NameOfDay(int i)
        {
            string day = String.Empty;
            switch (i)
            {
                case 0:
                    day = "Понедельник";
                    break;
                case 1:
                    day = "Вторник";
                    break;
                case 2:
                    day = "Среда";
                    break;
                case 3:
                    day = "Четверг";
                    break;
                case 4:
                    day = "Пятница";
                    break;
                case 5:
                    day = "Суббота";
                    break;
                case 6:
                    day = "Воскресенье";
                    break;
            }
            return day;
        }

        /// <summary>
        /// Загрузка занятий на целый день
        /// </summary>
        /// <param name="day"></param>
        /// <returns></returns>
        private List<Lesson> LoadLessons(int day)
        {
            List<Lesson> lessons = new List<Lesson>();
           
            string XPathInDay = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr";
            int itemCount = _htmlDoc.DocumentNode.SelectNodes(XPathInDay).Count;

            for (int j = 2; j <= itemCount; j++)
            {
                string XPathLesson; 
                Lesson lesson = new Lesson();

                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[1]/div  -- это часы
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1] -- название занятия
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[3]/div -- аудитория
                //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[4]/div -- преподаватель

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[1]/div/text()";  //-время
                var time = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                string[] splitTime = time.Split('-');

                lesson.TimeStart = splitTime[0];
                lesson.TimeEnd = splitTime[1];

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[2]/div[1]/text()[1]"; //-название занятия
                string nameLes = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Name = nameLes;

                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1]
                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[3]/td[2]/div[1]/text()[1]
                //XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[2]/div[1]/text()[1]";

                //-аудитория
                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()";  
                string classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Classroom = classroom;

                //-корпус
                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()[2]";  
                classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Classroom += " "+classroom;

                //-преподаватель
                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[4]/div/text()";

                string lecture = String.Empty;
                var node = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson);

                if (node != null)
                {
                    lecture = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                }
                else
                {
                    lecture = " :( ";
                }
                lesson.Lecturer = lecture;

                //загружаем ссылку на занятие
                //lesson.Link = GetLinkInLesson(nameLes);

                lessons.Add(lesson);
            }

            return lessons;
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
