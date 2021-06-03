using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpeningHours.Core.Models;
using OpeningHours.Core.Models.DTOs;

namespace OpeningHours.Core.Services
{
    public class ScheduleFormatter : IScheduleFormatter
    {
        // in-app cache
        private static Hashtable timeStore = new Hashtable();
         
        public ICollection<FormattedScheduleForReturnDTO> FormatSchedule
            (IFormFile resturantSchedulesFile)
        {

            IEnumerable<IDictionary<string, List<OpeningHour>>> resturantSchedules = 
                FetchResturantSchedulesFromFile(resturantSchedulesFile);

            //var stopWatch = Stopwatch.StartNew();
            //var formattedSchedule = new List<FormattedScheduleForReturnDTO>();

            //foreach (var schedule in resturantSchedules)
            //{
            //    formattedSchedule.Add(new FormattedScheduleForReturnDTO
            //    {
            //        FormattedSchedule = Format(schedule)
            //    });
            //}


            var formattedSchedule = resturantSchedules
                .Select(schedule => new FormattedScheduleForReturnDTO
                {
                    FormattedSchedule = Format(schedule)
                }).ToList();

            //Debug.WriteLine(stopWatch.Elapsed);

            return formattedSchedule;
            
        }

        #region HelperMethods

        private static IEnumerable<IDictionary<string, List<OpeningHour>>> FetchResturantSchedulesFromFile(IFormFile scheduleFile)
        {
            string jsonFromFile = String.Empty;
            using (var reader = new StreamReader(scheduleFile.OpenReadStream()))
            {
                jsonFromFile = reader.ReadToEnd();
            }

            IEnumerable<IDictionary<string, List<OpeningHour>>> resturantSchedules = new List<IDictionary<string, List<OpeningHour>>>();

            try
            {                
                resturantSchedules = JsonConvert.DeserializeObject<IEnumerable<IDictionary<string, List<OpeningHour>>>>(jsonFromFile);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return resturantSchedules;
        }

        private static string Format(IDictionary<string, List<OpeningHour>> schedule)
        {
            StringBuilder formattedSchedule = new StringBuilder(String.Empty);
            TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;

            var key = schedule.Keys.FirstOrDefault();
            var weekDay = textFormatter.ToTitleCase(key);
            var openingHours = schedule[key];

            if(openingHours.Count == 0) {
                formattedSchedule.Append($"{weekDay}: Closed");
                return formattedSchedule.ToString();
            }

            formattedSchedule.Append($"{weekDay}: ");

            var openingHoursList = openingHours.ToList();

            for (int i = 0; i < openingHoursList.Count; i++) {
                var hour = openingHoursList[i];
                var convertedTime = GetFormattedTime(hour);

                if (openingHours.Count > 1) {
                    if ((i + 1) == openingHours.Count) {
                        formattedSchedule.Append($"{convertedTime}");
                    } else {
                        if ((i + 1) % 2 != 0) {
                            formattedSchedule.Append($"{convertedTime} - ");
                        } else {
                            formattedSchedule.Append($"{convertedTime} , ");
                        }
                    }
                } else {
                    formattedSchedule.Append($"{convertedTime}");
                }
            }
            return formattedSchedule.ToString();
        }

        private static string GetFormattedTime(OpeningHour hour)
        {
            var convertedTime = String.Empty;

            if (timeStore.ContainsKey(hour.Value)) {
                convertedTime = timeStore[hour.Value].ToString();
            } else {
                convertedTime = String.Format("{0:t}", ConvertUnixTimeStampToDateTime(hour.Value));
                timeStore[hour.Value] = convertedTime;
            }

            return convertedTime;
        }

        private static DateTime ConvertUnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        #endregion

    }
}
