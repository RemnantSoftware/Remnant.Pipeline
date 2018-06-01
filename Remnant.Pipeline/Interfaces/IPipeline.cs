
using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	/// <summary>
	/// The pipeline interface
	/// </summary>
	public interface IPipeline
	{
		/// <summary>
		/// Runs the pipeline synchrously (waits until all processing is completed)
		/// </summary>
		void Run();

		/// <summary>
		/// Runs the pipeline asynchrounous
		/// <param name="idleTime">Time in milliseconds for pipeline thread to idle (the default is 1 second)</param>
		/// </summary>
		void RunAsync(int idleTime = 1000);

		/// <summary>
		/// Stops the pipeline gracefully, all events wil be completed first
		/// </summary>
		void Stop();

		/// <summary>
		/// Aborts the pipeline ungracefully, current events will be discarded
		/// </summary>
		void Abort();

		/// <summary>
		/// Get the name of the pipeline
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Get the current pipeline state
		/// </summary>
		PipelineState State { get; }

		/// <summary>
		/// Get if pipeline is set to automatically raise events 
		/// </summary>
		bool AutoRaiseEvents { get; }

		/// <summary>
		/// Connect one pipeline with another pipeline
		/// </summary>
		/// <param name="pipeline">The pipeline to connect</param>
		/// <returns>Returns the pipeline connector</returns>
		IConnector Connect(IPipeline pipeline);

		/// <summary>
		/// Manually raise an event for the pipeline using event type
		/// <param name="stage">Optional: specify the stage for the event, otherwise the current stage will be used</param>
		/// </summary>
		/// <typeparam name="TEvent">The generic event type</typeparam>
		void RaiseEvent<TEvent>(string stage) where TEvent : IEvent;

		/// <summary>
		/// Manually raise an event for the pipeline using event type
		/// <param name="stage">Specify the stage for the event</param>
		/// <param name="event">The event instance</param>
		/// </summary>
		void RaiseEvent(string stage, IEvent @event);

		/// <summary>
		/// Get or set the pipeline configuration
		/// </summary>
		IPipelineConfig Configuration { get; set; }

	}
}
