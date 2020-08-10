﻿using BDMT.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMT.Client.Store.WeatherUseCase
{
	public class WeatherState
	{
		public bool IsLoading { get; }
		public string? ErrorMessage { get; }
		public IEnumerable<WeatherForecast>? Forecasts { get; }

		public WeatherState(bool isLoading,string? errorMessage, IEnumerable<WeatherForecast>? forecasts)
		{
			IsLoading = isLoading;
            ErrorMessage = errorMessage;
            Forecasts = forecasts ?? Array.Empty<WeatherForecast>();
		}
	}
}
