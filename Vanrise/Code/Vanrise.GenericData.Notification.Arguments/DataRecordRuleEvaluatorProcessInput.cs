using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Notification.Arguments
{
    public class DataRecordRuleEvaluatorProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public Guid DataRecordRuleEvaluatorDefinitionId { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public VRTimePeriod TimePeriod {get; set;}

        public override string GetTitle()
        {
            return "Data Record Rule Evaluator";
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("ScheduleTime") && evaluatedExpressions.ContainsKey("VRTimePeriod"))
            {
                DateTime defaultDate = new DateTime(0001,1,1);
                if (DateTime.Compare(this.FromDate.Date, defaultDate) == 0 && DateTime.Compare(this.ToDate.Date, defaultDate) == 0)
                {
                    var effectiveDate = (DateTime)evaluatedExpressions["ScheduleTime"];
                    this.TimePeriod = (VRTimePeriod)evaluatedExpressions["VRTimePeriod"];

                    VRTimePeriodContext context = new VRTimePeriodContext();
                    context.EffectiveDate = effectiveDate;
                    this.TimePeriod.GetTimePeriod(context);

                    this.FromDate = context.FromTime;
                    this.ToDate = context.ToTime;
                }
            }
        }
    }
}
