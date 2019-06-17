using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Deal.MainExtensions.DealTimePeriod
{
    public class OverallDealsTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod
    {
        public override Guid ConfigId => new Guid("CED02013-65C0-49B9-97A7-7F557FF945BA");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime fromTime = DateTime.MaxValue;

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            Dictionary<int, DealDefinition> dealDefinitions = dealDefinitionManager.GetAllCachedDealDefinitions();

            foreach (var dealDefinition in dealDefinitions.Values)
            {
                if (dealDefinition.Settings.Status == DealStatus.Draft)
                    continue;

                if (dealDefinition.Settings.RealBED < fromTime)
                    fromTime = dealDefinition.Settings.RealBED;
            }

            context.ToTime = context.EffectiveDate;
            context.FromTime = fromTime;
        } 
    }
}