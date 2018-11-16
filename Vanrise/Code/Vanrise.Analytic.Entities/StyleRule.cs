﻿using System;
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
        public StyleCodeEnum StyleValue { get; set; }
        public string StyleCode { get; set; }
    }

    public abstract class StyleRuleCondition
    {
        public abstract Guid ConfigId { get; }
        public abstract bool Evaluate(IStyleRuleConditionContext context);
    }

    public interface IStyleRuleConditionContext
    {
        dynamic Value { get;}
    }

    public class MeasureStyleRuleDetail
    {
        public string MeasureTitle { get; set; }

        public string MeasureName { get; set; }

        public List<StyleRuleDetail> Rules { get; set; }
    }
    public class StyleRuleDetail
    {
        public string RecordFilterDescription { get; set; }
        public RecordFilter RecordFilter { get; set; }
        public string StyleValueDescription { get; set; }
        public string StyleCode { get; set; }
        public StyleCodeEnum StyleValue { get; set; }

    }
}
