using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.BP.Arguments
{
	public class VRCorrelationProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
	{
		public static Guid BPDefinitionId { get { return new Guid("93CE52F0-F9D3-44DE-8D84-897D100155BE"); } }

		public Guid VRCorrelationDefinitionId { get; set; }

		public TimeSpan DurationAcceptedErrorRange { get; set; }

		public TimeSpan DatetimeAcceptedErrorRange { get; set; }

		public override string GetTitle()
		{
			return String.Format("Correlation process");
		}

		public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
		{
			if (evaluatedExpressions.ContainsKey("ScheduleTime"))
			{
				var effectiveDate = (DateTime)evaluatedExpressions["ScheduleTime"];
			}
		}
	}
}
