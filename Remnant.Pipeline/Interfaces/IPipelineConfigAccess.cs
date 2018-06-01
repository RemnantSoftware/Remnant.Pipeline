using Remnant.Pipeline.Core;
using System.Collections.ObjectModel;

namespace Remnant.Pipeline.Interfaces
{
	public interface IPipelineConfigAccess
	{
		/// <summary>
		/// Get a readonly collection of registered stage
		/// </summary>
		ReadOnlyCollection<IPipelineStage> Stages { get; }

		/// <summary>
		/// Get a readnly collection of registered observers
		/// </summary>
		ReadOnlyCollection<Observer> Observers { get; }

		/// <summary>
		/// Get or set the pipeline data container for the pipeline
		/// </summary>
		IPipelineData Data { get; set; }

		/// <summary>
		/// Get or set the name of the pipeline
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Get or set if events must automatically be raised when pipeline starts
		/// </summary>
		bool AutoRaiseEvents { get; set; }
	}
}
