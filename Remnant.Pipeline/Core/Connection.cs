using System;
using System.Collections.Generic;
using System.Linq;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	internal class Connection
	{
		private readonly List<string> _eventPatterns;
		private IPipelineStage _stage;
		

		public Connection(IPipeline pipeline)
		{
			Pipeline = pipeline;
			_eventPatterns = new List<string>();
		}

		public IPipeline Pipeline { get; private set; }

		public void RaiseEvent(IEvent @event)
		{
			var stages = (Pipeline.Configuration as IPipelineConfigAccess).Stages.Where(s => s.Events.Any(e => e.GetType().Equals(@event.GetType())));

			foreach (var stage in stages)
			{
				Pipeline.RaiseEvent(stage.Name, @event);
			}
		}

		public void ForEvent<TEvent>() where TEvent : IEvent
		{
			_eventPatterns.Add(typeof(TEvent).FullName);
		}

		public void ForEvent(string eventNamePattern)
		{
			_eventPatterns.Add(eventNamePattern);
		}

		public bool IsEventRegistered(IEvent @event)
		{
			var patterns = from eventPattern in _eventPatterns
										 where @event.GetType().FullName.StartsWith(eventPattern, StringComparison.OrdinalIgnoreCase)
										 select eventPattern;

			return (patterns.Any());
		}

		public IPipelineStage Stage
		{
			get { return _stage; }
			set
			{
				_stage = value;
				Pipeline.Configuration.Stages().RegisterStage(_stage.Name);
			}
		}
	}
}