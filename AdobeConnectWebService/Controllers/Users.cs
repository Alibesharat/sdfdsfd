using AdobeConectApi.IO;
using AdobeConectApi.Service;
using AdobeConnectWebService.ApiViewModels;
using AdobeConnectWebService.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Linq;

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

        public UsersController(ILogger<HomeController> logger, AdobeConnectService ad, FileService fs, IHostEnvironment env)
        {
            _logger = logger;
            _ad = ad;
            _fs = fs;
            _env = env;
        }




        /// <summary>
        /// ذخیره یوزر در میتینگ
        /// </summary>
        /// <returns></returns>
        [HttpPost(nameof(AddUser))]
        public IActionResult AddUser([FromBody] UserInfoViewModel userData)
        {
            var meetings = _fs.GetMeetings(_env.ContentRootPath);
            var data = _ad.AddUserDataToMeeting(userData, meetings);
            _fs.WriteFileInJson(data, _env.ContentRootPath, data.IsSucess);
            return Ok(data);
        }



        /// <summary>
        /// دریافت آدرس کلاس کاربر
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(FindClass))]
        public IActionResult FindClass(string UserName)
        {
            var data = _fs.GetUsers(_env.ContentRootPath);
            var infos = data.Where(c => c.UserName == UserName).Select(c => new AddressViewmodels() { Date = c.Date, Url = c.url, }).ToList();
            return Ok(infos);
        }




    }
}
