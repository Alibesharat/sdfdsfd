using AdobeConectApi.IO;
using AdobeConectApi.Service;
using AdobeConnectWebService.ApiViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AdobeConnectWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {


        private readonly ILogger<HomeController> _logger;
        private readonly AdobeConnectService _ad;

        public UsersController(ILogger<HomeController> logger, AdobeConnectService ad)
        {
            _logger = logger;
            _ad = ad;
        }

        /// <summary>
        /// ذخیره لیست میتینگ ها در سرورها
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(AddUser))]
        public IActionResult AddUser([FromBody] UserDataViewModel userData)
        {
            var data = _ad.AddUserDataToGroups(userData);
            return Ok(data);
        }




    }
}
