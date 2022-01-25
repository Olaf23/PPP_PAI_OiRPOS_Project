using AmiMesApi.Model.Oee;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API_AmiOEE.Models
{
    public class StationWorkStatus
    {
        public int IdStation { get; set; }
        public DbDowntimeHistory CurrentProductionWorkStatus { get; set; } = new();
        public List<DowntimeShedule> DowntimeSheduleTable { get; set; } = new();
        public bool IsAnyPlannedDowntimeToStartNow(DateTime now) => DowntimeSheduleTable.Any(x => x.ShouldBeStartedNow(now));
        public bool IsAnyPlannedDowntimeToStopNow(DateTime now) => DowntimeSheduleTable.Any(x => x.ShouldBeStoppedNow(now));
        public bool IsPlannedDowntimeNow(DateTime now) => DowntimeSheduleTable.Any(x => x.IsPlannedDowntimeNow(now));
        public DowntimeShedule GetCurrentPlannedDowntime(DateTime now) => DowntimeSheduleTable.SingleOrDefault(x=>x.ShouldBeStartedNow(now));
        public DowntimeShedule GetLastPlannedDowntime(DateTime now) => DowntimeSheduleTable.SingleOrDefault(x=>x.ShouldBeStoppedNow(now));
        public StationWorkStatus()
        {

        }

        public StationWorkStatus(int idStation, StationWorkStatus lastStatus)
        {
            IdStation = idStation;
            CurrentProductionWorkStatus = lastStatus != null ? lastStatus.CurrentProductionWorkStatus : default;
            DowntimeSheduleTable = lastStatus != null ? lastStatus.DowntimeSheduleTable : default;
        }
    }
}
