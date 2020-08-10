using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Threading.Tasks;
using BDMT.Shared;
using Microsoft.AspNetCore.Authorization;

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
