using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.LCR.Entities;
using TOne.Entities;
using TOne.LCR.Data;

namespace TOne.LCRProcess.Activities
{
    #region Argument Classes

    public class BuildCodeMatchByFirstCodeDigitInput
    {
        public Char FirstDigit { get; set; }

        public DateTime EffectiveOn { get; set; }

        public List<SupplierCodeInfo> SupplierCodeInfo { get; set; }

        public TOneQueue<List<CodeMatch>> OutputQueue { get; set; }
    }

    #endregion

    public sealed class BuildCodeMatchByFirstCodeDigit : BaseAsyncActivity<BuildCodeMatchByFirstCodeDigitInput>
    {
        [RequiredArgument]
        public InArgument<Char> FirstDigit { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveOn { get; set; }

        [RequiredArgument]
        public InArgument<List<SupplierCodeInfo>> SupplierCodeInfo { get; set; }

        [RequiredArgument]
        public InOutArgument<TOneQueue<List<CodeMatch>>> OutputQueue { get; set; }

        protected override void OnBeforeExecute(AsyncCodeActivityContext context, AsyncActivityHandle handle)
        {
            if (this.OutputQueue.Get(context) == null)
                this.OutputQueue.Set(context, new TOneQueue<List<CodeMatch>>());
            base.OnBeforeExecute(context, handle);
        }

        protected override BuildCodeMatchByFirstCodeDigitInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new BuildCodeMatchByFirstCodeDigitInput
            {
                FirstDigit = this.FirstDigit.Get(context),
                EffectiveOn = this.EffectiveOn.Get(context),
                SupplierCodeInfo = this.SupplierCodeInfo.Get(context),
                OutputQueue = this.OutputQueue.Get(context)
            };
        }

        protected override void DoWork(BuildCodeMatchByFirstCodeDigitInput inputArgument, AsyncActivityHandle handle)
        {
            DateTime start = DateTime.Now;
            ICodeDataManager dataManager = LCRDataManagerFactory.GetDataManager<ICodeDataManager>();
            List<string> distinctCodes;
            Dictionary<string, Dictionary<string, LCRCode>> suppliersCodes = dataManager.GetCodesForActiveSuppliers(inputArgument.FirstDigit, inputArgument.EffectiveOn, inputArgument.SupplierCodeInfo, out distinctCodes);
            Console.WriteLine("{0}: Code Loaded for Digit '{1}'", DateTime.Now, inputArgument.FirstDigit);

            CodeList distinctCodesList = new CodeList(distinctCodes);
            int codeMatchCount = 0;

            List<CodeMatch> codeMatches = new List<CodeMatch>();
            foreach (var dCode in distinctCodesList.CodesWithPossibleMatches)
            {
                foreach (var suppCodes in suppliersCodes)
                {
                    LCRCode supplierMatch = null;
                    int index = 0;
                    do
                    {
                        string possibleMatch = dCode.Value[index];
                        if(suppCodes.Value.TryGetValue(possibleMatch, out supplierMatch))
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
                if (codeMatches.Count > 250000)
                {
                    inputArgument.OutputQueue.Enqueue(codeMatches);
                    codeMatchCount += codeMatches.Count;
                    codeMatches = new List<CodeMatch>();
                }
            }
            
            //dataManager.LoadCodeMatchesFromDistinctCodes(inputArgument.DistinctCodes, inputArgument.EffectiveOn, inputArgument.SupplierCodeInfo, (codeMatch) =>
            //    {
            //        codeMatches.Add(codeMatch);
            //        if (codeMatches.Count > 50000)
            //        {
            //            inputArgument.OutputQueue.Enqueue(codeMatches);
            //            codeMatchCount +=codeMatches.Count;
            //            codeMatches = new List<CodeMatch>();
            //        }
            //    });
            if(codeMatches.Count > 0)
                inputArgument.OutputQueue.Enqueue(codeMatches);
            codeMatchCount +=codeMatches.Count;
            //Console.WriteLine("{0}: {1} Code Matches ready in {2}", DateTime.Now, codeMatchCount, (DateTime.Now - start));
        }
    }
}
