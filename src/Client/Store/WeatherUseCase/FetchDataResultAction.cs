using BDMT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		public FetchDataResultAction( string errorMessage)
		{
			Forecasts = null;
			ErrorMessage = errorMessage;
		}
	}
}
