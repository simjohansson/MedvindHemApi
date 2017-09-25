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
        private readonly int RIGHT_ANGLE_TAIL_WIND = 45;
        private readonly int RIGHT_ANGLE_HEAD_WIND = 135;
        private readonly int LEFT_ANGLE_HEAD_WIND = 225;
        private readonly int LEFT_ANGLE_TAIL_WIND = 315;
        private static double _currentBearing;
        public SmhiService(IMemoryCache cache, IOptions<SmhiOptions> smhiOptions)
        {
            _cache = cache;
            _smhiOptions = smhiOptions.Value;
        }

        public async Task<string> GetWeatherDataAsync(DirectionInput directionInput)
        {
            var halfwayPoint = new Coordinate
            {
                Latitude = (directionInput.FromCoordinate.Latitude + directionInput.ToCoordinate.Latitude) / 2,
                Longitude = (directionInput.FromCoordinate.Longitude + directionInput.ToCoordinate.Longitude) / 2,
            };

            _currentBearing = DegreeBearing(directionInput);

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

            var nearestStation = stationDtos.OrderBy(x => DistanceToStation(x, halfwayPoint)).First(t => t.WindDirection != null && t.WindSpeed != null);
            var result = string.Empty;
            var windDirection = (Int32.Parse(nearestStation.WindDirection) + 180) % 360;

            if (DegreeDistance(windDirection, SumDegrees(LEFT_ANGLE_TAIL_WIND)) <= 90 && DegreeDistance(windDirection, SumDegrees(RIGHT_ANGLE_TAIL_WIND)) <= 90)
            {
                result = "Tail wind! :)";
            }
            else if (DegreeDistance(windDirection, SumDegrees(RIGHT_ANGLE_HEAD_WIND)) <= 90 && DegreeDistance(windDirection, SumDegrees(LEFT_ANGLE_HEAD_WIND)) <= 90)
            {
                result = "Head wind! ;(";
            }
            else
            {
                result = "Side wind!";
            }

            return JsonConvert.SerializeObject(value: result);
        }

        private int DegreeDistance(int alpha, int beta)
        {
            int phi = Math.Abs(beta - alpha) % 360;
            int distance = phi > 180 ? 360 - phi : phi;
            return distance;
        }


        private int SumDegrees(int degree)
        {
            return (degree + (int)_currentBearing) % 360;
        }

        private static double DistanceToStation(StationDto station, Coordinate halfwayPoint)
        {
            return Math.Sqrt((station.Coordinate.Latitude - halfwayPoint.Latitude) * (station.Coordinate.Latitude - halfwayPoint.Latitude) + (station.Coordinate.Longitude - halfwayPoint.Longitude) * (station.Coordinate.Longitude - halfwayPoint.Longitude));
        }

        private static double DegreeBearing(DirectionInput directionInput)
        {
            var dLon = ToRad(directionInput.ToCoordinate.Longitude - directionInput.FromCoordinate.Longitude);
            var dPhi = Math.Log(Math.Tan(ToRad(directionInput.ToCoordinate.Latitude) / 2 + Math.PI / 4) / Math.Tan(ToRad(directionInput.FromCoordinate.Latitude) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
            {
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            }
            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        private static double ToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        private static double ToBearing(double radians)
        {
            return (ToDegrees(radians) + 360) % 360;
        }
    }
}
