﻿using System;
using Vanrise.Common;
using System.Activities;
using TOne.WhS.Deal.Business;
using TOne.WhS.Deal.Entities;
using Vanrise.BusinessProcess;
using System.Collections.Generic;

namespace TOne.WhS.Deal.BP.Activities
{
    public class GetDealDefinitionsToReevaluateInput
    {
        public DateTime DealEffectiveAfter { get; set; }
    }

    public class GetDealDefinitionsToReevaluateOutput
    {
        public IEnumerable<DealDefinition> DealDefinitionsToReevaluate { get; set; }
        public IEnumerable<int> DealIdsToReevaluate { get; set; }
        public IEnumerable<int> DealIdsToKeep { get; set; }
    }

    public sealed class GetDealDefinitionsToReevaluate : BaseAsyncActivity<GetDealDefinitionsToReevaluateInput, GetDealDefinitionsToReevaluateOutput>
    {
        [RequiredArgument]
        public InArgument<DateTime> DealEffectiveAfter { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<DealDefinition>> DealDefinitionsToReevaluate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<int>> DealIdsToReevaluate { get; set; }

        [RequiredArgument]
        public OutArgument<IEnumerable<int>> DealIdsToKeep { get; set; }

        protected override GetDealDefinitionsToReevaluateOutput DoWorkWithResult(GetDealDefinitionsToReevaluateInput inputArgument, AsyncActivityHandle handle)
        {
            var dealEffectiveAfter = inputArgument.DealEffectiveAfter;

            var cachedDealsWithDeleted = new DealDefinitionManager().GetAllCachedDealDefinitions(true);

            var dealIdsToKeep = new List<int>();
            var dealIdsToReevaluate = new List<int>();
            var dealDefinitionsToReevaluate = new List<DealDefinition>();

            if (cachedDealsWithDeleted != null)
            {
                foreach (var cachedDealKvp in cachedDealsWithDeleted)
                {
                    int dealId = cachedDealKvp.Key;
                    var dealDefinition = cachedDealKvp.Value;

                    if (dealDefinition.IsDeleted)
                    {
                        dealIdsToReevaluate.Add(dealId);
                        dealDefinitionsToReevaluate.Add(dealDefinition);
                        continue;
                    }

                    if (dealDefinition.Settings.RealBED == dealDefinition.Settings.RealEED)
                        continue;

                    if (dealDefinition.Settings.RealEED.VRGreaterThan(dealEffectiveAfter))
                    {
                        dealIdsToReevaluate.Add(dealId);
                        dealDefinitionsToReevaluate.Add(dealDefinition);
                    }
                    else
                    {
                        dealIdsToKeep.Add(dealId);
                    }
                }
            }

            if (dealDefinitionsToReevaluate.Count == 0)
            {
                dealDefinitionsToReevaluate = null;
                dealIdsToReevaluate = null;
            }

            if (dealIdsToKeep.Count == 0)
                dealIdsToKeep = null;

            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Getting Deals to reevaluate is done.");

            return new GetDealDefinitionsToReevaluateOutput()
            {
                DealDefinitionsToReevaluate = dealDefinitionsToReevaluate,
                DealIdsToReevaluate = dealIdsToReevaluate,
                DealIdsToKeep = dealIdsToKeep
            };
        }

        protected override GetDealDefinitionsToReevaluateInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new GetDealDefinitionsToReevaluateInput
            {
                DealEffectiveAfter = this.DealEffectiveAfter.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, GetDealDefinitionsToReevaluateOutput result)
        {
            context.SetValue(this.DealDefinitionsToReevaluate, result.DealDefinitionsToReevaluate);
            context.SetValue(this.DealIdsToReevaluate, result.DealIdsToReevaluate);
            context.SetValue(this.DealIdsToKeep, result.DealIdsToKeep);
        }
    }
}