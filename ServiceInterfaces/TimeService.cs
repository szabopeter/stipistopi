using System;

namespace ServiceInterfaces
{
    public interface ITimeService
    {
        DateTime Now { get; }
    }

    public class RealTimeService : ITimeService
    {
        public DateTime Now => DateTime.Now;
    }

    public static class TimeService
    {
        public static ITimeService Instance { get; set; } = new RealTimeService();
    }
}