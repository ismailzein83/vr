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
        public String CodePrefixGroup { get; set; }

        public DateTime EffectiveTime { get; set; }

        public bool IsFuture { get; set; }

        public List<CarrierAccountInfo> ActiveSuppliers { get; set; }

        public BaseQueue<List<CodeMatch>> OutputQueue { get; set; }
    }

    public class BuildCodeMatchByCodeGroup : BaseAsyncActivity<BuildCodeMatchByCodeGroupInput>
    {

        [RequiredArgument]
        public InArgument<String> CodePrefixGroup { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveTime { get; set; }

        [RequiredArgument]
        public InArgument<bool> IsFuture { get; set; }

        [RequiredArgument]
        public InArgument<List<CarrierAccountInfo>> ActiveSuppliers { get; set; }

        [RequiredArgument]
        public InOutArgument<BaseQueue<List<CodeMatch>>> OutputQueue { get; set; }


        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new MemoryQueue<List<CodeMatch>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchByCodeGroupInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchByCodeGroupInput
            {
                CodePrefixGroup = this.CodePrefixGroup.Get(context),
                EffectiveTime = this.EffectiveTime.Get(context),
                IsFuture = this.IsFuture.Get(context),
                ActiveSuppliers = this.ActiveSuppliers.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(BuildCodeMatchByCodeGroupInput inputArgument, AsyncActivityHandle handle)
        {

            DateTime start = DateTime.Now;
            //Console.WriteLine("{0}: Code Load Started for Code Prefix Group '{1}'", DateTime.Now, inputArgument.CodePrefixGroup);
            CodeManager codeManager = new CodeManager();
            List<string> distinctCodes;
            SuppliersCodes suppliersCodes = codeManager.GetCodesByCodePrefixGroup(inputArgument.CodePrefixGroup, inputArgument.EffectiveTime, inputArgument.IsFuture, inputArgument.ActiveSuppliers, out distinctCodes);
            //Console.WriteLine("{0}: Code Loaded for Code Prefix Group '{1}'", DateTime.Now, inputArgument.CodePrefixGroup);

            CodeList distinctCodesList = new CodeList(distinctCodes);
            int codeMatchCount = 0;

            List<CodeMatch> codeMatches = new List<CodeMatch>();
            int bcpBatchSize = ConfigParameterManager.Current.GetBCPBatchSize();
            foreach (var dCode in distinctCodesList.CodesWithPossibleMatches)
            {
                foreach (var suppCodes in suppliersCodes.Codes)
                {
                    Code supplierMatch = null;
                    int index = 0;
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
                            codeMatches.Add(codeMatch);
                        }
                        index++;
                    }
                    while (supplierMatch == null && index < dCode.Value.Count);
                }
                if (codeMatches.Count > bcpBatchSize)
                {
                    inputArgument.OutputQueue.Enqueue(codeMatches);
                    codeMatchCount += codeMatches.Count;
                    codeMatches = new List<CodeMatch>();
                }
            }

            if (codeMatches.Count > 0)
                inputArgument.OutputQueue.Enqueue(codeMatches);
            codeMatchCount += codeMatches.Count;
        }
    }
}
