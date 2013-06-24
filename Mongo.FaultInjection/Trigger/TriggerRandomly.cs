using System;
using Microsoft.Test.FaultInjection;
using System.Diagnostics;

namespace Mongo.FaultInjection.Trigger
{
    public class TriggerRandomlyWithMinMaxThresholds : ICondition
    {
        private int triggerCount = 0;
		private readonly int minTriggers = 0;
		private readonly int maxTriggers = int.MaxValue;
        private readonly Random random = new Random();

        public TriggerRandomlyWithMinMaxThresholds(int minN, int maxN)
        {
			Debug.Assert(minN <= maxN);
	        this.minTriggers = minN;
            this.maxTriggers = maxN;
        }

        public bool Trigger(IRuntimeContext context)
        {
			// There is no control over how many times this will be called so 
			// there is no guarantee that rule will be triggered given Minimum times.
            bool trigger = triggerCount < minTriggers || random.Next(0, 2) == 0;
            if (trigger) triggerCount++;
            return (triggerCount <= maxTriggers) && trigger;
        }
    }
}
