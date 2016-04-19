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
            List<IRouteOptionPercentageTarget> optionsList = context.Options.ToList();

            int percentagesCount = this.Percentages.Count;
            int optionsCount = optionsList.Count;

            decimal assignedPercentages = 0;
            for (int j = 0; j < optionsCount; j++)
            {
                if (j < percentagesCount)
                    assignedPercentages += this.Percentages[j];
            }

            decimal unassignedPercentages = 0;
            int difference = percentagesCount - optionsCount;
            if (difference > 0)
            {
                for (int j = optionsCount; j < percentagesCount; j++)
                {
                    unassignedPercentages += this.Percentages[j];
                }
            }

            Decimal totalTakenPercentage = 0;
            for (int i = 0; i < optionsCount; i++)
            {
                if (i > (percentagesCount - 1))
                    optionsList[i].Percentage = 0;
                else
                {
                    if (i == (optionsCount - 1) || i == (percentagesCount - 1))
                        optionsList[i].Percentage = 100 - totalTakenPercentage;
                    else
                    {
                        decimal caluclatedPercentage = decimal.Round(this.Percentages[i] + this.Percentages[i] * unassignedPercentages / assignedPercentages, 2);
                        optionsList[i].Percentage = caluclatedPercentage;
                        totalTakenPercentage += caluclatedPercentage;
                    }
                }
            }

            //Decimal totalTakenPercentage = 0;
            //int percentagesCount = this.Percentages.Count;
            //List<IRouteOptionPercentageTarget> optionsList = context.Options.ToList();
            //int optionsCount = optionsList.Count;
            //for (int i = 0; i < optionsCount; i++)
            //{
            //    Decimal optionPercentage = 0;

            //    if (i >= percentagesCount)
            //        optionPercentage = 100 - totalTakenPercentage;
            //    else
            //        optionPercentage = Math.Min(this.Percentages[i], 100 - totalTakenPercentage);
            //    optionsList[i].Percentage = optionPercentage;
            //    totalTakenPercentage += optionPercentage;

            //    if (totalTakenPercentage == 100)
            //        break;
            //}
        }
    }
}
