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

            for (int j = 0; j < optionsCount; j++)
            {
                var currentOption = optionsList[j];
                if (j < percentagesCount)
                    currentOption.Percentage = this.Percentages[j];
                else
                    break;
            }
        }
    }
}
