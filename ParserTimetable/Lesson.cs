using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTimetable
{
    /// <summary>
    /// Занятие, со всеми данными
    /// </summary>
    public class Lesson
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public string TimeStart { get; set; }
        public string TimeEnd { get; set; }
        public string Lecturer { get; set; }
        public string Classroom { get; set; }
    }
}
