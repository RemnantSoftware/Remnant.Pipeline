
namespace Remnant.Pipeline.Interfaces
{
	/// <summary>
	/// Interface to be used for observers to implement observing events
	/// </summary>
	/// <typeparam name="TEvent">The generic event type</typeparam>
	/// <typeparam name="TData">The generic data interface type</typeparam>
	public interface IObserve<in TEvent, in TData>
		where TEvent : IEvent		
		where TData : IPipelineData
	{
		/// <summary>
		/// The member will be called when event is raised
		/// </summary>
		/// <param name="event">The raised event that is observed</param>
		/// <param name="data">The data container for the pipeline</param>
		void OnEvent(TEvent @event, TData data);
	}
}