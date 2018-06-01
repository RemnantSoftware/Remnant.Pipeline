using Remnant.Pipeline.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Remnant.Pipeline.Interfaces
{
	public interface IObservers
	{
		/// <summary>
		/// Register an observer for the pipeline
		/// </summary>
		/// <typeparam name="TObserver">The generic observer type</typeparam>
		/// <returns>Returns interface to continue registration</returns>
		IObservers RegisterObserver<TObserver>() where TObserver : Observer, new();

		/// <summary>
		/// Register an observer for the pipeline
		/// </summary>
		/// <returns>Returns interface to continue registration</returns>
		IObservers RegisterObserver(Observer observer);

		/// <summary>
		/// Configure stages
		/// </summary>
		/// <returns></returns>
		IStages Stages();
	}
}
