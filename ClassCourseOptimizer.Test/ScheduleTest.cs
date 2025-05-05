using ClassCourseOptimizer.Console.Models;
using ClassCourseOptimizer.Console.Services;

namespace ClassCourseOptimizer.Test
{
    public class ScheduleTest
    {
        private static ScheduleService _scheduleService = null;
        [SetUp]
        public void Setup()
        {
            if (_scheduleService == null)
                _scheduleService = new ScheduleService();
        }

        [Test]
        public void CreateMultipleSchedule()
        {

            int i = 0;
            List<Schedule> schedules = new List<Schedule>();
            try
            {
                for (i = 0; i < 1000; i++)
                {
                    var schedule = _scheduleService.CreateSchedule();
                    schedules.Add(schedule);
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(i + " " + ex.ToString());
            }
            Assert.That(schedules.Count, Is.EqualTo(1000));

        }
    }
}