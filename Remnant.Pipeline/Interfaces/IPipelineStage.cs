using System.Collections.ObjectModel;

namespace Remnant.Pipeline.Interfaces
{

	public interface IPipelineStage 
	{
		/// <summary>
		/// Get or set the name of the pipeline stage
		/// </summary>
		string Name { get; set;  }

		/// <summary>
		/// Get reaonly collection of registered events for the stage
		/// </summary>
		ReadOnlyCollection<IEvent> Events { get; }

		/// <summary>
		/// Register an event for the pipeline stage
		/// </summary>
		/// <typeparam name="TEvent"></typeparam>
		/// <returns></returns>
		IPipelineStage ForEvent<TEvent>() where TEvent : IEvent, new();

		/// <summary>
		/// Register an event for the pipeline stage
		/// </summary>
		/// <param name="event"></param>
		/// <returns></returns>
		IPipelineStage ForEvent(IEvent @event);
	}
}
