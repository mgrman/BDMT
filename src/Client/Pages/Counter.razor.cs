﻿using BDMT.Client.Store.CounterStore;
using Fluxor;
using Microsoft.AspNetCore.Components;

namespace BDMT.Client.Pages
{
    public partial class Counter
    {
        [Inject]
        private IState<CounterState> CounterState { get; set; }

        [Inject]
        public IDispatcher Dispatcher { get; set; }

        private void IncrementCount()
        {
            var action = new IncrementCounterAction();
            Dispatcher.Dispatch(action);
        }
    }
}