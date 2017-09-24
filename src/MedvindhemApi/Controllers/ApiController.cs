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
        
        [HttpPost, Route("setroute")]
        public async Task<string> GetWind([FromBody] DirectionInput directionInput)
        {
            return await _smhiService.GetWeatherDataAsync(directionInput);
        }      
    }
}
