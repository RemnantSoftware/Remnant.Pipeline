using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnCallObserver : AsyncEvent
	{
		public OnCallObserver(Observer obserber, IEvent @event)
		{
			Observer = obserber;
			Event = @event;
		}

		public Observer Observer { get; private set; }
		public IEvent Event { get; private set; }
	}
}