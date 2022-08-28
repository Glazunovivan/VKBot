using System.Collections.Generic;

namespace Schedule
{
    public class DayOfWeekLesson
    {
        public string Day { get; set; }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
