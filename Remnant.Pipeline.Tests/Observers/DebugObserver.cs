using Remnant.Pipeline.Core;
using Remnant.Pipeline.Events;
using Remnant.Pipeline.Interfaces;
using Remnant.Pipeline.Tests.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remnant.Pipeline.Tests.Observers
{
	public class DebugObserver : Observer, 
		IObserve<OnEventRaised, IPipelineData>
	{
		public void OnEvent(OnEventRaised @event, IPipelineData data)
		{
			OnDebugEvent?.Invoke(@event.Event);
			@event.Complete(this);
		}

		public event Action<IEvent> OnDebugEvent;
	}
}
