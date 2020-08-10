using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace BDMT.Shared
{
    [ServiceContract]
    [AuthorizeInterface]
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecasts();
    }
}