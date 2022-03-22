using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using AngleSharp.Html.Dom;

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

        public List<DayOfWeek> LoadDays()
        {
            List<DayOfWeek> days = new List<DayOfWeek>();

            //_days = new List<string>();

            _htmlDoc = _web.Load(_url);

            var localDays = _htmlDoc.DocumentNode.SelectNodes("//*[@id=\"content-tab1\"]/h2");

            for (int i = 0; i < localDays.Count; i++)
            {
                DayOfWeek dayOfWeek = new DayOfWeek();
                
                string day = localDays[i].InnerText;
                //_days.Add(localDays[i].InnerText);
                
                //именуем название дня
                dayOfWeek.Day = day;
                //загружаем занятия на день
                dayOfWeek.Lessons = LoadLessons(i);

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
                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[4]/td[2]/div[1]/text()[1]
                ////*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1]
                //XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[2]/div[1]/text()[1]";
                //var str = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                //lesString.Add(str);

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[3]/div/text()";  //-аудитория
                string classroom = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Classroom = classroom;

                XPathLesson = $"//*[@id=\"content-tab1\"]/div[{day}]/table/tr[{j}]/td[4]/div/a/text()";  //-преподаватель
                string lecture = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
                lesson.Lecturer = lecture;

                lessons.Add(lesson);
            }

            return lessons;
        }

        //private void LoadLessons()
        //{
        //    _lessons = new Dictionary<int, List<string>>();

        //    //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]
        //    //*[@id="content-tab1"]/div[1]/table/tbody/tr[3]
        //    //*[@id="content-tab1"]/div[1]/table/tbody/tr[4]
        //    //key - день, list<string> - занятия

        //    for (int i = 1; i <= _days.Count; i++)
        //    {
        //        //получим количество занятий на каждый день
        //        //*[@id="content-tab1"]/div[1]/table/tbody - таблица
        //        //*[@id="content-tab1"]/div[1]/table/tbody/tr[1]
        //        //*[@id="content-tab1"]/div[1]/table/tbody/tr[1]
        //        string XPathInDay = $"//*[@id=\"content-tab1\"]/div[{i}]/table/tr";
        //        int itemCount = _htmlDoc.DocumentNode.SelectNodes(XPathInDay).Count;

        //        List<string> lesString = new List<string>();
        //        for (int j = 2; j <= itemCount; j++)
        //        {
        //            //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1]
        //            //*[@id="content-tab1"]/div[1]/table/tbody/tr[3]/td[2]/div[1]/text()[1]
        //            //*[@id="content-tab1"]/div[1]/table/tbody/tr[4]/td[2]/div[1]/text()[1]
        //            //*[@id="content-tab1"]/div[1]/table/tbody/tr[2]/td[2]/div[1]/text()[1]
        //            string XPathLesson = $"//*[@id=\"content-tab1\"]/div[{i}]/table/tr[{j}]/td[2]/div[1]/text()[1]";
        //            var str = _htmlDoc.DocumentNode.SelectSingleNode(XPathLesson).InnerText;
        //            lesString.Add(str);
        //        }
        //        _lessons.Add(i, lesString);
        //    }
        //}

    }
}
