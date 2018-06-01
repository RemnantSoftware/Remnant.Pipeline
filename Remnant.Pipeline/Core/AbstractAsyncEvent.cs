
using Remnant.Pipeline.Interfaces;
using System;

namespace Remnant.Pipeline.Core
{
    public abstract class AbstractAsyncEvent : IEvent
    {
			public void Complete(Observer observer)
			{
				if (OnCompleted != null)
					OnCompleted(this, observer);
			}

	    public event Action<IEvent, Observer> OnCompleted;
    }
}
