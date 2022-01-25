using AmiMesApi.DataBase;
using AmiMesApi.Model;
using AmiMesApi.Services;
using API_AmiOrder.DataBase;
using API_Standard.DataBase.Base;
using API_Standard.Models.Base.Structure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AmiMesApi.Controllers.Base
{
    [Route("plcs7driver")]
    public class BasePlcS7DriverController : Controller
    {
        private readonly AmiMesSystemDbContext dbContext;
        private readonly IAppInfo appInfo;
        private readonly IAuthorizationService authorizationService;

        public BasePlcS7DriverController(AmiMesSystemDbContext dbContext, IAppInfo appInfo, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.appInfo = appInfo;
            this.authorizationService = authorizationService;
        }

        [HttpGet]
        [Route("GetPlcAdresses")]
        public IActionResult GetPlcAddresses()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                List<PlcS7Connection> dbPlcAddresses = dbContext.GetPlcAdresses(userId);
                return Ok(dbPlcAddresses);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }



        [HttpGet]
        [Route("GetCommunicationStandards")]
        public IActionResult GetCommunicationStandards()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                List<DbCommunicationStandard> dbCommunicationStandards = dbContext.GetCommunicationStandards(userId);
                return Ok(dbCommunicationStandards);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("CheckPlcConnection")]
        public IActionResult CheckPlcConnection()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                List<int> stationsIds = dbContext.GetStationIdsForUser(userId);
                //Zmiana z channelami
                List<int> plcIds = default;//dbContext.MesPoints.Where(x => stationsIds.Contains(x.IdChannel.Value)).Select(x => x.IdPlcAddress).Distinct().ToList();
                bool isConnection = true;
                foreach (var plc in PlcConnectionStatuses.PlcConnections.FindAll(x => plcIds.Contains(x.Id)))
                {
                    if (!plc.IsConnected)
                    {
                        isConnection = false;
                    }
                    plc.IsConnected = false;
                }
                return Ok(isConnection);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("GetMesPoints")]
        public IActionResult GetMesPoints()
        {
            int userId = authorizationService.GetUserIdFromRequest(Request);
            try
            {
                List<int> availlableStationsId = dbContext.GetStationIdsForUser(userId);
                //Zmiana z channelami
                List<DbMesPointS7> dbMesPoints = dbContext.MesPoints.Where(x => availlableStationsId.Contains(x.IdChannel)).ToList();
                return Ok(dbMesPoints);
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }


        [HttpPut]
        [Route("PutConnectionStatus")]
        public IActionResult PutConnectionStatus([FromBody] PlcInfo plcInfo)
        {
            try
            {
                PlcConnectionStatuses.PlcConnections.Find(x => x.Id == plcInfo.Id).IsConnected = plcInfo.IsConnected;
                return Ok();
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }
    }
}
