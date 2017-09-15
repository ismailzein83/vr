using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business.RouteRules.Percentages
{
    public class FixedOptionPercentage : RouteOptionPercentageSettings
    {
        public override Guid ConfigId { get { return new Guid("9d6d19c0-4904-4e91-8831-80619abec818"); } }

        public List<int> Percentages { get; set; }

        public override void Execute(IRouteOptionPercentageExecutionContext context)
        {
            List<IRouteOptionPercentageTarget> optionsList = context.Options.ToList();

            int percentagesCount = this.Percentages.Count;
            int optionsCount = optionsList.Count;

            int assignedPercentages = 0;
            for (int j = 0; j < optionsCount; j++)
            {
                if (j < percentagesCount)
                    assignedPercentages += this.Percentages[j];
            }

            int unassignedPercentages = 100 - assignedPercentages;

            IRouteOptionPercentageTarget routeOptionWithHighestPercentage = null;
            int totalTakenPercentage = 0;
            for (int i = 0; i < optionsCount; i++)
            {
                var currentOption = optionsList[i];
                if (i > (percentagesCount - 1))
                {
                    currentOption.Percentage = 0;
                }
                else
                {
                    var correspondingPercentage = this.Percentages[i];
                    int caluclatedPercentage = correspondingPercentage + correspondingPercentage * unassignedPercentages / assignedPercentages;
                    currentOption.Percentage = caluclatedPercentage;
                    totalTakenPercentage += caluclatedPercentage;

                    if (routeOptionWithHighestPercentage == null || routeOptionWithHighestPercentage.Percentage < currentOption.Percentage)
                        routeOptionWithHighestPercentage = currentOption;
                }
            }

            if (routeOptionWithHighestPercentage != null && totalTakenPercentage != 100)
                routeOptionWithHighestPercentage.Percentage = routeOptionWithHighestPercentage.Percentage.Value + (100 - totalTakenPercentage);
        }
    }
}
