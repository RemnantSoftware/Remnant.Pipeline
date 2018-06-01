namespace Remnant.Pipeline.Core
{
	public class AsyncEventTracker
	{
		public AsyncEventTracker(AbstractAsyncEvent @event,  Observer observer)
		{
			Event = @event;
			SubscribeObserver = observer;
		}

		public Observer SubscribeObserver { get; set; }
		public AbstractAsyncEvent Event { get; set; }
	}
}