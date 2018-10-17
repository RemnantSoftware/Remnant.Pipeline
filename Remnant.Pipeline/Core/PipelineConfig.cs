using Remnant.Pipeline.Data;
using Remnant.Pipeline.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Remnant.Pipeline.Core
{
	public class PipelineConfig : 
		IPipelineConfig, 
		IPipelineConfigAccess, 
		IStages, 
		IObservers, 
		IRegisterObserver, 
		IRegisterStage
	{
		private readonly List<IPipelineStage> _stages = new List<IPipelineStage>();
		private readonly List<Observer> _observers = new List<Observer>();
		private IPipelineStage _currentStage;

		public PipelineConfig()
		{
			_stages = new List<IPipelineStage>();

			Data = new PipelineData();
			Name = GetType().FullName;
			AutoRaiseEvents = true;
		}

		public PipelineConfig(Action<IPipelineConfig> configure)
			: this()
		{
			configure?.Invoke(this);
		}


		#region IPipelineConfigAccess

		public string Name { get; set; }

		public bool AutoRaiseEvents { get; set; }

		public IPipelineData Data { get; set; }

		ReadOnlyCollection<IPipelineStage> IPipelineConfigAccess.Stages => new ReadOnlyCollection<IPipelineStage>(_stages);

		ReadOnlyCollection<Observer> IPipelineConfigAccess.Observers => new ReadOnlyCollection<Observer>(_observers);

		#endregion

		#region IPipelineConfig

		IStages IPipelineConfig.Stages()
		{
			return this;
		}

		IPipelineConfig IPipelineConfig.AutoRaiseEvents(bool value)
		{
			AutoRaiseEvents = value;
			return this;
		}

		IPipelineConfig IPipelineConfig.Name(string name)
		{
			Name = name;
			return this;
		}

		IPipelineConfig IPipelineConfig.UseData(IPipelineData data)
		{
			Data = data;
			return this;
		}

		#endregion

		#region IObservers

		IObservers IObservers.RegisterObserver<TObserver>()
		{
			_observers.Add(new TObserver());
			return this;
		}

		IObservers IObservers.RegisterObserver(Observer observer)
		{
			_observers.Add(observer);
			return this;
		}

		IStages IObservers.Stages()
		{
			return this;
		}


		public IObservers Observers()
		{
			return this;
		}

		#endregion

		#region IStages

		IRegisterStage IStages.RegisterStage<TPipelineStage>(string name)
		{
			var stage = new TPipelineStage() { Name = name } as IPipelineStage;
			_stages.Add(stage);
			return this;
		}

		IRegisterStage IStages.RegisterStage(string name)
		{
			_currentStage = new PipelineStage(name);
			_stages.Add(_currentStage);
			return this;
		}

		IPipelineStage IStages.FindStage(string name)
		{
			var stage = _stages.Find(s => s.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

			//Shield.AgainstNull(stage, nameof(stage)).Raise();

			return stage;
		}

		#endregion

		#region IRegisterObserver

		IRegisterObserver IRegisterObserver.AndObserver<TObserver>()
		{
			_observers.Add(new TObserver());
			return this;
		}

		#endregion

		#region IRegisterStage

		IRegisterStage IRegisterStage.ForEvent<TEvent>()
		{
			_currentStage.ForEvent<TEvent>();
			return this;
		}

		IRegisterStage IRegisterStage.ForEvent(IEvent @event)
		{
			_currentStage.ForEvent(@event);
			return this;
		}

		IRegisterStage IRegisterStage.RegisterStage<TPipelineStage>(string name)
		{
			return (this as IStages).RegisterStage<TPipelineStage>(name);
		}

		IRegisterStage IRegisterStage.RegisterStage(string name)
		{
			return (this as IStages).RegisterStage(name);
		}

		IObservers IRegisterStage.Observers()
		{
			return this;
		}

		#endregion
	}
}
