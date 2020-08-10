using Fluxor;

namespace BDMT.Client.Store.CounterStore
{
    public class Feature : Feature<CounterState>
    {
        public override string GetName() => "Counter";

        protected override CounterState GetInitialState() =>
            new CounterState(clickCount: 0);
    }
}