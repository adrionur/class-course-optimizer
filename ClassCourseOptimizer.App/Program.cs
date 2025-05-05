using ClassCourseOptimizer.Console.Models;
using ClassCourseOptimizer.Console.Services;

public class Program
{
    public static void Main()
    {
        var service = new ScheduleService();
        var schedule = service.CreateSchedule();

        if (schedule == null)
            Console.WriteLine("Uygun çizelge oluşturulamadı.");
        else
        {
            PrintSchedule(schedule);
            PrintTeachersCourseCount(schedule);
        }
    }

    /// PRINT METHODS
    /// 
    ///*
    public static void PrintSchedule(Schedule schedule)
    {
        List<string> Days = new() { "Pazartesi", "Salı", "Çarşamba", "Perşembe", "Cuma" };
        foreach (var cls in schedule.ClassSchedules.Keys)
        {
            Console.WriteLine($"\n{cls} Sınıfı Ders Programı:");
            Console.Write("\t");
            foreach (var day in Days)
                Console.Write(day + "\t");
            Console.WriteLine();

            for (int hour = 0; hour < 5; hour++)
            {
                Console.Write($"{hour + 1}. Saat\t");
                foreach (var day in Enumerable.Range(0, Days.Count))
                {
                    var slot = schedule.ClassSchedules[cls][day, hour];
                    if (slot != null)
                        Console.Write($"{slot.Course}, {slot.Teacher}\t");
                    else
                        Console.Write("-\t");
                }
                Console.WriteLine();
            }
        }
    }

    public static void PrintTeachersCourseCount(Schedule schedule)
    {
        Console.WriteLine("\nÖğretmenlerin Haftalık Ders Saatleri:");
        foreach (var kvp in schedule.TeacherCourseCount.OrderBy(k => k.Value))
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value} saat");
        }
    }
    //*/
}