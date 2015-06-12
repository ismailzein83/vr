using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Entities;
using Vanrise.Queueing;
using Vanrise.BusinessProcess;
using TOne.BusinessEntity.Entities;

namespace TOne.LCRProcess.Activities
{
    public class BuildSingleDestinationCodeMatchesInput
    {
        public CodeMatchesByCode CodeMatches { get; set; }
        public BaseQueue<SingleDestinationCodeMatches> OutputQueueForRouting { get; set; }
        public SupplierZoneRates SupplierZoneRates { get; set; }

        public bool RebuildZoneRates { get; set; }
    }

    public sealed class BuildSingleDestinationCodeMatches : BaseAsyncActivity<BuildSingleDestinationCodeMatchesInput>
    {
        [RequiredArgument]
        public InArgument<CodeMatchesByCode> CodeMatches { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<SingleDestinationCodeMatches>> OutputQueueForRouting { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneRates> SupplierZoneRates { get; set; }
        [RequiredArgument]
        public InArgument<bool> RebuildZoneRates { get; set; }
        protected override void DoWork(BuildSingleDestinationCodeMatchesInput inputArgument, AsyncActivityHandle handle)
        {
            foreach (var dCode in inputArgument.CodeMatches)
            {
                List<CodeMatch> codeMatches = new List<CodeMatch>();
                SingleDestinationCodeMatches singleDestinationCodeMatches = new SingleDestinationCodeMatches
                {
                    RouteCode = dCode.Key,
                    CodeMatchesBySupplierId = new CodeMatchesBySupplierId(),
                    OrderedCodeMatches = new List<CodeMatch>()
                };

                foreach (CodeMatch codeMatch in dCode.Value)
                {
                    RateInfo rate;
                    if (inputArgument.RebuildZoneRates && inputArgument.SupplierZoneRates.RatesByZoneId.TryGetValue(codeMatch.SupplierZoneId, out rate))
                        codeMatch.SupplierRate = rate;
                    if (String.Compare(codeMatch.SupplierId, "SYS", true) == 0)
                    {
                        singleDestinationCodeMatches.SysCodeMatch = codeMatch;
                    }
                    else
                    {
                        singleDestinationCodeMatches.CodeMatchesBySupplierId.Add(codeMatch.SupplierId, codeMatch);
                        singleDestinationCodeMatches.OrderedCodeMatches.Add(codeMatch);
                    }
                }
                singleDestinationCodeMatches.OrderedCodeMatches = singleDestinationCodeMatches.OrderedCodeMatches.OrderBy(c => c.SupplierRate.Rate).ToList();
                inputArgument.OutputQueueForRouting.Enqueue(singleDestinationCodeMatches);
            }
        }

        protected override BuildSingleDestinationCodeMatchesInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildSingleDestinationCodeMatchesInput()
            {
                CodeMatches = this.CodeMatches.Get(context),
                OutputQueueForRouting = this.OutputQueueForRouting.Get(context),
                SupplierZoneRates = this.SupplierZoneRates.Get(context),
                RebuildZoneRates = this.RebuildZoneRates.Get(context)
            };
        }
    }
}
