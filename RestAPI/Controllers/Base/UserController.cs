using AmiMesApi.DataBase;
using AmiMesApi.Model;
using AmiMesApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IAuthorizationService = AmiMesApi.Services.IAuthorizationService;

namespace AmiMesApi.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {
        private IAuthorizationService authorizationService;
        private readonly IAppInfo appInfo;
        public UserController(IAppInfo appInfo, IAuthorizationService authorizationService)
        {
            this.appInfo = appInfo;
            this.authorizationService = authorizationService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPut]
        [Route("LoginIntoMES")]
        public async Task<IActionResult> LogIntoMES([FromBody] DbMesUser tryingToLogUser)
        {
            if (tryingToLogUser == null)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(tryingToLogUser.Login))
            {
                return Unauthorized("Proszę podać login."); 
            }

            if (string.IsNullOrEmpty(tryingToLogUser.Password))
            {
                return Unauthorized("Proszę podać hasło.");
            }


            try
            {
                OperationResult<DbMesUser> result = await authorizationService.AuthorizeUser(tryingToLogUser.Login, tryingToLogUser.Password, tryingToLogUser.IsPasswordHashed);
                if (result.Status == OperationStatus.Success)
                {
                    return Ok(result.Result);
                }
                else if (result.Status == OperationStatus.WrongLogin)
                {
                    return Unauthorized("Niepoprawny login.");
                }
                else if (result.Status == OperationStatus.WrongPassword)
                {
                    return Unauthorized("Niepoprawne hasło.");
                }
                else
                {
                    return BadRequest("Logowanie zakończone niepowodzeniem.");
                }
            }
            catch (Exception ex)
            {
                appInfo.CreateAppError(ex, new());
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        [Route("LogOutFromMES")]
        public async Task<IActionResult> LogOutFromMES([FromBody] string token)
        {
            bool isSucccess = await authorizationService.UnAuthorizeUserByToken(token);
            if (isSucccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpGet]
        [Route("LogOutMES")]
        public async Task<IActionResult> LogOutMES()
        {
            string token = authorizationService.GetTokenFromRequest(Request);
            bool isSucccess = await authorizationService.UnAuthorizeUserByToken(token);
            if (isSucccess)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
