using System.Collections.Generic;

namespace Schedule
{
    public class DayOfWeekWithLesson
    {
        public string Day { get; set; }

        public List<Lesson> Lessons { get; set; } = new List<Lesson>();
    }
}
