using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Remnant.Pipeline.Core;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Data
{
	/// <summary>
	/// The base class that must be derived from to setup a data container for a pipeline
	/// </summary>
	public class PipelineData : IPipelineData
	{
		/// <summary>
		/// The current stage within the puppet
		/// </summary>
		public IPipelineStage CurrentStage
		{
			get { return OnGetCurrentStage?.Invoke(); }
		}

		/// <summary>
		/// A read only collection of registered pipeline stages and events
		/// </summary>
		public ReadOnlyCollection<IPipelineStage> RegisteredStages
		{
			get { return OnGetStages?.Invoke(); }
		}

		/// <summary>
		/// A read only list of event listeners for observers
		/// </summary>
		public ReadOnlyCollection<EventListener> EventListeners
		{
			get { return OnGetEventListeners?.Invoke(); }
		}

		public int TotalStages
		{
			get { return RegisteredStages.Count; }
		}

		public int TotalEvents
		{
			get { return RegisteredStages.Sum(stage => stage.Events.Count); }
		}

		public int TotalObservers
		{
			get 
			{ 
				var observers = (from listener in EventListeners
												 from observer in listener.Observers
												 select observer).Distinct();

				return observers.Count();
			}
		}

		#region Events

		public event Func<IPipelineStage> OnGetCurrentStage;
		public event Func<ReadOnlyCollection<IPipelineStage>> OnGetStages;
		public event Func<ReadOnlyCollection<EventListener>> OnGetEventListeners;

		#endregion
	}
}