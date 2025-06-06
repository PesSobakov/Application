namespace Application.Server.Services
{
    public class TimeProvider : ITimeProvider
    {
        public DateTime Now()
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "FLE Standard Time"); ;
        }
    }
}
