using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Test.FaultInjection;

namespace Mongo.FaultInjection
{
	/// <summary>
	/// Handler for listening to fault generation events
	/// </summary>
	/// <param name="e">The event.</param>
	public delegate void FaultHandler(FaultEvent e);

	/// <summary>
	/// Details of a fault event
	/// </summary>
	public class FaultEvent : EventArgs
	{
		public string MethodName { get; set; }
		public Exception Exception { get; set; }
		public object ReturnValue { get; set; }
		public bool IsException { get { return Exception != null; } }
		public bool HasReturnValue { get { return ReturnValue != null; } }

        public override string ToString()
        {
            object value = IsException ? Exception : ReturnValue;
            return string.Format("Method: {0}, Value Type: {1}", MethodName, value.GetType().Name);
        }
	}

	/// <summary>
	/// Behavior specification of a fault injection scenario
	/// </summary>
	public interface IFaultInjectionTestScenario
	{
		IList<FaultRule> Rules { get; }
        ConcurrentQueue<FaultEvent> Events { get; }

		void SetupRules();
		void AssertFaults();
	}

	/// <summary>
	/// Base class which contains boilerplate code
	/// </summary>
	public abstract class BaseFaultInjectionTestScenario : IFaultInjectionTestScenario
	{
        public static IFaultInjectionTestScenario Create(Type scenarioType)
        {
            var fiScenario = Activator.CreateInstance(scenarioType) as IFaultInjectionTestScenario;

            if (fiScenario == null)
            {
                throw new ArgumentException(string.Format("Wrong type: {0} must implement {1}",
                                            scenarioType,
                                            typeof(IFaultInjectionTestScenario).Name));
            }
            return fiScenario;
        }

		protected BaseFaultInjectionTestScenario()
		{
			Rules = new List<FaultRule>();
            Events = new ConcurrentQueue<FaultEvent>();
		}

		public IList<FaultRule> Rules { get; private set; }
        public ConcurrentQueue<FaultEvent> Events { get; private set; }

		public abstract void AssertFaults();
		public abstract void SetupRules();
	}

	/// <summary>
	/// Interface which any fault injection test class should implement 
	/// </summary>
	public interface IFaultInjectionTest
	{
		IFaultInjectionTestScenario TestScenario { get; set; }
	}

	/// <summary>
	/// Test scope class to configure fault injection rules specific for that test
	/// </summary>
	public sealed class FaultInjection : Attribute, IDisposable
	{
		private IFaultInjectionTestScenario fiScenario;
	    private IFaultInjectionTest fiTest;

        public FaultInjection(IFaultInjectionTestScenario scenario, IFaultInjectionTest test)
        {
            Init(scenario, test);
        }

        public void Dispose()
        {
            Deinit();
        }

        private void Init(IFaultInjectionTestScenario scenario, IFaultInjectionTest test)
        {
            this.fiScenario = scenario;
            this.fiTest = test;
            this.fiTest.TestScenario = this.fiScenario;

            this.fiScenario.SetupRules();
            new FaultScope(this.fiScenario.Rules.ToArray());
        }

        private void Deinit()
        {
			FaultScope.Current.Dispose();
			this.fiScenario.AssertFaults();
			this.fiTest.TestScenario = null;
        }
	}
}
