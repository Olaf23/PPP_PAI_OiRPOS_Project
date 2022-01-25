using AmiMesApi.DataBase.ProcedureResults;
using AmiMesApi.Model;
using AmiMesApi.Model.Oee;
using AmiMesApi.Services;
using API_AmiOEE.Models.Enums;
using API_AmiOEE.Services;
using API_AmiOEE.Storages;
using API_Standard.DataBase.Base;
using API_Standard.Models.Base.Structure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace AmiMesApi.DataBase
{
    public class OeeDbContext : AmiMesSystemDbContext
    {
        private readonly IProductionShiftService productionShiftService;
        private readonly IWorkTimeStatusStorage workTimeStatusStorage;

        private readonly string getDowntimesProcedure;
        private readonly string getOeeFormulasForEntityProcedure;
        private readonly string getOeeFormulasForAreaProcedure;
        private readonly string getVariantsCounts;

        public OeeDbContext(DbContextOptions<OeeDbContext> options,
                            IAppSeettingsReader appSeettingsReader,
                            IProductionShiftService productionShiftService,
                            IWorkTimeStatusStorage workTimeStatusStorage) : base(options)
        {
            this.productionShiftService = productionShiftService;
            this.workTimeStatusStorage = workTimeStatusStorage;
            getDowntimesProcedure = appSeettingsReader.AppSettings.SqlProcedures.GetDowntimes;
            getOeeFormulasForEntityProcedure = appSeettingsReader.AppSettings.SqlProcedures.GetOeeFormulasForStation;
            getOeeFormulasForAreaProcedure = appSeettingsReader.AppSettings.SqlProcedures.GetOeeFormulasForArea;
            getVariantsCounts = appSeettingsReader.AppSettings.SqlProcedures.GetVariantsCounts;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("AmiOEE");
            SetQueryFilters(modelBuilder);
        }

        #region DataSets
        //History
        public DbSet<DbDowntimeHistory> DownTimeHistoryDbTable { get; set; }
        public DbSet<DbProductionPartHistory> ProductionPartsHistoryDbTable { get; set; }

        //Ties
        public DbSet<DbStationDowntimeTies> StationsToDowntimesTiesDbTable { get; set; }

        //DynamicData
        public DbSet<DbDowntimeReason> DowntimeReasonsDbTable { get; set; }
        public DbSet<DbDowntimeSchedule> DownTimeScheduleDbTable { get; set; }

        //Procedure Results
        public DbSet<OeeRating> OeeRatings { get; set; }
        public DbSet<OeeHourSummary> OeeHourSummary { get; set; }
        public DbSet<OeeWorkTime> OeeWorkTimes { get; set; }

        public DbSet<OeeProcedureResult> OeeStatuses { get; set; }
        public DbSet<OeeDowntimeProc> DowntimesTable { get; set; }
        public DbSet<VariantsCountsProc> VariantsCounts { get; set; }
        #endregion

        #region GET

        #region GetOeeResult
        public OeeResult GetOeePack(int userId)
        {
            List<Area> areas = GetProductionStructureForUser(userId);
            OeeResult oeeResult = new(this, areas, productionShiftService);
            return oeeResult;
        }

        public List<OeeDowntimeProc> GetDowntimesFromProcedure(Station station, DateTime fromDate, DateTime toDate)
        {
            string procedureQuery = $"{getDowntimesProcedure} {station.Id}, '{fromDate:yyyy-MM-dd HH:mm:ss}', '{toDate:yyyy-MM-dd HH:mm:ss}'";
            List<OeeDowntimeProc> oeeDowntimes = DowntimesTable.FromSqlRaw(procedureQuery).ToList();
            return oeeDowntimes;
        }

        public OeeProcedureResult GetOeeStatusForEntityFromProcedure(Station station, DateTime fromDate, DateTime toDate)
        {
            string procedureQuery = $"{getOeeFormulasForEntityProcedure} {station.Id}, '{fromDate:yyyy-MM-dd HH:mm:ss}', '{toDate:yyyy-MM-dd HH:mm:ss}'";
            OeeProcedureResult oeeStatus = OeeStatuses.FromSqlRaw(procedureQuery).ToList().First();
            return oeeStatus;
        }

        public OeeProcedureResult GetOeeStatusForAreaFromProcedure(Area area, DateTime fromDate, DateTime toDate)
        {
            string procedureQuery = $"{getOeeFormulasForAreaProcedure} {area.Id}, '{fromDate:yyyy-MM-dd HH:mm:ss}', '{toDate:yyyy-MM-dd HH:mm:ss}'";
            OeeProcedureResult oeeStatus = OeeStatuses.FromSqlRaw(procedureQuery).ToList().First();
            return oeeStatus;
        }
        #endregion
        #endregion

        #region PUT

        #region PlcDriver
        public async Task PutDowntime(DbDowntimeHistory downtime)
        {
            if (!workTimeStatusStorage.IsPlannedDowntimeNow(downtime.IdStation, productionShiftService.TimeNow()))
            {
                await InsertDowntimeIntoDB(downtime);
            }
            else
            {
                if (downtime.WorkStatus != (int)WorkTimeStatus.Planned)
                {
                    workTimeStatusStorage.UpdateWorkStatus(downtime);
                }
                else
                {
                    await InsertDowntimeIntoDB(downtime);
                }
            }
        }

        private async Task InsertDowntimeIntoDB(DbDowntimeHistory downtime)
        {
            DbDowntimeHistory lastStationDownTime = DownTimeHistoryDbTable.OrderByDescending(x => x.DtStart)
                    .FirstOrDefault(x => x.IdStation == downtime.IdStation && x.Duration == null);
            if (lastStationDownTime != null)
            {
                lastStationDownTime.Duration = (downtime.DtStart - lastStationDownTime.DtStart).TotalSeconds;
                DownTimeHistoryDbTable.Add(downtime);
                await SaveChangesAsync();
                if (downtime.WorkStatus != (int)WorkTimeStatus.Planned)
                {
                    workTimeStatusStorage.UpdateWorkStatus(downtime);
                }
            }
            else
            {
                throw new Exception($"No Initial row for stationId = {downtime.IdStation}");
            }
        }
        #endregion

        #region Manager
        public async Task PutStationToDowntimesTies(List<OeeStationToDowntimeTiesUpdate> tiesUpdate)
        {
            foreach (var tie in tiesUpdate)
            {
                foreach (var station in tie.StationsToTie)
                {
                    DbStationDowntimeTies tieFromDb;
                    tieFromDb = await StationsToDowntimesTiesDbTable.SingleOrDefaultAsync(x => x.IdDowntime == tie.IdDowntime && x.IdStation == station.Id);
                    if (tieFromDb != null)
                    {
                        if (!station.IsTiedToDowntime)
                        {
                            StationsToDowntimesTiesDbTable.Remove(tieFromDb);
                        }
                    }
                    else
                    {
                        if (station.IsTiedToDowntime)
                        {
                            StationsToDowntimesTiesDbTable.Add(new()
                            {
                                IdDowntime = tie.IdDowntime,
                                IdStation = station.Id
                            });
                        }
                    }
                }
            }
            await SaveChangesAsync();
        }

        public async Task PutNewDowntimeReason(DbDowntimeReason downtimeReason)
        {
            downtimeReason.WorkStatus = (int)WorkTimeStatus.Planned;
            if (downtimeReason.Id == -1)
            {
                downtimeReason.Id = 0;
                downtimeReason.Code = DowntimeReasonsDbTable
                    .Where(x => x.WorkStatus == (int)WorkTimeStatus.Planned)
                    .Max(x => x.Code) + 1;
                DowntimeReasonsDbTable.Add(downtimeReason);
            }
            else
            {
                DbDowntimeReason dbDowntimeReason = await DowntimeReasonsDbTable.SingleAsync(x => x.Id == downtimeReason.Id);
                dbDowntimeReason.Name = downtimeReason.Name;
                dbDowntimeReason.Description = downtimeReason.Description;
            }
            await SaveChangesAsync();
        }

        public async Task PutNewDowntimeHistory(DbDowntimeHistory downtimeHistory)
        {
            var rowsTochange = DownTimeHistoryDbTable.Where(x => x.Id == downtimeHistory.Id).ToList();
            foreach (var row in rowsTochange)
            {
                row.WorkStatus = downtimeHistory.WorkStatus;
                row.DowntimeReason = downtimeHistory.DowntimeReason;
                row.Comment = downtimeHistory.Comment;
            }
            await SaveChangesAsync();
        }

        public async Task PutDowntimeSchedule(List<OeeDowntimeScheduleDto> newDowntimeSchedule)
        {
            foreach (var schedule in newDowntimeSchedule)
            {
                if (schedule.Id == -1)
                {
                    DownTimeScheduleDbTable.Add(new()
                    {
                        Id = 0,
                        HourStart = TimeSpan.FromTicks(schedule.HourStartTicks),
                        HourStop = TimeSpan.FromTicks(schedule.HourStopTicks),
                        IdDowntimeReason = schedule.IdDowntimeReason,
                        IdStation = schedule.IdStation
                    });
                }
                else
                {
                    DbDowntimeSchedule dbDowntimeSchedule = await DownTimeScheduleDbTable.SingleAsync(x => x.Id == schedule.Id);
                    dbDowntimeSchedule.HourStart = TimeSpan.FromTicks(schedule.HourStartTicks);
                    dbDowntimeSchedule.HourStop = TimeSpan.FromTicks(schedule.HourStopTicks);
                    dbDowntimeSchedule.IdDowntimeReason = schedule.IdDowntimeReason;
                }
            }
            await SaveChangesAsync();
        }
        #endregion
        #endregion
    }
}
