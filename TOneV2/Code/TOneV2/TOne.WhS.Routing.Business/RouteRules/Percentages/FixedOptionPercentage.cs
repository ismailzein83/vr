using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Percentages
{
    public class FixedOptionPercentage : RouteOptionPercentageSettings
    {
        public List<Decimal> Percentages { get; set; }

        public override void Execute(IRouteOptionPercentageExecutionContext context)
        {
            Decimal totalTakenPercentage = 0;
            int percentagesCount = this.Percentages.Count;
            List<IRouteOptionPercentageTarget> optionsList = context.Options.ToList();
            int optionsCount = optionsList.Count;
            for (int i = 0; i < optionsCount; i++)
            {
                Decimal optionPercentage = 0;
                
                if (i >= percentagesCount)
                    optionPercentage = 100 - totalTakenPercentage;
                else
                    optionPercentage = Math.Min(this.Percentages[i], 100 - totalTakenPercentage);
                optionsList[i].Percentage = optionPercentage;
                totalTakenPercentage -= optionPercentage;

                if (totalTakenPercentage == 100)
                    break;
            }
        }
    }
}
