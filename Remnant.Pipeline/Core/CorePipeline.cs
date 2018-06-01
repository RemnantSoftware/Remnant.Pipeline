
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using log4net;
using Remnant.Pipeline.Events;
using Remnant.Pipeline.Interfaces;

namespace Remnant.Pipeline.Core
{
	public class CorePipeline : IPipeline
	{
		#region Fields

		private static readonly ILog _log = LogManager.GetLogger(typeof(CorePipeline));

		private readonly List<EventListener> _eventListeners = new List<EventListener>();
		private readonly List<AsyncEventTracker> _asyncEventTrackers = new List<AsyncEventTracker>();
		private readonly object _padLock = new object();

		private Connector _connector;
		private IPipelineStage _currentStage;
		protected IPipelineConfigAccess _config;
		protected PipelineState _state;

		#endregion

		#region Construct & Destroy

		public CorePipeline()
		{
			_state = PipelineState.Neutral;
		}

		public CorePipeline(IPipelineConfig config) : this()
		{
			_config = config as IPipelineConfigAccess;
			_config.Data.OnGetCurrentStage += OnGetCurrentStage;
			_config.Data.OnGetEventListeners += () => new ReadOnlyCollection<EventListener>(_eventListeners);
			_config.Data.OnGetStages += () => _config.Stages;
			//   RegisterObserver<ExceptionObserver>();
		}

		public static IPipeline Create(Action<IPipelineConfig> configure = null)
		{
			var config = new PipelineConfig() as IPipelineConfig;

			configure?.Invoke(config);

			return new CorePipeline(config) as IPipeline;
		}

		public static IPipeline Create<TPipeline>(Action<IPipelineConfig> configure = null)
			where TPipeline : CorePipeline, new()
		{
			var config = new PipelineConfig() as IPipelineConfig;

			configure?.Invoke(config);

			var pipeline = new TPipeline() as IPipeline;

			pipeline.Configuration = config;

			return pipeline;
		}


		#endregion

		#region Private Members

		protected IPipelineStage OnGetCurrentStage()
		{
			return _currentStage;
		}

		protected ReadOnlyCollection<IPipelineStage> OnGetRegisteredStages()
		{
			return _config.Stages;
		}

		private void CallObserver(Observer observer, IEvent @event)
		{
			try
			{
				RaiseEvent(new OnCallObserver(observer, @event));

				observer.GetType().InvokeMember("OnEvent",
																				BindingFlags.FlattenHierarchy | BindingFlags.Instance |
																				BindingFlags.InvokeMethod | BindingFlags.Public, null,
																				observer,
																				new object[] { @event, _config.Data });
			}
			catch (TargetInvocationException e)
			{
				if (_state == PipelineState.Aborting)
					throw e.InnerException;

				RaiseEvent(new OnException(e.InnerException));
			}
			catch (Exception e)
			{
				RaiseEvent(new OnException(e));
			}
		}

		private void AsyncEventCallback(IEvent @event, Observer observer)
		{
			if (_state == PipelineState.Aborting)
				return;

			lock (_padLock)
			{
				((AbstractAsyncEvent)@event).OnCompleted -= AsyncEventCallback;
				var asyncTracker = _asyncEventTrackers.Find(tracker => tracker.Event == @event && tracker.SubscribeObserver == observer);

				if (asyncTracker != null)
				{
					_asyncEventTrackers.Remove(asyncTracker);
					if (@event as OnAsyncCompleted == null)
						RaiseEvent(new OnAsyncCompleted(@event, observer));
				}
			}
		}

		private void CallObserverAsync(Observer observer, AbstractAsyncEvent @event)
		{
			@event.OnCompleted += AsyncEventCallback;
			var asyncTracker = new AsyncEventTracker(@event, observer);
			_asyncEventTrackers.Add(asyncTracker);

			var thread = new Thread(() => CallObserver(observer, @event));
			thread.Name = @event.GetType().FullName;
			thread.Start();
		}

		private void Execute()
		{
			SetupObservers();

			if (!_config.AutoRaiseEvents)
				return;

			try
			{
				foreach (var stage in _config.Stages)
				{
					if (_state != PipelineState.Aborting)
					{
						_currentStage = stage;
						RaiseEvent(new OnEnterStage { Stage = _currentStage });

						foreach (var @event in stage.Events)
							RaiseEvent(@event);

						RaiseEvent(new OnExitStage { Stage = _currentStage });
					}
				};
			}
			catch (Exception e)
			{
				if (_state != PipelineState.Aborting)
					RaiseEvent(new OnException(e));
				else
					throw; // pipeline is aborted, propogate exception to caller
			}
		}

		private void CallObservers(IEvent @event)
		{
			var eventListener = _eventListeners.Find(listener =>
						listener.EventName.Equals(@event.GetType().FullName,
																			StringComparison.InvariantCultureIgnoreCase));

			if (eventListener != null)
			{
				eventListener.Observers.ForEach(observer =>
				{
					if (_state == PipelineState.Aborting)
						return;

					if (@event as AbstractAsyncEvent != null)
						CallObserverAsync(observer, @event as AbstractAsyncEvent);
					else
						CallObserver(observer, @event);
				});
			}
		}

