using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.BP.Arguments
{
	public class DealEvaluatorProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
	{
		public override string GetTitle()
		{
			return String.Format("Deal Evaluator Process");
		}

		public DateTime DealEffectiveAfter { get; set; }

		public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
		{
			if (evaluatedExpressions.ContainsKey("ScheduleTime") && evaluatedExpressions.ContainsKey("DaysBack"))
			{
				var effectiveDate = (DateTime)evaluatedExpressions["ScheduleTime"];
				this.DealEffectiveAfter = effectiveDate.Date.AddDays(-1 * int.Parse(evaluatedExpressions["DaysBack"].ToString()));
			}
		}
	}
}
