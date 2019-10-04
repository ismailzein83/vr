using System;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class NumberLengthEvaluator
    {
        public abstract Guid ConfigID { get; }

        public abstract void Evaluate(INumberLengthEvaluateContext context);
    }

    public interface INumberLengthEvaluateContext
    {
        string Code { get; }

        int MinCodeLength { set; }

        int MaxCodeLength { set; }
    }

    public class NumberLengthEvaluateContext : INumberLengthEvaluateContext
    {
        public string Code { get; set; }

        public int MinCodeLength { get; set; }

        public int MaxCodeLength { get; set; }
    }
}