		private void SetupObservers()
		{
			foreach (var observer in _config.Observers)
			{
				var implementedEvents = from inf in observer.GetType().GetInterfaces()
																where inf.IsAssignableFrom(typeof(IObserve<IEvent, IPipelineData>))
																select inf;

				observer.OnRaiseEvent += RaiseEvent;

				foreach (var @event in implementedEvents)
				{
					var eventName = @event.GetGenericArguments()[0].FullName;
					var eventListener = _eventListeners.Find(listener =>
						listener.EventName.Equals(@event.GetGenericArguments()[0].FullName,
						StringComparison.InvariantCultureIgnoreCase));

					if (eventListener == null)
					{
						eventListener = new EventListener(eventName);
						_eventListeners.Add(eventListener);
					}

					eventListener.Observers.Add(observer);
				}

				_log.Debug($"Observer {observer.GetType().FullName} is registered.");
			}
		}

		#endregion

		#region IPipeline

		public IPipelineConfig Configuration
		{
			get { return _config as IPipelineConfig; }
			set { _config = value as IPipelineConfigAccess; }
		}

		internal void RaiseEvent(IEvent @event)
		{
			if (@event is OnAbortPipeline)
			{
				_state = PipelineState.Aborting;

				var abortEvent = @event as OnAbortPipeline;
				if (abortEvent.Exception != null)
					throw new PipelineException("Pipeline aborted.", abortEvent.Exception)
					{
						CurrentEvent = abortEvent,
						CurrentStage = _currentStage
					};
			}

			if (_state == PipelineState.Aborting)
				return;

			if (@event as AsyncEvent == null)
				RaiseEvent(new OnEventRaised(@event));

			CallObservers(@event);
			RaiseEventForConnectors(@event);
		}

		public void RaiseEventForConnectors(IEvent @event)
		{
			if (_connector != null)
			{
				foreach (var connection in _connector.Connections)
				{
					if ((connection.Stage != null && connection.Stage == _currentStage) ||
						connection.IsEventRegistered(@event))
					{
						connection.RaiseEvent(@event);
					}
				}
			}
		}

		public void RaiseEvent<TEvent>(string stage)
			where TEvent : IEvent
		{
			Shield.Against(_config.AutoRaiseEvents)
				.WithMessage("You cannot raise an event manually unless you set 'AutoRaiseEvents' to false on the pipeline.")
				.Raise<MethodAccessException>();

			_currentStage = _config.Stages.FirstOrDefault(s => s.Name.Equals(stage, StringComparison.OrdinalIgnoreCase));

			Shield.AgainstNull(_currentStage, typeof(TEvent).FullName, stage)
					.WithMessage("Unable to raise event '{0}' because the stage '{1}' is not registered/found.")
					.Raise();

			var @event = _currentStage.Events.FirstOrDefault(e => e.GetType() == typeof(TEvent));

			Shield.AgainstNull(@event, typeof(TEvent).FullName, stage)
			 .WithMessage("Unable to raise event '{0}' because the event is not registered for stage '{1}'.")
			 .Raise();

			RaiseEvent(@event);
		}

		public void RaiseEvent(string stage, IEvent @event)
		{
			_currentStage = _config.Stages.FirstOrDefault(s => s.Name.Equals(stage, StringComparison.OrdinalIgnoreCase));

			Shield.AgainstNull(_currentStage, @event.GetType().FullName, stage)
						.WithMessage("Unable to raise event '{0}' because the stage '{1}' is not registered/found.")
						.Raise();

			if (_currentStage.Events.FirstOrDefault(e => @event.GetType() == e.GetType()) != null)
			{
				RaiseEvent(@event);
			}
		}

		public void Run()
		{
			_state = PipelineState.Running;

			Execute();

			while (_state != PipelineState.Aborting && _asyncEventTrackers.Count != 0)
			{
				Thread.Sleep(100);
			}

			_state = PipelineState.Stopped;
		}

		public void RunAsync(int idleTime = 1000)
		{
			var thread = new Thread(() =>
				{
					Execute();

					while (_state != PipelineState.Aborting && _asyncEventTrackers.Count != 0)
					{
						Thread.Sleep(idleTime);
					}
				});

			thread.Name = Name;
			thread.Start();
			_state = PipelineState.Running;
		}

		public void Stop()
		{
			_state = PipelineState.Aborting;
		}

		public void Abort()
		{
			_state = PipelineState.Aborting;
			_asyncEventTrackers.Clear();
		}

		public string Name { get { return _config.Name; } }

		public PipelineState State { get { return _state; } }

		public bool AutoRaiseEvents { get { return _config.AutoRaiseEvents; } }

		public IConnector Connect(IPipeline pipeline)
		{
			if (_connector == null)
				_connector = new Connector();

			_connector.Connect(pipeline);
			return _connector;
		}

		#endregion
	}
}
