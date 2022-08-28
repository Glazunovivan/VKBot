using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace HomeWork
{
    public class HomeWorks
    {
        public class HomeWorkSimple
        {
            public string Name { get; set; }
            public List<string> Tasks { get; set; }

            public HomeWorkSimple()
            {
                Name = "";
                Tasks = new List<string>();
            }
        }

        public List<HomeWorkSimple> HomeWorksList { get; set; }

        private const string url = @"https://raw.githubusercontent.com/Glazunovivan/LessonsXML/main/HomeWork.xml";

        /// <summary>
        /// URL прописан в поле класса
        /// </summary>
        public HomeWorks()
        {
            HomeWorksList = new List<HomeWorkSimple>();

            string xml = new WebClient().DownloadString(url);
            var collection = XDocument.Parse(xml).Descendants("HomeWorks").Descendants("Lesson").ToList();

            foreach (var item in collection)
            {
                HomeWorkSimple hws = new HomeWorkSimple();
                
                hws.Name = item.Attribute("name").Value;

                foreach (var task in item.Elements("Task"))
                {
                    hws.Tasks.Add(task.Value);
                }
                HomeWorksList.Add(hws);
            }
        }

        public string Get()
        {
            string result = "";

            foreach (var item in HomeWorksList)
            {
                result += $"\n{item.Name.Replace(".", "")}:";
                foreach (var task in item.Tasks)
                {
                    result += $"\n{task.Replace(@"\n\t\t", "")}";
                }
                result += "\n";
            }

            return result;
        }
    }
}
