using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.CDRProcess.Arguments
{
    public class DailyRepricingProcessInput : Vanrise.BusinessProcess.Entities.BaseProcessInputArgument
    {
        public DateTime RepricingDay { get; set; }
        public bool GenerateTrafficStatistic { get; set; }
        public List<string> CustomersIds { get; set; }
        public List<string> SupplierIds { get; set; }
        public bool DivideProcessIntoSubProcesses { get; set; }

        public override string GetTitle()
        {
            return String.Format("#BPDefinitionTitle# for date {0:dd-MMM-yyyy}", this.RepricingDay);
        }

        public override void MapExpressionValues(Dictionary<string, object> evaluatedExpressions)
        {
            if (evaluatedExpressions.ContainsKey("RepricingDay"))
            {
                Console.WriteLine("RepricingDay old value {0}", RepricingDay);
                Console.WriteLine("The new value {0}", evaluatedExpressions["RepricingDay"]);
                RepricingDay = (DateTime) evaluatedExpressions["RepricingDay"];
                Console.WriteLine("RepricingDay new value {0}", RepricingDay);
            }
        }
    }
}
