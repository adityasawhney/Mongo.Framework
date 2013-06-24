using System.Linq;
using Microsoft.Test.FaultInjection;

namespace Mongo.FaultInjection.Trigger
{
    /// <summary>
    /// Combines multiple triggers and fires only if all are triggered
    /// </summary>
    public class CompositeTrigger : ICondition
    {
        private readonly ICondition[] triggers;

        public CompositeTrigger(params ICondition[] conditions)
        {
            this.triggers = conditions;
        }

        public bool Trigger(IRuntimeContext context)
        {
            return triggers.All(t => t.Trigger(context));
        }
    }
}
