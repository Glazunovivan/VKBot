using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;
using System.Net;

namespace ParserTimetable
{
    public class Parser
    {
        private HtmlAgilityPack.HtmlDocument _htmlDoc;
        private HtmlAgilityPack.HtmlWeb _web;
        private string _url;
        private List<LinkLesson> _links;

        public Parser(string url)
        {
            _url = url;
            _web = new HtmlAgilityPack.HtmlWeb();
            _htmlDoc = new HtmlAgilityPack.HtmlDocument();
        }

        public List<DayOfWeek> StartParse()
        {
            _links = LoadLinks();

            List<DayOfWeek> days = new List<DayOfWeek>();

            _htmlDoc = _web.Load(_url);

            var localDays = _htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content-tab1\"]/h2");

            for (int i = 0, j = 0; i < 7; i++)
            {
                DayOfWeek dayOfWeek = new DayOfWeek();
                string day = "";

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

                if (j < localDays.Count)
                {
                    if (day == localDays[j].InnerText)
                    {
                        //загружаем занятия на день
                        dayOfWeek.Lessons = LoadLessons(j);
                        j++;
                    }
                    else
                    {
                        dayOfWeek.Lessons = null;
                    }
                }
                //именуем название дня
                dayOfWeek.Day = day;
                days.Add(dayOfWeek);

            }
            return days;
        }
        
        private List<Lesson> LoadLessons(int day)
        {
            List<Lesson> lessons = new List<Lesson>();
            
            day++;

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

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()";  //-аудитория
                string classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Classroom = classroom;

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()[2]";  //-корпус
                classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Classroom += " "+classroom;

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[4]/div/a/text()";  //-преподаватель
                string lecture = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Lecturer = lecture;

                //загружаем ссылку на занятие
                lesson.Link = GetLinkInLesson(nameLes);

                lessons.Add(lesson);
            }

            return lessons;
        }

        private List<LinkLesson> LoadLinks()
        {
            List<LinkLesson> linkLessons = new List<LinkLesson>();

            string url = @"https://raw.githubusercontent.com/Glazunovivan/LessonsXML/main/LinksForLessons.xml";
            string xml = new WebClient().DownloadString(url);

            //string xml = File.ReadAllText(@"..\\Data\\LinksForLessons.xml");
            var collection = XDocument.Parse(xml).Descendants("Links").Descendants("Lesson").ToList();

            foreach (var item in collection)
            {
                LinkLesson linkLesson = new LinkLesson();
                linkLesson.Name = item.Attribute("name").Value;
                linkLesson.Link = item.Element("Link").Value;
                linkLessons.Add(linkLesson);
            }

            return linkLessons;
        }

        private string GetLinkInLesson(string name)
        {
            foreach (LinkLesson item in _links)
            {
                name = name.TrimEnd(' ');
                if (item.Name == name)
                {
                    return item.Link;
                }
            }
            return "";
        }
    }
}
