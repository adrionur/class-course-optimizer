using ClassCourseOptimizer.Console.Models;
using System.Text.Json;

namespace ClassCourseOptimizer.Console.Services
{
    public class ScheduleService
    {
        private readonly List<string> Days = new() { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma" };
        private const int HoursPerDay = 5;

        // JSON verileri saklama amacıyla kullanılacak nesneler
        private static List<string> Classes;
        private static Dictionary<string, Course> Courses;

        public Schedule CreateSchedule()
        {
            LoadData();
            var schedule = new Schedule();

            // Tüm sınıflar için takvim ve derslere ait sayacı oluştur
            foreach (var cls in Classes)
            {
                schedule.ClassSchedules[cls] = new Slot[Days.Count, HoursPerDay];
                schedule.CourseCount[cls] = Courses.ToDictionary(l => l.Key, l => l.Value.Hours);
            }

            // Takvimdeki tüm dersler ve derslere atanacak öğretmenlere ait uygunluk matrislerini oluştur
            foreach (var course in Courses)
            {
                schedule.CourseTeacherCount[course.Key] = new Dictionary<string, int>();

                foreach (var teacher in course.Value.Teachers)
                {
                    if (!schedule.TeacherOccupied.ContainsKey(teacher))
                        schedule.TeacherOccupied[teacher] = new bool[Days.Count, HoursPerDay];

                    if (!schedule.TeacherCourseCount.ContainsKey(teacher))
                        schedule.TeacherCourseCount[teacher] = 0;

                    schedule.CourseTeacherCount[course.Key][teacher] = 0;
                }
            }

            return FillSchedule(schedule, 0, 0, 0, new Dictionary<string, string>()) ? schedule : throw new Exception("Takvim oluşturulamadı");
        }

        private void LoadData()
        {
            var classes = JsonSerializer.Deserialize<List<string>>(File.ReadAllText(Path.Combine("Data", "classes.json")));
            if (classes != null && classes.Any())
                Classes = classes;

            var courses = JsonSerializer.Deserialize<Dictionary<string, Course>>(File.ReadAllText(Path.Combine("Data", "courses.json")));
            if (courses != null && courses.Count != 0)
                Courses = courses;
        }

        private bool FillSchedule(Schedule schedule, int day, int hour, int clsIndex, Dictionary<string, string> lastLesson)
        {
            //Tüm günler doluysa
            if (day == Days.Count)
                return true;

            string cls = Classes[clsIndex];

            // Önce bitmeye yakınları derslere yerleştir
            var lessonsToTry = schedule.CourseCount[cls].Where(l => l.Value > 0)
                                        .OrderBy(l => l.Value)
                                        .ThenBy(l => Guid.NewGuid()) // Random üretim
                                        .ToList();

            foreach (var lesson in lessonsToTry)
            {
                // Bu dersi verebilecek öğretmenleri en az görev alanı ilk olacak şekilde sırala
                foreach (var teacher in Courses[lesson.Key].Teachers
                    .OrderBy(t => TotalAssigned(schedule.TeacherOccupied[t]))
                    .OrderBy(t => schedule.CourseTeacherCount[lesson.Key][t])
                    .ThenBy(t => schedule.TeacherCourseCount[t]))
                {
                    // Sınıf ve öğretmen o anda boşsa
                    if (IsAvailable(schedule, cls, teacher, day, hour))
                    {
                        // Dersi ve öğretmeni uygun ise takvime yerleştir
                        schedule.ClassSchedules[cls][day, hour] = new Slot { Course = lesson.Key, Teacher = teacher };
                        schedule.TeacherOccupied[teacher][day, hour] = true;
                        schedule.CourseCount[cls][lesson.Key]--;
                        schedule.TeacherCourseCount[teacher]++;
                        schedule.CourseTeacherCount[lesson.Key][teacher]++;
                        lastLesson[cls] = lesson.Key;

                        //Bir sonraki sınıfa git eğer sınıf yoksa bir sonraki derse o da olmazsa bir sonraki güne geç
                        //Prio order: Class > Hour > Day
                        int nextCls = (clsIndex + 1) % Classes.Count;
                        int nextHour = clsIndex == Classes.Count - 1 ? hour + 1 : hour;
                        int nextDay = nextHour == HoursPerDay ? day + 1 : day;
                        nextHour %= HoursPerDay;

                        //Küçük boyutlu işlem olduğundan rekürsif çözüm daha çabuk sonuç vermektedir.
                        //Iterative yaklaşım complexityi çok artırıyor.
                        if (FillSchedule(schedule, nextDay, nextHour, nextCls, lastLesson))
                            return true;

                        // Öğretmene uygun takvim aralığı bulunamadığı durumda geri alma işlemi yapılır.
                        // Iterative yaklaşımda stack kullanımı ile implement edilebilir.
                        schedule.ClassSchedules[cls][day, hour] = null;
                        schedule.TeacherOccupied[teacher][day, hour] = false;
                        schedule.CourseCount[cls][lesson.Key]++;
                        schedule.TeacherCourseCount[teacher]--;
                        schedule.CourseTeacherCount[lesson.Key][teacher]--;
                        lastLesson[cls] = null;
                    }
                }
            }

            return false;
        }

        //Tüm verilen uygunluğunu kontrol eden metot. Dolu olması durumunda sonraki ders kontrol edilecektir.
        private bool IsAvailable(Schedule schedule, string cls, string teacher, int day, int hour)
        {
            if (schedule.ClassSchedules[cls][day, hour] != null)
                return false;
            return !schedule.TeacherOccupied[teacher][day, hour];
        }

        //Her bir öğretmenin takvimindeki dolu ders sayısını dönecek metot, 5 x 5 matris
        private int TotalAssigned(bool[,] teacherSchedule)
        {
            int count = 0;
            foreach (var slot in teacherSchedule)
                if (slot) count++;
            return count;
        }
    }
}