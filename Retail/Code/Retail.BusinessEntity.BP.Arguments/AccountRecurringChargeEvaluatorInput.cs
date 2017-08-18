using System;
using System.Collections.Generic;

namespace Retail.BusinessEntity.BP.Arguments
{
    public class AccountRecurringChargeEvaluatorInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime EffectiveDate { get; set; }

        public override string GetTitle()
        {
            return String.Format("Account Recurring Charge Evaluator Process for Effective Date: {0}", EffectiveDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime"))
            {
                EffectiveDate = DateTime.Now.Date;
            }
        }
    }
}
