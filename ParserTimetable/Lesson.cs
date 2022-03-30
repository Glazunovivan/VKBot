namespace ParserTimetable
{
    /// <summary>
    /// Занятие, со всеми данными
    /// </summary>
    public class Lesson
    {
        /// <summary>
        /// Название занятия
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Ссылка на курс
        /// </summary>
        public string Link { get; set; }

        /// <summary>
        /// Начало занятия
        /// </summary>
        public string TimeStart { get; set; }

        /// <summary>
        /// Конец занятия
        /// </summary>
        public string TimeEnd { get; set; }

        /// <summary>
        /// Преподаватель
        /// </summary>
        public string Lecturer { get; set; }

        /// <summary>
        /// Аудитория
        /// </summary>
        public string Classroom { get; set; }
    }
}
