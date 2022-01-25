using AmiMesApi.DataBase;
using AmiMesApi.Helpers;
using AmiMesApi.Model;
using AmiMesApi.Model.Oee;
using AmiMesApi.Services;
using API_AmiOEE.Models.DTO.Models;
using API_AmiOrder.Model;
using API_Standard.Models.Base.Structure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using IAuthorizationService = AmiMesApi.Services.IAuthorizationService;

namespace AmiMesApi.Controllers
{
    [Route("oee/visualization")]
    public class OeeVisualizationController : Controller
    {
        private readonly OeeDbContext dbContext;
        private readonly IAppInfo appInfo;
        private readonly IAuthorizationService authorizationService;
        private readonly IProductionShiftService productionShiftService;
        public OeeVisualizationController(OeeDbContext dbContext, IAppInfo appInfo, IAuthorizationService authorizationService, IProductionShiftService productionShiftService)
        {
            this.dbContext = dbContext;
            this.appInfo = appInfo;
            this.authorizationService = authorizationService;
            this.productionShiftService = productionShiftService;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region GET
        [HttpGet]
        [Route("CheckConnection")]
        public IActionResult CheckConnection(int userId)
        {
            return Ok();
        }

        [HttpGet]
        [Route("GetProductionStructure")]
        public IActionResult GetProductionStructure()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                List<Area> areas = dbContext.GetProductionStructureForUser(userId);
                List<Entity> entities = areas.SelectMany(x => x.Entities).ToList();
                ProductionStructureDto productionStructure = new()
                {
                    Areas = areas,
                    Stations = entities.SelectMany(x => x.Stations).ToList(),
                    ProductionShifts = productionShiftService.GetProductionShifts()
                };
                return Ok(productionStructure);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest();
            }
        }

        #region Get OEE Result
        [HttpGet]
        [Route("GetOeeResult")]
        public IActionResult GetOeeResult()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                OeeResult oeeResult = dbContext.GetOeePack(userId);
                return Ok(oeeResult);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest();
            }
        }

        private int idArea;
        private int idStation;

        private void CreateShiftDbData(int idStation)
        {
            this.idStation = idStation;
            idArea = dbContext.Stations.ToList().Find(x => x.Id == idStation).IdArea;
        }

        private OeeRating CreateOeeRating(DbVariant variant = null)
        {
            try
            {
                int variantCode = variant != null ? variant.Code : -1;
                OeeRating oeeRating = dbContext.OeeRatings.FromSqlRaw<OeeRating>($"GetOeeResult {idStation}, {variantCode}").ToList().First();
                oeeRating.IdArea = idArea;
                oeeRating.IdStation = idStation;
                oeeRating.Variant = variant != null ? variant.Code : -1;
                return oeeRating;
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return null;
            }
        }

        private List<OeeWorkTime> CreateOeeWorkTimes()
        {
            try
            {
                List<OeeWorkTime> oeeWorktimes = dbContext.OeeWorkTimes.FromSqlRaw<OeeWorkTime>($"GetOeeShiftWorkTime {idStation}").ToList();
                return oeeWorktimes;
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return null;
            }
        }

        private List<OeeHourSummary> CreateOeeHourSummary(DbVariant variant = null)
        {
            try
            {
                int variantCode = variant != null ? variant.Code : -1;
                List<OeeHourSummary> oeeHourSummaries = dbContext.OeeHourSummary.FromSqlRaw<OeeHourSummary>($"GetOeeSummary {idStation}, {variantCode}").ToList();
                return oeeHourSummaries;
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return null;
            }
        }
        #endregion

        [HttpGet]
        [Route("GetOeeRatingsFromDate/{fromDateTicks}/{toDateTicks}")]
        public IActionResult GetOeeRatingsFromDate(long fromDateTicks, long toDateTicks)
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            DateTime fromDate = new(fromDateTicks);
            DateTime toDate = new(toDateTicks);
            List<OeeRating> oeeRatingsCollection = new();
            try
            {
                foreach (var stationTie in dbContext.StationsToUsersTies.Where(x => x.IdUser == userId).ToList())
                {
                    CreateShiftDbData(stationTie.IdStation);
                    oeeRatingsCollection.Add(CreateOeeRatingFromDate(fromDate, toDate));
                    foreach (var variant in dbContext.Variants.Where(x => x.IdStation == stationTie.IdStation).ToList())
                    {
                        oeeRatingsCollection.Add(CreateOeeRatingFromDate(fromDate, toDate, variant));
                    }
                }
                return Ok(oeeRatingsCollection);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }
        private OeeRating CreateOeeRatingFromDate(DateTime fromDate, DateTime toDate, DbVariant variant = null)
        {
            try
            {
                int variantCode = variant != null ? variant.Code : -1;
                string query = $"GetOeeResultFromDate {idStation}, {variantCode}, \'{fromDate:yyyy-MM-dd HH:mm:ss}\',  \'{toDate:yyyy-MM-dd HH:mm:ss}\'";
                OeeRating oeeRating = dbContext.OeeRatings.FromSqlRaw<OeeRating>(query).ToList().First();
                oeeRating.IdArea = idArea;
                oeeRating.IdStation = idStation;
                oeeRating.Variant = variant != null ? variant.Code : -1;
                return oeeRating;
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return null;
            }
        }

        #endregion
    }
}
