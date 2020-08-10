namespace BDMT.Client.Store.CounterStore
{
    public class CounterState
    {
        public int ClickCount { get; }

        public CounterState(int clickCount)
        {
            ClickCount = clickCount;
        }
    }
}