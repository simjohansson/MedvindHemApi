using MedvindhemApi.Models;
using System.Threading.Tasks;

namespace MedvindhemApi.Services
{
    public interface ISmhiService
    {
        Task<string> GetWeatherDataAsync(DirectionInput directionInput);
    }
}
