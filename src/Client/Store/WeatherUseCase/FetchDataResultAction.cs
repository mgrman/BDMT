using BDMT.Shared;
using System.Collections.Generic;

namespace BDMT.Client.Store.WeatherUseCase
{
    public class FetchDataResultAction
    {
        public IEnumerable<WeatherForecast>? Forecasts { get; }
        public string? ErrorMessage { get; }

        public FetchDataResultAction(IEnumerable<WeatherForecast> forecasts)
        {
            Forecasts = forecasts;
            ErrorMessage = null;
        }

        public FetchDataResultAction(string errorMessage)
        {
            Forecasts = null;
            ErrorMessage = errorMessage;
        }
    }
}