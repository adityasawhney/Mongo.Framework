using System;
using Microsoft.Test.FaultInjection;
using System.Threading;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Mongo.FaultInjection.Trigger
{
	public class TriggerOnFirstNCallBy : ICondition
    {
        public TriggerOnFirstNCallBy(int aN)
        {
            calledTimes = 0;
            n = aN;
            if (aN <= 0)
            {
                throw new ArgumentException("The first parameter of TriggerOnFirstNCallBy(int, string) should be a postive number");
            }
        }

        public TriggerOnFirstNCallBy(int aN, String aTargetCaller) : this(aN)
        {
            targetCaller = Signature.ConvertSignature(aTargetCaller);
        }

        public bool Trigger(IRuntimeContext context)
        {
            if (!string.IsNullOrEmpty(targetCaller) && targetCaller != context.Caller)
            {
                return false;
            }

	        Interlocked.Increment(ref calledTimes);

			return calledTimes <= n;
        }

		private int calledTimes;
        private readonly int n;
        private readonly String targetCaller; // Optional
    }
}
