using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDMT.Shared
{
    public static class Constants
    {
        public const string Role = "BBB";
    }

    [AuthorizeInterface(Roles = Constants.Role)]
    public interface IWeatherService
    {
        Task<IEnumerable<string>> GetWeatherForecasts();
    }
}