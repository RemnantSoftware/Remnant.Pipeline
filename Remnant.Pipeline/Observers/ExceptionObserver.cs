using Remnant.Pipeline.Core;
using Remnant.Pipeline.Events;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Observers
{
	public class ExceptionObserver : Observer,
		IObserve<OnException, IPipelineData>
	{
		public virtual void OnEvent(OnException @event, IPipelineData data)
		{
			//throw @event.Exception;
			RaiseEvent(new OnAbortPipeline
			{
				Exception = @event.Exception
			});
		}
	}
}