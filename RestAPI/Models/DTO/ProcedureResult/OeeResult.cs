using AmiMesApi.DataBase;
using AmiMesApi.DataBase.ProcedureResults;
using AmiMesApi.Services;
using API_Standard.Models.Base.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Model.Oee
{
    public class OeeResult
    {
        public List<OeeRating> OeeRatingsCollection { get; private set; } = new();
        public List<OeeWorkTime> OeeWorkTimesCollection { get; private set; } = new();
        public List<OeeHourSummary> OeeHourSummariesCollection { get; set; } = new();
        private DateTime NowTime { get; }
        private DateTime ShiftStartTime { get; }
        private readonly OeeDbContext oeeDbContext;
        private readonly List<Area> areas;
        public OeeResult()
        {
        }

        public OeeResult(OeeDbContext oeeDbContext, List<Area> areas, IProductionShiftService productionShiftService)
        {
            this.oeeDbContext = oeeDbContext;
            this.areas = areas;
            NowTime = productionShiftService.TimeNow();
            ShiftStartTime = productionShiftService.GetStartShiftTime();
            GetOeeResult();
        }

        private void GetOeeResult()
        {
            if (areas != null)
            {
                foreach (var area in areas)
                {
                    List<Station> stations = area.Entities.SelectMany(x=>x.Stations).ToList();
                    GetOeeResultForArea(area);
                    foreach (var station in stations)
                    {
                        GetOeeResultForEntity(station);
                    }
                }
            }
            else
            {
                throw new Exception("User wrong configuration");
            }
        }

        private void GetOeeResultForArea(Area area)
        {
            OeeProcedureResult oeeStatus = oeeDbContext.GetOeeStatusForAreaFromProcedure(area, ShiftStartTime, NowTime);

            OeeRating oeeRating = CreateOeeRating(oeeStatus, area.Id, -1);
            List<OeeHourSummary> hourSummary = CreateHourSummaryForArea(area);
            OeeRatingsCollection.Add(oeeRating);

            OeeHourSummariesCollection.AddRange(hourSummary);
        }

        private void GetOeeResultForEntity(Station station)
        {
            OeeProcedureResult oeeStatus = oeeDbContext.GetOeeStatusForEntityFromProcedure(station, ShiftStartTime, NowTime);
            List<OeeDowntimeProc> oeeDowntimes = oeeDbContext.GetDowntimesFromProcedure(station, ShiftStartTime, NowTime);

            OeeRating oeeRating = CreateOeeRating(oeeStatus, station.IdArea, station.Id);
            List<OeeWorkTime> oeeWorktimes = CreateOeeWorktimes(oeeDowntimes, station.IdArea, station.Id);
            List<OeeHourSummary> hourSummary = CreateHourSummaryForEntity(station);

            OeeRatingsCollection.Add(oeeRating);
            OeeWorkTimesCollection.AddRange(oeeWorktimes);
            OeeHourSummariesCollection.AddRange(hourSummary);
        }

        private void GetCountsForVariants()
        {

        }

        private OeeRating CreateOeeRating(OeeProcedureResult oeeStatus, int idArea, int idEntity)
        {
            OeeRating oeeRating = new(oeeStatus, idArea, idEntity);
            return oeeRating;
        }

        private List<OeeWorkTime> CreateOeeWorktimes(List<OeeDowntimeProc> oeeDowntimes, int idArea, int idEntity)
        {
            List<OeeWorkTime> oeeWorktimes = new();
            foreach (var downtime in oeeDowntimes)
            {
                oeeWorktimes.Add(new(downtime, idArea, idEntity));
            }
            return oeeWorktimes;
        }

        private List<OeeHourSummary> CreateHourSummaryForEntity(Station station)
        {
            List<OeeHourSummary> shiftHourSummary = new();
            OeeProcedureResult oeeStatus = oeeDbContext.GetOeeStatusForEntityFromProcedure(station, ShiftStartTime, NowTime);

            OeeHourSummary hourSummary = new(station, -1, oeeStatus);
            shiftHourSummary.Add(hourSummary);

            for (int hour = 0; hour < 8; hour++)
            {
                DateTime fromDate = ShiftStartTime.AddHours(hour);
                DateTime toDate = ShiftStartTime.AddHours(hour + 1);
                oeeStatus = fromDate < NowTime
                    ? oeeDbContext.GetOeeStatusForEntityFromProcedure(station, fromDate, NowTime < toDate ? NowTime : toDate)
                    : (new());
                hourSummary = new(station, ShiftStartTime.Hour + hour, oeeStatus);
                shiftHourSummary.Add(hourSummary);
            }
            return shiftHourSummary;
        }

        private List<OeeHourSummary> CreateHourSummaryForArea(Area area)
        {
            List<OeeHourSummary> shiftHourSummary = new();
            OeeProcedureResult oeeStatus = oeeDbContext.GetOeeStatusForAreaFromProcedure(area, ShiftStartTime, NowTime);

            OeeHourSummary hourSummary = new(new() { IdArea = area.Id, Id = -1 }, -1, oeeStatus);
            shiftHourSummary.Add(hourSummary);

            for (int hour = 0; hour < 8; hour++)
            {
                DateTime fromDate = ShiftStartTime.AddHours(hour);
                DateTime toDate = ShiftStartTime.AddHours(hour + 1);
                oeeStatus = fromDate < NowTime
                    ? oeeDbContext.GetOeeStatusForAreaFromProcedure(area,fromDate, NowTime < toDate ? NowTime : toDate)
                    : (new());
                hourSummary = new(new() { IdArea = area.Id, Id = -1 }, ShiftStartTime.Hour + hour, oeeStatus);
                shiftHourSummary.Add(hourSummary);
            }
            return shiftHourSummary;
        }
    }
}
