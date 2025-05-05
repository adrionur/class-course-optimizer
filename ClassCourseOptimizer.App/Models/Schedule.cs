namespace ClassCourseOptimizer.Console.Models
{
    public class Schedule
    {
        // Her sınıfın takvim bilgisini tutan nesne. Ör: Key: 1A, Value = 5x5 Slot matrisi
        public Dictionary<string, Slot[,]> ClassSchedules = new();
        // Öğretmenler için 5x5'lik uygunluk tablosu
        public Dictionary<string, bool[,]> TeacherOccupied = new();
        // Sınıflara ait derslerin kalan miktarını hesaplamak için kullanılacak kvp (Ör: 1A sınıfı için Türkçe:4, Matematik: 3 vb.)
        public Dictionary<string, Dictionary<string, int>> CourseCount = new();
        // Her öğretmenin toplam verdiği ders saati
        public Dictionary<string, int> TeacherCourseCount = new();
        // Her ders için öğretmen başına atanan ders sayısı
        public Dictionary<string, Dictionary<string, int>> CourseTeacherCount = new();
    }
}
