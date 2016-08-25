using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public class ProfilingDGHandler : DataGroupingHandler
    {
        public Guid OutputItemDefinitionId { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }


        public override string GetItemGroupingKey(IDataGroupingHandlerGetItemGroupingKeyContext context)
        {
            return (context.Item as ProfilingDGItem).GroupingKey;
            throw new NotImplementedException();
        }

        public override void UpdateExistingItemFromNew(IDataGroupingHandlerUpdateExistingFromNewContext context)
        {
            //Get Aggregate fields from Data AnalysisItemDefinitionId (OutputItemDefinitionId) and call UpdateExistingFromNew on the Aggregate
            throw new NotImplementedException();
        }

        public override void FinalizeGrouping(IDataGroupingHandlerFinalizeGroupingContext context)
        {
            //Evaluate the Calculated Fields, generate the DAProfCalcOutputRecord. and call ProcessOutputRecord of the OutputRecordProcessor if available. or set it to FinalResult if not
            base.FinalizeGrouping(context);
        }
    }
}
