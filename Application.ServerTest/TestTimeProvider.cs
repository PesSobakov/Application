using Application.Server.Services;

namespace TaskBoard.ServerTest
{
    internal class TestTimeProvider:ITimeProvider
    {
        DateTime CurrentTime;
        public DateTime Now()
        {
            return CurrentTime;
        }
        public void SetTime(DateTime dateTime)
        {
            CurrentTime = dateTime;
        }
    }
}
