using AmiMesApi.Model.Oee;
using System;

namespace API_AmiOEE.Models
{
    public class DowntimeShedule
    {
        public int Id { get; set; }
        public int IdStation { get; set; }
        public int DowntimeReason { get; set; }
        public TimeSpan HourStart { get; set; }
        public TimeSpan HourStop { get; set; }
        public DateTime LastStartDate { get; set; }
        public DateTime LastStopDate { get; set; }
        public DowntimeShedule()
        {

        }
        public DowntimeShedule(DbDowntimeSchedule dbDowntimeSchedule, int downtimeCode, DateTime now)
        {
            Id = dbDowntimeSchedule.Id;
            IdStation = dbDowntimeSchedule.IdStation;
            DowntimeReason = downtimeCode;
            HourStart = dbDowntimeSchedule.HourStart;
            HourStop = dbDowntimeSchedule.HourStop;

            LastStartDate = (now.TimeOfDay - dbDowntimeSchedule.HourStart).TotalSeconds > 0
                ? now.Date + dbDowntimeSchedule.HourStart
                : now.AddDays(-1).Date + dbDowntimeSchedule.HourStart;

            LastStopDate = (now.TimeOfDay - dbDowntimeSchedule.HourStop).TotalSeconds > 0
                    ? now.Date + dbDowntimeSchedule.HourStop
                    : now.AddDays(-1).Date + dbDowntimeSchedule.HourStop;
        }

        public bool ShouldBeStartedNow(DateTime now) => (now - LastStartDate).TotalDays > 1;
        public bool ShouldBeStoppedNow(DateTime now) => (now - LastStopDate).TotalDays > 1;
        public bool IsPlannedDowntimeNow(DateTime now) => now.TimeOfDay > HourStart && now.TimeOfDay < HourStop;
    }
}
