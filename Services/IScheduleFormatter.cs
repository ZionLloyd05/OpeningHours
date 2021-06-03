using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using OpeningHours.Core.Models;
using OpeningHours.Core.Models.DTOs;

namespace OpeningHours.Core.Services
{
    public interface IScheduleFormatter
    {
        ICollection<FormattedScheduleForReturnDTO> FormatSchedule
            (IFormFile resturantSchedulesFile); 
    }
}
