using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BDMT.Shared
{
    public class WeatherService : IWeatherService
    {
        public WeatherService(string aaa, int bbb)
        {
            Aaa = aaa;
            Bbb = bbb;
        }

        public string Aaa { get; }
        public int Bbb { get; }

        public Task<IEnumerable<string>> GetWeatherForecasts()
        {
            throw new NotImplementedException();
        }
    }
}