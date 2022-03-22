using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParserTimetable
{
    /// <summary>
    /// Содержит список занятий на день
    /// </summary>
    public class DayOfWeek
    {
        public string Day { get; set; }

        public List<Lesson> Lessons { get; set; } 
    }
}
