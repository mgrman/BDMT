using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using BDMT.Shared;

namespace BDMT.Shared
{
    [ServiceContract]
    [AuthorizeInterface]
    public interface IWeatherService
    {
        Task<IEnumerable<WeatherForecast>> GetWeatherForecasts();
    }
}
