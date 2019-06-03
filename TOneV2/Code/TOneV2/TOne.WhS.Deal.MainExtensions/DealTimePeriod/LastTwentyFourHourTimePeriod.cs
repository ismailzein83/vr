using System;
using System.Collections.Generic;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.Entities;

namespace TOne.WhS.Deal.MainExtensions.DealTimePeriod
{
    public class LastTwentyFourHourTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod
    {
        public override Guid ConfigId => new Guid("215017D3-256F-4AFD-AF99-4266DD4D0AC4");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.ToTime = context.EffectiveDate;
            context.FromTime = context.EffectiveDate.AddDays(-1);
        }
    }

    public class WeeklyTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod
    {
        public override Guid ConfigId => new Guid("3C5CCF5B-224E-4502-AFA4-2F18A8E39660");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            context.ToTime = context.EffectiveDate;
            context.FromTime = context.EffectiveDate.AddDays(-7);
        }
    }

    public class OverallDealsTimePeriod : TOne.WhS.Deal.Entities.DealTimePeriod
    {
        public override Guid ConfigId => new Guid("CED02013-65C0-49B9-97A7-7F557FF945BA");

        public override void GetTimePeriod(IVRTimePeriodContext context)
        {
            DateTime fromTime = DateTime.MaxValue;

            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            Dictionary<int, DealDefinition> dealDefinitions = dealDefinitionManager.GetAllCachedDealDefinitions();

            foreach(var dealDefinition in dealDefinitions.Values)
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