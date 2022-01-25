using AmiMesApi.DataBase;
using AmiMesApi.Model;
using AmiMesApi.Model.Oee;
using AmiMesApi.Services;
using API_Standard.DataBase.Base;
using API_Standard.Models.Base.Structure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IAuthorizationService = AmiMesApi.Services.IAuthorizationService;

namespace AmiMesApi.Controllers
{
    [Route("oee/plcs7driver")]
    public class OeePlcS7DriverController : Controller
    {
        private readonly OeeDbContext dbContext;
        private readonly IAppInfo appInfo;
        private readonly IAuthorizationService authorizationService;
        public OeePlcS7DriverController(OeeDbContext dbContext, IAppInfo appInfo, IAuthorizationService authorizationService)
        {
            this.dbContext = dbContext;
            this.appInfo = appInfo;
            PlcConnectionStatuses.CreatePlcConnectionsList(dbContext);
            this.authorizationService = authorizationService;
        }

        #region PUT

        [HttpPut]
        [Route("PutPartResult")]
        public async Task<IActionResult> PutPartResult([FromBody] DbProductionPartHistory partResult)
        {
            try
            {
                dbContext.ProductionPartsHistoryDbTable.Add(partResult);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("PutDowntime")]
        public async Task<IActionResult> PutDowntime([FromBody] DbDowntimeHistory downtime)
        {
            try
            {
                await dbContext.PutDowntime(downtime);
                return Ok();
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
