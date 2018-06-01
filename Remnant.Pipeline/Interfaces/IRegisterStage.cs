
using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	public interface IRegisterStage
	{
		/// <summary>
		/// Register event for the pipeline stage
		/// </summary>
		/// <typeparam name="TEvent">The generic event type</typeparam>
		/// <returns>Returns interface to continue registration</returns>
		IRegisterStage ForEvent<TEvent>() where TEvent : IEvent, new();

		/// <summary>
		/// Register initial event for the pipeline stage
		/// </summary>
		/// <returns>Returns interface to continue registration</returns>
		IRegisterStage ForEvent(IEvent @event);

		/// <summary>
		/// Register a stage within the pipeline for events
		/// </summary>
		/// <typeparam name="TPipelineStage">The generic pipeline stage type</typeparam>
		/// <param name="name">Specify a name for the stage</param>
		/// <returns>Returns interface to continue registration</returns>
		IRegisterStage RegisterStage<TPipelineStage>(string name) 
			where TPipelineStage : IPipelineStage, new();

		/// <summary>
		/// Register a stage within the pipeline for events
		/// </summary>
		/// <param name="name">>Specify a name for the stage</param>
		/// <returns>Returns interface to continue registration</returns>
		IRegisterStage RegisterStage(string name);

		/// <summary>
		/// Configure observers
		/// </summary>
		/// <returns></returns>
		IObservers Observers();
	}
}
