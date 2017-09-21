using System.Threading.Tasks;

namespace MedvindhemApi.Services
{
    public interface ISmhiService
    {
        Task<string> GetWeatherDataAsync();
    }
}
