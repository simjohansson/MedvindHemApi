using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedvindhemApi.Models;
using MedvindhemApi.Services;

namespace MedvindhemApi.Controllers
{
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private ISmhiService _smhiService;

        public ApiController(ISmhiService smhiService)
        {
            _smhiService = smhiService;
        }
        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpPost, Route("setroute")]
        public async Task<string> SetRoute([FromBody] Coordinate[] routeCoordinates)
        {
            return await _smhiService.GetWeatherDataAsync();
        }
      
    }
}
