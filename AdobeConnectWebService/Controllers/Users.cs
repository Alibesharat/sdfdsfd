using AdobeConectApi.IO;
using AdobeConectApi.Service;
using AdobeConnectWebService.ApiViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdobeConnectWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {


        private readonly ILogger<HomeController> _logger;
        private readonly AdobeConnectService _ad;
        private readonly FileService _fs;
        IHostEnvironment _env;

        public UsersController(ILogger<HomeController> logger, AdobeConnectService ad,FileService fs,IHostEnvironment env)
        {
            _logger = logger;
            _ad = ad;
            _fs = fs;
            _env = env;
        }

        [HttpGet(nameof(Test))]
        public IActionResult Test()
        {

            var meetings = _fs.GetMeetings(_env.ContentRootPath);
            return Ok(meetings);
        }


        /// <summary>
        /// ذخیره یوزر در میتینگ
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(AddUser))]
        public IActionResult AddUser([FromBody] UserInfoViewModel userData)
        {
            var meetings = _fs.GetMeetings(_env.ContentRootPath);
            var data = _ad.AddUserDataToMeeting(userData,meetings);
            return Ok(data);
        }




    }
}
