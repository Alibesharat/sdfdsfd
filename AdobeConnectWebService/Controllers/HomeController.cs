using AdobeConectApi.IO;
using AdobeConectApi.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AdobeConnectWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {


        private readonly ILogger<HomeController> _logger;
        private readonly FileService _fs;
        private readonly AdobeConnectService _ad;
        IHostEnvironment _env;


        public HomeController(ILogger<HomeController> logger, FileService fs, AdobeConnectService ad, IHostEnvironment env)

        {
            _logger = logger;
            _fs = fs;
            _ad = ad;
            _env=env;
        }

        /// <summary>
        /// ذخیره لیست میتینگ ها در سرورها
        /// </summary>
        /// <returns></returns>
        [HttpGet(nameof(AddGroups))]
        public IActionResult AddGroups()
        {
            var data = _fs.GetMeetings(_env.ContentRootPath);
            var res = _ad.AddMetingsToServers(data);
            return Ok(res);
        }




    }
}
