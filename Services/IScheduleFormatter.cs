using Microsoft.AspNetCore.Http;


namespace OpeningHours.Core.Services
{
    public interface IScheduleFormatter
    {
        string FormatSchedule(IFormFile resturantSchedulesFile);
    }
}
