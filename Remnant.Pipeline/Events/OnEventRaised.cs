using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Events
{
	public class OnEventRaised : AsyncEvent
	{
		public OnEventRaised(IEvent @event)
		{
			Event = @event;
		}

		public IEvent Event { get; private set; }
	}
}