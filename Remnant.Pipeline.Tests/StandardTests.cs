
using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using log4net;
using NUnit.Framework;
using Remnant.Pipeline;
using Remnant.Pipeline.Core;
using Remnant.Pipeline.Data;
using Remnant.Pipeline.Events;
using Remnant.Pipeline.Interfaces;
using Remnant.Pipeline.Tests.Events;
using Remnant.Pipeline.Tests.Observers;

namespace Remnant.Datahub.Pipeline.Tests
{
	[TestFixture]
	public class StandardTests
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(StandardTests));

		public StandardTests()
		{
			var log4netConfig = new XmlDocument();
			log4netConfig.Load(File.OpenRead("log4net.config"));

			var repo = LogManager.CreateRepository(
					Assembly.GetEntryAssembly(),
					typeof(log4net.Repository.Hierarchy.Hierarchy));

			log4net.Config.XmlConfigurator.Configure(repo, log4netConfig["log4net"]);
		}

		[Test]
		public void Should_be_able_to_perform_full_config()
		{
			var config = new PipelineConfig() as IPipelineConfig;

			config
				.Name("Pipeline A")
				.AutoRaiseEvents(false)
				.UseData(new PipelineData())
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent<OnParseEvent>()
				.Observers()
				.RegisterObserver<ParserObserver>();

			var configAccess = config as IPipelineConfigAccess;

			Assert.AreEqual(configAccess.Name, "Pipeline A");
			Assert.AreEqual(configAccess.AutoRaiseEvents, false);
			Assert.IsNotNull(configAccess.Data);

			Assert.IsNotNull(configAccess.Stages[0]);
			Assert.AreEqual(configAccess.Stages[0].Name, "Stage 1");
			Assert.IsNotNull(configAccess.Stages[0].Events[0]);
			Assert.AreEqual(configAccess.Stages[0].Events[0].GetType(), typeof(OnParseEvent));
			Assert.IsNotNull(configAccess.Observers[0]);
			Assert.AreEqual(configAccess.Observers[0].GetType(), typeof(ParserObserver));
		}

		[Test]
		public void Should_be_able_to_register_basic_and_raise_event()
		{
			var pipeline = CorePipeline.Create(c => c
				.Name("TestPipeline")
				.UseData(new PipelineData())
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent(new OnParseEvent { Text = "Please parse me" })
				.Observers()
				.RegisterObserver<ParserObserver>()
				.RegisterObserver<DebugObserver>());

			var debugObserver = (DebugObserver)(pipeline.Configuration as IPipelineConfigAccess).Observers[1];
			debugObserver.OnDebugEvent += e =>
			{
				Assert.AreEqual(e.GetType(), typeof(OnParseEvent));
				Assert.AreEqual(((OnParseEvent)e).Text, "Please parse me");
			};

			pipeline.Run();

			Assert.AreEqual(pipeline.Name, "TestPipeline");
		}

		[Test]
		public void Should_be_able_to_register_basic_and_raise_async_event()
		{
			// expected fist event to fire async, and immediate the next one
			// a 1sec wait is put in async to see if 2nd event is raised, async will be on its own thread

			var observer = new ParserObserver();


			var pipeline = CorePipeline.Create(c => c
				.Name("TestPipeline")
				.UseData(new PipelineData())
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent(new OnParseAsyncEvent { Text = "Please parse me async" })
				.ForEvent(new OnParseEvent { Text = "Please parse me" })
				.Observers()
				.RegisterObserver(observer)
				.RegisterObserver<DebugObserver>());

			pipeline.RunAsync();


			while (pipeline.State == PipelineState.Running)
			{
				Thread.Sleep(1000);
				if (observer.IsAsyncParseReceived && !observer.IsAsyncParseCompleted)
				{
					Assert.True(observer.IsSyncParseCompleted, "The second sync event was not observed after async event.");
					pipeline.Abort();
				}
			}
		}


		[Test]
		public void Should_be_able_to_connect_pipelines()
		{

			var pipelineA = CorePipeline.Create(c => c
				.Name("Pipeline A")
				.AutoRaiseEvents(false)
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent<OnParseEvent>());

			var observer = new ReverseParserObserver();

			var pipelineB = CorePipeline.Create(c => c
				.Name("Pipeline B")
				.Observers()
				.RegisterObserver(observer));

			pipelineA.Connect(pipelineB)
				.ForStages("Stage 1")
				.ForEvent<OnParseEvent>();

			pipelineA.RunAsync();
			pipelineA.RaiseEvent<OnParseEvent>("Stage 1");

			while (pipelineA.State == PipelineState.Running)
			{
				Thread.Sleep(1000);

				Assert.True(observer.IsSyncParseCompleted, "The second sync event was not observed after async event.");
				pipelineA.Abort();

			}
		}

		[Test]
		public void Should_be_able_to_connect_pipelines_for_multiple_stages()
		{
			var pipelineA = CorePipeline.Create(c => c
				.Name("Pipeline A")
				.AutoRaiseEvents(true)
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent<OnParseEvent>()
				.RegisterStage("Stage 2")
				.ForEvent<OnSomeEvent>());

			var observer = new ReverseParserObserver();

			var pipelineB = CorePipeline.Create(c => c
				.Name("Pipeline B")
				.Observers()
				.RegisterObserver(observer));

			pipelineA.Connect(pipelineB)
				.ForStages("Stage2")
				.ForEvent<OnSomeEvent>();

			pipelineA.Run();
		}

		[Test]
		public void Should_be_able_to_connect_pipelines_for_one_stage()
		{
			var pipelineA = CorePipeline.Create(c => c
				.Name("Pipeline A")
				.AutoRaiseEvents(true)
				.Stages()
				.RegisterStage("Stage 1")
				.ForEvent<OnParseEvent>()
				.RegisterStage("Stage 2")
				.ForEvent<OnSomeEvent>());

			var observer = new ReverseParserObserver();

			var pipelineB = CorePipeline.Create(c => c
				.Name("Pipeline B")
				.Observers()
				.RegisterObserver(observer));

			pipelineA.Connect(pipelineB)
				.ForStages("Stage 2")
				.ForEvent<OnSomeEvent>();

			pipelineA.Run();
		}
	}
}
