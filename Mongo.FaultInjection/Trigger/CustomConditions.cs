using System.Reflection;
using Microsoft.Test.FaultInjection;
using Microsoft.Test.FaultInjection.SignatureParsing;

namespace Mongo.FaultInjection.Trigger
{
	public static class CustomConditions
	{
        /// <summary>
        /// Creates a condition which is an aggregation of various conditions
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public static ICondition Combine(params ICondition[] conditions)
        {
            return new CompositeTrigger(conditions);
        }

		/// <summary>
		/// Condition which triggers a fault when the faulted method is called first n times by the specified caller.
		/// </summary>
		/// <param name="n">A positive number.</param>
		/// <param name="caller">A string in the format:
		/// System.Console.WriteLine(string),
		/// Namespace&lt;T&gt;.OuterClass&lt;E&gt;.InnerClass&lt;F,G&gt;.MethodName&lt;H&gt;(T, E, F, H, List&lt;H&gt;).
		/// </param>   
		public static ICondition TriggerOnFirstNCallBy(int n, string caller)
		{
			return new TriggerOnFirstNCallBy(n, caller);
		}

		/// <summary>
		/// Condition which triggers a fault when the faulted method is called first n times by the specified caller.
		/// </summary>
		/// <param name="n">A positive number.</param>
		/// <param name="caller">The target method's caller.</param>   
		public static ICondition TriggerOnFirstNCallBy(int n, MethodBase caller)
		{
			return new TriggerOnFirstNCallBy(n, MethodSignatureTranslator.GetCSharpMethodString(caller));
		}

        /// <summary>
        /// Condition is triggered for first n calls of a method
        /// </summary>
        /// <param name="n">A positive number</param>
        /// <returns></returns>
        public static ICondition TriggerOnFirstNCall(int n)
        {
            return new TriggerOnFirstNCallBy(n);
        }

        /// <summary>
        /// Condition to randomly trigger a fault
        /// </summary>
        /// <returns></returns>
        public static ICondition TriggerRandomly(int minN = 0, int maxN = int.MaxValue)
        {
			return new TriggerRandomlyWithMinMaxThresholds(minN, maxN);
        }

        /// <summary>
        /// Condition to trigger randomly with an upper bound on number of triggers
        /// </summary>
        /// <param name="maxN"></param>
        /// <returns></returns>
        public static ICondition TriggerRandomlyWithMax(int maxN)
        {
            return new TriggerRandomlyWithMinMaxThresholds(0, maxN);
        }
	}
}
