using Remnant.Pipeline.Core;
using Remnant.Pipeline.Tests.Events;
using System;
using Remnant.Pipeline.Interfaces;
using Remnant.Pipeline.Events;
using System.Threading;

namespace Remnant.Pipeline.Tests.Observers
{
	public class ParserObserver : Observer,
		IObserve<OnParseEvent, IPipelineData>,
		IObserve<OnParseAsyncEvent, IPipelineData>
	{
		public ParserObserver()
		{
		}

		public void OnEvent(OnParseEvent @event, IPipelineData data)
		{
			Thread.Sleep(500);
			IsSyncParseCompleted = true;
			Console.WriteLine("Sync observed");
		}

		public void OnEvent(OnParseAsyncEvent @event, IPipelineData data)
		{
			Console.WriteLine("ASync observed");
			IsAsyncParseReceived = true;
			Thread.Sleep(2000);
			IsAsyncParseCompleted = true;
			Console.WriteLine("ASync completed after wait");
			@event.Complete(this);
		}

		public bool IsSyncParseCompleted;
		public bool IsAsyncParseReceived;
		public bool IsAsyncParseCompleted;
	}
}