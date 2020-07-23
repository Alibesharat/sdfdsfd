using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AdobeConectApi.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AdobeConnectWebService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {


        private readonly ILogger<HomeController> _logger;
        private readonly FileService _fs;

        public HomeController(ILogger<HomeController> logger, FileService fs)
        {
            _logger = logger;
            _fs = fs;
        }


        [HttpGet(nameof(GetMeetings))]
        public IActionResult GetMeetings()
        {
            var data = _fs.GetFiles();
            return Ok(data);

        }

    }
}
