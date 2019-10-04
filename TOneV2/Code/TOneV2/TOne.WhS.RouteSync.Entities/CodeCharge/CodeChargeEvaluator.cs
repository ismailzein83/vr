using System;

namespace TOne.WhS.RouteSync.Entities
{
    public abstract class CodeChargeEvaluator
    {
        public abstract Guid ConfigID { get; }

        public abstract void Evaluate(ICodeChargeEvaluateContext context); 
    }

    public interface ICodeChargeEvaluateContext
    {
        string Code { get; }

        string CodeCharge { set; }
    }

    public class CodeChargeEvaluateContext : ICodeChargeEvaluateContext
    {
        public string Code { get; set; }

        public string CodeCharge { get; set; }
    }
}