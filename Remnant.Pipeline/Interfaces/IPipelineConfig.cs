using Remnant.Pipeline.Core;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Remnant.Pipeline.Interfaces
{
	public interface IPipelineConfig
	{
		/// <summary>
		/// Specify a name for the pipeline
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		IPipelineConfig Name(string name);

		/// <summary>
		/// Configure stages for the pipeline
		/// </summary>
		/// <returns></returns>
		IStages Stages();

		/// <summary>
		/// Configure observers for the pipeline
		/// </summary>
		/// <returns></returns>
		IObservers Observers();

		/// <summary>
		/// If true the pipeline will automatically raise events in the sequence that are registered on start
		/// Note: The value is set to true by default
		/// </summary>
		/// <param name="value">True or false</param>
		/// <returns></returns>
		IPipelineConfig AutoRaiseEvents(bool value);

		/// <summary>
		/// Specify the data container for the pipeline
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		IPipelineConfig UseData(IPipelineData data);
	}
}
