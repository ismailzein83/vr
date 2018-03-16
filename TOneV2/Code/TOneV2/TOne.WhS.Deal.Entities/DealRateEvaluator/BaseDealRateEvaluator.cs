using System;

namespace TOne.WhS.Deal.Entities
{
    public abstract class BaseDealRateEvaluator
    {
        public abstract Guid ConfigId { get; }

        public abstract string GetDescription();

    }
}
