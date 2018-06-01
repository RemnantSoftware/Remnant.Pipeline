using System;
using System.Collections.Generic;
using System.Text;

namespace Remnant.Pipeline.Interfaces
{
	public interface IStages
	{
		/// <summary>
		/// Register a stage within the pipeline for events
		/// </summary>
		/// <typeparam name="TPipelineStage">The generic pipeline stage type</typeparam>
		/// <param name="name">Specify a name for the stage</param>
		/// <returns>Returns interface to continue registration</returns>
		IRegisterStage RegisterStage<TPipelineStage>(string name) where TPipelineStage : IPipelineStage, new();

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
