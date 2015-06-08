using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Business;
using TOne.BusinessEntity.Business;
using TOne.BusinessEntity.Entities;
using TOne.Entities;
using TOne.LCR.Entities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;

namespace TOne.LCRProcess.Activities
{
    public class BuildCodeMatchByCodeGroupInput
    {

        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public List<String> DistinctCodes { get; set; }

        public BaseQueue<List<CodeMatch>> OutputQueue { get; set; }

        public BaseQueue<SingleDestinationCodeMatches> OutputQueueForRouting { get; set; }

        public SuppliersCodes SuppliersCodes { get; set; }

        public SupplierZoneRates SupplierZoneRates { get; set; }

        public Dictionary<string, CodeGroupInfo> CodeGroups { get; set; }

        public bool IsLcrOnly { get; set; }

        public BaseQueue<ZoneMatchWithCodeGroup> OutputQueueForZoneMatch { get; set; }
    }

    public class BuildCodeMatchByCodeGroup : BaseAsyncActivity<BuildCodeMatchByCodeGroupInput>
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<List<String>> DistinctCodes { get; set; }

        [RequiredArgument]
        public InArgument<SuppliersCodes> SupplierCodes { get; set; }

        [RequiredArgument]
        public InArgument<SupplierZoneRates> SupplierZoneRates { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsLcrOnly { get; set; }

        [RequiredArgument]
        public InArgument<Dictionary<string, CodeGroupInfo>> CodeGroups { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<List<CodeMatch>>> OutputQueue { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<SingleDestinationCodeMatches>> OutputQueueForRouting { get; set; }
        [RequiredArgument]
        public InOutArgument<BaseQueue<ZoneMatchWithCodeGroup>> OutputQueueForZoneMatch { get; set; }
        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<List<CodeMatch>>());

            if (this.OutputQueueForRouting.Get(context) == null)
                this.OutputQueueForRouting.Set(context, new MemoryQueue<SingleDestinationCodeMatches>());

            if (this.OutputQueueForZoneMatch.Get(context) == null)
                this.OutputQueueForZoneMatch.Set(context, new MemoryQueue<List<CodeMatch>>());

            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchByCodeGroupInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchByCodeGroupInput
            {
                SuppliersCodes = this.SupplierCodes.Get(context),
                SupplierZoneRates = this.SupplierZoneRates.Get(context),
                DistinctCodes = this.DistinctCodes.Get(context),
                EffectiveTime = this.EffectiveTime.Get(context),
                IsFuture = this.IsFuture.Get(context),
                IsLcrOnly = this.IsLcrOnly.Get(context),
                OutputQueue = this.OutputQueue.Get(context),
                OutputQueueForRouting = this.OutputQueueForRouting.Get(context),
                CodeGroups = this.CodeGroups.Get(context),
                OutputQueueForZoneMatch = this.OutputQueueForZoneMatch.Get(context)
            };
        }

        protected override void DoWork(BuildCodeMatchByCodeGroupInput inputArgument, AsyncActivityHandle handle)
        {
            CodeList distinctCodesList = new CodeList(inputArgument.DistinctCodes);

            foreach (var dCode in distinctCodesList.CodesWithPossibleMatches)
            {
                List<CodeMatch> codeMatches = new List<CodeMatch>();
                SingleDestinationCodeMatches singleDestinationCodeMatches = new SingleDestinationCodeMatches
                {
                    RouteCode = dCode.Key,
                    CodeMatchesBySupplierId = new CodeMatchesBySupplierId(),
                    OrderedCodeMatches = new List<CodeMatch>()
                };
                foreach (var suppCodes in inputArgument.SuppliersCodes.Codes)
                {
                    Code supplierMatch = null;
                    int index = 0;
                    int possibleMatchesCount = dCode.Value.Count;
                    do
                    {
                        string possibleMatch = dCode.Value[index];
                        if (suppCodes.Value.Codes.TryGetValue(possibleMatch, out supplierMatch))
                        {
                            CodeMatch codeMatch = new CodeMatch
                            {
                                Code = dCode.Key,
                                SupplierCode = supplierMatch.Value,
                                SupplierId = supplierMatch.SupplierId,
                                SupplierZoneId = supplierMatch.ZoneId,
                                SupplierCodeId = supplierMatch.ID
                            };

                            if (String.Compare(codeMatch.SupplierId, "SYS", true) == 0)
                            {
                                singleDestinationCodeMatches.SysCodeMatch = codeMatch;
                            }
                            else
                            {
                                RateInfo rate;
                                if (inputArgument.SupplierZoneRates.RatesByZoneId.TryGetValue(supplierMatch.ZoneId, out rate))
                                {
                                    codeMatch.SupplierRate = rate;
                                    if (!inputArgument.IsLcrOnly)
                                    {
                                        singleDestinationCodeMatches.CodeMatchesBySupplierId.Add(suppCodes.Key, codeMatch);
                                        decimal rateValue = rate.Rate;
                                        bool isAddedToOrderedList = false;
                                        for (int i = 0; i < singleDestinationCodeMatches.OrderedCodeMatches.Count; i++)
                                        {
                                            decimal currentRate = singleDestinationCodeMatches.OrderedCodeMatches[i].SupplierRate.Rate;
                                            if (currentRate >= rateValue)
                                            {
                                                singleDestinationCodeMatches.OrderedCodeMatches.Insert(i, codeMatch);
                                                isAddedToOrderedList = true;
                                                break;
                                            }
                                        }
                                        if (!isAddedToOrderedList)
                                            singleDestinationCodeMatches.OrderedCodeMatches.Add(codeMatch);
                                    }
                                }
                            }
                            codeMatches.Add(codeMatch);
                        }
                        index++;
                    }
                    while (supplierMatch == null && index < possibleMatchesCount);
                }
                if (codeMatches.Count > 0)
                {
                    CodeGroupInfo codeGroup;
                    ZoneMatchWithCodeGroup saleCodeMatch = new ZoneMatchWithCodeGroup();
                    if (singleDestinationCodeMatches.SysCodeMatch != null)
                    {
                        if (inputArgument.CodeGroups.TryGetValue(singleDestinationCodeMatches.SysCodeMatch.Code, out codeGroup))
                        {
                            List<CodeMatch> exactCodeMatches = GetExactCodeMatches(codeMatches, inputArgument.CodeGroups);
                            saleCodeMatch.SaleZoneId = singleDestinationCodeMatches.SysCodeMatch.SupplierZoneId;
                            saleCodeMatch.IsMatchingCodeGroup = true;
                            saleCodeMatch.SupplierCodeMatches = exactCodeMatches;
                        }
                        else
                        {
                            saleCodeMatch.SaleZoneId = singleDestinationCodeMatches.SysCodeMatch.SupplierZoneId;
                            saleCodeMatch.IsMatchingCodeGroup = false;
                            saleCodeMatch.SupplierCodeMatches = CloneHelper.Clone<List<CodeMatch>>(singleDestinationCodeMatches.OrderedCodeMatches);
                        }
                        inputArgument.OutputQueueForZoneMatch.Enqueue(saleCodeMatch);
                        inputArgument.OutputQueue.Enqueue(codeMatches);
                        if (!inputArgument.IsLcrOnly)
                            inputArgument.OutputQueueForRouting.Enqueue(singleDestinationCodeMatches);
                    }
                }
            }
        }

        private List<CodeMatch> GetExactCodeMatches(List<CodeMatch> supplierCodeMatches, Dictionary<string, CodeGroupInfo> codeGroups)
        {
            List<CodeMatch> codeMatches = new List<CodeMatch>();
            foreach (var supplierCodeMatch in supplierCodeMatches)
            {
                if (codeGroups.ContainsKey(supplierCodeMatch.SupplierCode) && String.Compare(supplierCodeMatch.SupplierId, "SYS", true) == -1)
                {
                    codeMatches.Add(supplierCodeMatch);
                }
            }
            return codeMatches;
        }
    }
}
