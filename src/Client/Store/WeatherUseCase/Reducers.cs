using Fluxor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BDMT.Client.Store.WeatherUseCase
{
	public static class Reducers
	{
		[ReducerMethod]
		public static WeatherState ReduceFetchDataAction(WeatherState state, FetchDataAction action) =>
			new WeatherState(
				isLoading: true,
				errorMessage:null,
				forecasts: null);

		[ReducerMethod]
		public static WeatherState ReduceFetchDataResultAction(WeatherState state, FetchDataResultAction action) =>
			new WeatherState(
				isLoading: false,
				errorMessage: action.ErrorMessage,
				forecasts: action.Forecasts);
	}
}
