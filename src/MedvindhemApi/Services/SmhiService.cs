using MedvindhemApi.API_Interfaces;
using MedvindhemApi.Models;
using MedvindhemApi.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MedvindhemApi.Services
{
    public class SmhiService : ISmhiService
    {
        private IMemoryCache _cache;
        private readonly SmhiOptions _smhiOptions;
        private static HttpClient client = new HttpClient();

        public SmhiService(IMemoryCache cache, IOptions<SmhiOptions> smhiOptions)
        {
            _cache = cache;
            _smhiOptions = smhiOptions.Value;
        }

        public async Task<string> GetWeatherDataAsync()
        {
            if (!_cache.TryGetValue("stationer", out List<StationDto> stationDtos))
            {
                // Key not in cache, so get data.               
                var smhiApi = RestService.For<ISmhiAPI>(_smhiOptions.SmhiUrl);
                var windDirectionWeatherData = await smhiApi.GetWeather(_smhiOptions.WindDirectionType);
                var windSpeedWeatherData = await smhiApi.GetWeather(_smhiOptions.WindSpeedType);

                stationDtos = windDirectionWeatherData.Stations.Select(x => new StationDto
                {
                    Key = x.Key,
                    Coordinate = new Coordinate
                    {
                        Latitude = x.Latitude,
                        Longitude = x.Longitude
                    },
                    Name = x.Name,
                    WindDirection = x.Value?.First().WeatherValue,
                    WindSpeed = windSpeedWeatherData.Stations.First(t => t.Key == x.Key).Value?.First().WeatherValue
                }).ToList();

                // Set cache options.
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(30));

                // Save data in cache.
                _cache.Set("stationer", stationDtos, cacheEntryOptions);
            }
            return JsonConvert.SerializeObject(value: stationDtos);
        }
    }
}
