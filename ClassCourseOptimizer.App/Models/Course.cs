namespace ClassCourseOptimizer.Console.Models
{
    public class Course
    {
        //Haftalık ders saati (Ör: Matematik: 4)
        public int Hours { get; set; }
        //Dersi veren öğretmen listesi (Ör: Matematik için Mehmet, Özlem)
        public List<string> Teachers { get; set; } = [];
    }
}
