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

        /// <summary>
        /// Recieves schedule file and returns an array of formatted schedule
        /// </summary>
        /// <param name="resturantSchedulesFile"></param>
        /// <response code="200">Info updated</response>
        /// <response code="400">Bad request</response>
        /// <response code="500">Server went bad</response>
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
