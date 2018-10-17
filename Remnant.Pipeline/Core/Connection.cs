using System;
using System.Collections.Generic;
using System.Linq;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	internal class Connection
	{
		private IPipelineStage _currentstage;
		private readonly List<Type> _eventTypes;

		public Connection(IPipeline pipeline)
		{
			Pipeline = pipeline;
			_eventTypes = new List<Type>();
		}

		public IPipeline Pipeline { get; private set; }

		public void RaiseEvent(IPipelineStage stage, IEvent @event)
		{
			if (IsEventRegistered(stage, @event))
			{
				_currentstage = Pipeline.Configuration.Stages().FindStage(stage.Name);

				if (_currentstage.FindEvent(@event.GetType()) == null)
				{
					_currentstage.ForEvent(@event);
				}

				Pipeline.RaiseEvent(_currentstage.Name, @event);
			}
		}

		public void ForEvent<TEvent>() where TEvent : IEvent
		{
			_eventTypes.Add(typeof(TEvent));
		}

		public void ForStage(string stage)
		{
			Pipeline.Configuration.Stages().RegisterStage(stage);

			_currentstage = Pipeline.Configuration.Stages().FindStage(stage);
		}

		public bool IsEventRegistered(IPipelineStage stage, IEvent @event)
		{
			return Pipeline.Configuration.Stages().FindStage(stage.Name) != null &&
			_eventTypes.Exists(e => e.FullName.Equals(@event.GetType().FullName, StringComparison.InvariantCultureIgnoreCase));
		}
	}
}