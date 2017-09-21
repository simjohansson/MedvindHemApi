using MedvindhemApi.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MedvindhemApi.API_Interfaces
{
    public interface ISmhiAPI
    {
        [Get("/{weatherType}/station-set/all/period/latest-hour/data.json")]
        Task <WeatherData> GetWeather(int weatherType);
    }
}
