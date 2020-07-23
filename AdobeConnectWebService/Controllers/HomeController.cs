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

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        [HttpPost(nameof(GetMeetings))]
        public IActionResult GetMeetings()
        {


           
        }
       
    }
}
