﻿using System;
using System.Collections.Generic;
using System.Linq;
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

            decimal unassignedPercentages = 100 - assignedPercentages;

            Decimal totalTakenPercentage = 0;
            for (int i = 0; i < optionsCount; i++)
            {
                var currentOption = optionsList[i];
                if (i > (percentagesCount - 1))
                    currentOption.Percentage = 0;
                else
                {
                    if (i == (optionsCount - 1) || i == (percentagesCount - 1))
                        currentOption.Percentage = 100 - totalTakenPercentage;
                    else
                    {
                        var correspondingPercentage = this.Percentages[i];
                        decimal caluclatedPercentage = decimal.Round(correspondingPercentage + correspondingPercentage * unassignedPercentages / assignedPercentages, 2);
                        optionsList[i].Percentage = caluclatedPercentage;
                        totalTakenPercentage += caluclatedPercentage;
                    }
                }
            }
        }
    }
}
