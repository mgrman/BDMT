using BDMT.Shared;
using Fluxor;
using System;
using System.Threading.Tasks;

namespace BDMT.Client.Store.WeatherUseCase
{
    public class FetchDataActionEffect : Effect<FetchDataAction>
    {
        private readonly IWeatherService weatherService;

        public FetchDataActionEffect(IWeatherService weatherService)
        {
            this.weatherService = weatherService;
        }

        protected override async Task HandleAsync(FetchDataAction action, IDispatcher dispatcher)
        {
            try
            {
                var forecasts = await weatherService.GetWeatherForecasts();
                dispatcher.Dispatch(new FetchDataResultAction(forecasts));
            }
            catch (Exception ex)
            {
                dispatcher.Dispatch(new FetchDataResultAction(ex.Message));
            }
        }
    }
}