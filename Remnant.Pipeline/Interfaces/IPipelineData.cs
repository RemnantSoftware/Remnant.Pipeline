using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remnant.Pipeline.Core;

namespace Remnant.Pipeline.Interfaces
{
	/// <summary>
	/// Derive from this interface for data management within the pipeline.
	/// The interface also provides core data information of the pipeline.
	/// </summary>
	public interface IPipelineData
	{
		/// <summary>
		/// The current stage within the pipeline
		/// </summary>
		IPipelineStage CurrentStage { get; }

		/// <summary>
		/// A read only collection of registered pipeline stages and events
		/// </summary>
		ReadOnlyCollection<IPipelineStage> RegisteredStages { get; }

		/// <summary>
		/// A read only collection of event listeners for observers
		/// </summary>
		ReadOnlyCollection<EventListener> EventListeners { get; }

		/// <summary>
		/// Get the total amount of stages for the pipeline
		/// </summary>
		int TotalStages { get; }

		/// <summary>
		/// Get the total amount of events that are registered
		/// </summary>
		int TotalEvents { get; }

		/// <summary>
		/// Get the total amount of observers that are registered
		/// </summary>
		int TotalObservers { get; }

		event Func<IPipelineStage> OnGetCurrentStage;

		event Func<ReadOnlyCollection<IPipelineStage>> OnGetStages;

		event Func<ReadOnlyCollection<EventListener>> OnGetEventListeners;
	}
}