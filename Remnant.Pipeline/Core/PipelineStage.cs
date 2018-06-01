
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	public class PipelineStage : IPipelineStage
	{
		private readonly List<IEvent> _events = new List<IEvent>();

		public PipelineStage()
		{
			Name = GetType().FullName;
		}

		public PipelineStage(string name)
			: this()
		{
			Name = name;
		}

		public string Name { get; set; }

		public IPipelineStage ForEvent<TEvent>() where TEvent : IEvent, new()
		{
			_events.Add(new TEvent());
			return this;
		}

		public IPipelineStage ForEvent(string eventName, object argument)
		{
			_events.Add(new AnonymousEvent<object>() {Object = argument});
			return this;
		}

		public IPipelineStage ForEvent(IEvent @event)
		{
			_events.Add(@event);
			return this;
		}

		public IEvent FindEvent<TEvent>() where TEvent : IEvent
		{
			return FindEvent(typeof (TEvent));
		}

		public IEvent FindEvent(Type @eventType)
		{
			return _events.Find(e => e.GetType() == @eventType);
		}

		public ReadOnlyCollection<IEvent> Events 
		{
			get { return new ReadOnlyCollection<IEvent>(_events); }
		}
	}
}
