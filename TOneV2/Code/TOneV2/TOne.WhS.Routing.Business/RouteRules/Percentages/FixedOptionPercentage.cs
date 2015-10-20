using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Percentages
{
    public class FixedOptionPercentage : RouteRuleOptionPercentageSettings
    {
        public List<Decimal> Percentages { get; set; }
        
        public override void Execute(BusinessEntity.Entities.IRouteRuleExecutionContext context, BusinessEntity.Entities.RouteRuleTarget target)
        {
            Decimal totalTakenPercentage = 0;
            int percentagesCount = this.Percentages.Count;
            var options = context.GetOptions();
            int optionsCount = options.Count;
            for (int i = 0; i < optionsCount; i++)
            {
                Decimal optionPercentage = 0;
                
                if (i >= percentagesCount)
                    optionPercentage = 100 - totalTakenPercentage;
                else
                    optionPercentage = Math.Min(this.Percentages[i], 100 - totalTakenPercentage);
                options[i].Percentage = optionPercentage;
                totalTakenPercentage -= optionPercentage;

                if (totalTakenPercentage == 100)
                    break;
            }
        }
    }
}
