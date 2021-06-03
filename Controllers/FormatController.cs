using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpeningHours.Core.Services;

namespace OpeningHours.Core.Controllers
{
    [ApiController]
    public class FormatController : Controller
    {
        private readonly IScheduleFormatter scheduleFormatter;

        public FormatController(IScheduleFormatter scheduleFormatter)
        {
            this.scheduleFormatter = scheduleFormatter;
        }

        [HttpPost("")]
        public IActionResult Format(IFormFile resturantSchedulesFile)
        {
            try
            {
                var formattedResponse =
                 this.scheduleFormatter.FormatSchedule(resturantSchedulesFile);

                return Ok(formattedResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
