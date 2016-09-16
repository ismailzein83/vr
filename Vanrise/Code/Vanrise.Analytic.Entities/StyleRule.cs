using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Analytic.Entities
{
    public class MeasureStyleRule
    {
        public string MeasureName { get; set; }

        public List<StyleRule> Rules { get; set; }
    }
    public class StyleRule
    {
        public RecordFilter RecordFilter { get; set; } 

        public string StyleCode { get; set; }
    }

    public abstract class StyleRuleCondition
    {
        public Guid ConfigId { get; set; }
        public abstract bool Evaluate(IStyleRuleConditionContext context);
    }

    public interface IStyleRuleConditionContext
    {
        dynamic Value { get;}
    }
}
