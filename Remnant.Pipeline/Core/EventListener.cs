using System.Collections.Generic;

namespace Remnant.Pipeline.Core
{
	public class EventListener
	{
		public EventListener(string eventName)
		{
			EventName = eventName;
			Observers = new List<Observer>();
		}

		public string EventName { get; private set; }
		public List<Observer> Observers { get; private set; } 
	}
}
