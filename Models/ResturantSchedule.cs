using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OpeningHours.Core.Models
{
    public class ResturantSchedule
    {
        public string WeekDay { get; set; }
        public IList<OpeningHour> OpeningHours { get; set; }
    }

}
