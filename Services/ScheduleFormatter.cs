using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using OpeningHours.Core.Models;

namespace OpeningHours.Core.Services
{
    public class ScheduleFormatter : IScheduleFormatter
    {
        // in-app cache
        private static Hashtable timeStore = new Hashtable();

        public string FormatSchedule
            (IFormFile resturantSchedulesFile)
        {

            IEnumerable<IDictionary<string, List<OpeningHour>>> resturantSchedules =
                FetchResturantSchedulesFromFile(resturantSchedulesFile);

            StringBuilder readableSchedule = new StringBuilder();

            //var stopWatch = Stopwatch.StartNew();
            var resturantScheduleList = resturantSchedules.ToList();

            for (int i = 0; i < resturantScheduleList.Count - 1; i++)
            {
                var schedule = resturantScheduleList[i];
                var formattedSchedule = Format(schedule);

                var scheduleKey = schedule.Keys.FirstOrDefault();

                var scheduleHours = schedule[scheduleKey];

                //checking for overlapping close time
                if (scheduleHours.Count > 0)
                {
                    var status = scheduleHours[scheduleHours.Count - 1].Type.ToLower();

                    if (status == "open")
                    {
                        var nextSchedule = resturantScheduleList[i + 1];
                        var nextScheduleHours = nextSchedule[nextSchedule.Keys.FirstOrDefault()];
                        var formattedNextScheduleFirstTime = GetFormattedTime(nextScheduleHours[0]);

                        formattedSchedule.Append($"{formattedNextScheduleFirstTime}");
                    }
                }

                formattedSchedule.Append("\n");
                readableSchedule.Append(formattedSchedule);
            }

            //append last one manually
            var lastSchedule = resturantScheduleList[resturantScheduleList.Count - 1];
            var formattedLastSchedule = Format(lastSchedule);

            readableSchedule.Append(formattedLastSchedule);


            return readableSchedule.ToString();

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

        private static StringBuilder Format(IDictionary<string, List<OpeningHour>> schedule)
        {
            StringBuilder formattedSchedule = new StringBuilder(String.Empty);
            TextInfo textFormatter = new CultureInfo("en-US", false).TextInfo;

            var key = schedule.Keys.FirstOrDefault();
            var weekDay = textFormatter.ToTitleCase(key);
            var openingHours = schedule[key];

            if (openingHours.Count == 0)
            {
                formattedSchedule.Append($"{weekDay}: Closed");
                return formattedSchedule;
            }

            formattedSchedule.Append($"{weekDay}: ");

            var openingHoursList = openingHours.ToList();

            for (int i = 0; i < openingHoursList.Count; i++)
            {
                var hour = openingHoursList[i];

                var convertedTime = GetFormattedTime(hour);

                var status = hour.Type.ToLower();

                
                if (i == 0 && status == "close")
                {
                    //escape overlapping close hour
                    continue;
                }
                else
                {
                    if (status == "open")
                    {
                        formattedSchedule.Append($"{convertedTime} - ");
                    }
                    else
                    {
                        if (status == "close" && (i + 1) == openingHoursList.Count)
                            formattedSchedule.Append($"{convertedTime}");
                        else
                            formattedSchedule.Append($"{convertedTime}, ");
                    }
                }
            }
            return formattedSchedule;
        }

        private static string GetFormattedTime(OpeningHour hour)
        {
            var convertedTime = String.Empty;

            if (timeStore.ContainsKey(hour.Value))
            {
                convertedTime = timeStore[hour.Value].ToString();
            }
            else
            {
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
