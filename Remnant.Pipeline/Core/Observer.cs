
using Remnant.Pipeline.Interfaces;
using System;

namespace Remnant.Pipeline.Core
{
	public class Observer
	{
		public void RaiseEvent<TEvent>()
				where TEvent : IEvent, new()
		{
			OnRaiseEvent?.Invoke(new TEvent());
		}

		public void RaiseEvent<TEvent>(TEvent @event)
				where TEvent : IEvent
		{
			OnRaiseEvent?.Invoke(@event);
		}

		public event Action<IEvent> OnRaiseEvent;
	}
}
