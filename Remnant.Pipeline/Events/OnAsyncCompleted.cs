using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnAsyncCompleted : AsyncEvent
	{		 
		public OnAsyncCompleted(IEvent @event, Observer observer)
		{
			Event = @event;
			Observer = observer;
		}

		public Observer Observer { get; private set; }

		public IEvent Event { get; private set; }

	}
}