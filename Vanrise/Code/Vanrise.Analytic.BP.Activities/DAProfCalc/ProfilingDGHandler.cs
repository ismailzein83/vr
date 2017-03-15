using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions;
using Vanrise.Common.Business;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    public class ProfilingDGHandler : DataGroupingHandler
    {
        public DAProfCalcExecInput DAProfCalcExecInput { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }


        public override string GetItemGroupingKey(IDataGroupingHandlerGetItemGroupingKeyContext context)
        {
            return (context.Item as ProfilingDGItem).GroupingKey;
        }

        public override void UpdateExistingItemFromNew(IDataGroupingHandlerUpdateExistingFromNewContext context)
        {
            //Get Aggregate fields from Data AnalysisItemDefinitionId (OutputItemDefinitionId) and call UpdateExistingFromNew on the Aggregate
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            var dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(DAProfCalcExecInput.OutputItemDefinitionId);
            RecordProfilingOutputSettings recordProfilingOutputSettings = (dataAnalysisItemDefinition.Settings as RecordProfilingOutputSettings);

            ProfilingDGItem existingItem = context.Existing as ProfilingDGItem;
            ProfilingDGItem newItem = context.New as ProfilingDGItem;

            if (recordProfilingOutputSettings.AggregationFields == null || recordProfilingOutputSettings.AggregationFields.Count == 0)
                return;

            for (var index = 0; index < recordProfilingOutputSettings.AggregationFields.Count; index++)
            {
                var aggregationField = recordProfilingOutputSettings.AggregationFields[index];
                var existingItemAggregateState = existingItem.AggregateStates[index];
                var newItemAggregateState = newItem.AggregateStates[index];

                aggregationField.RecordAggregate.UpdateExistingFromNew(new DARecordAggregateUpdateExistingFromNewContext(existingItemAggregateState, newItemAggregateState));
            }

        }

        public override void FinalizeGrouping(IDataGroupingHandlerFinalizeGroupingContext context)
        {
            //Evaluate the Calculated Fields, generate the DAProfCalcOutputRecord. and call ProcessOutputRecord of the OutputRecordProcessor if available. or set it to FinalResult if not
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            RecordProfilingOutputSettings recordProfilingOutputSettings = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinitionSettings<RecordProfilingOutputSettings>(DAProfCalcExecInput.OutputItemDefinitionId);

            DAProfCalcOutputRecordProcessorProcessContext daProfCalcOutputRecordProcessorProcessContext = new DAProfCalcOutputRecordProcessorProcessContext()
            {
                OutputRecords = new List<DAProfCalcOutputRecord>()
            };

            RecordProfilingOutputSettingsManager recordProfilingOutputSettingsManager = new RecordProfilingOutputSettingsManager();
            Dictionary<string, DAProfCalcCalculationFieldDetail> daProfCalcCalculationFieldDetailDict = recordProfilingOutputSettingsManager.GetRecordProfilingCalculationFields(DAProfCalcExecInput.OutputItemDefinitionId);

            foreach (IDataGroupingItem item in context.GroupedItems)
            {
                ProfilingDGItem profilingDGItem = item as ProfilingDGItem;
                Dictionary<string, dynamic> groupingValues = new Dictionary<string, dynamic>(profilingDGItem.GroupingValues);

                Dictionary<string, dynamic> aggregationValues = new Dictionary<string, dynamic>(profilingDGItem.GroupingValues);

                for (var index = 0; index < recordProfilingOutputSettings.AggregationFields.Count; index++)
                {
                    var aggregationField = recordProfilingOutputSettings.AggregationFields[index];
                    var profilingDGItemAggregateState = profilingDGItem.AggregateStates[index];
                    var aggregateResult =  aggregationField.RecordAggregate.GetResult(new DARecordAggregateGetResultContext(profilingDGItemAggregateState));
                    aggregationValues.Add(aggregationField.FieldName, aggregateResult);
                    groupingValues.Add(aggregationField.FieldName, aggregateResult);
                }

                if (daProfCalcCalculationFieldDetailDict != null && daProfCalcCalculationFieldDetailDict.Count > 0)
                {
                    DAProfCalcGetMeasureValueContext daProfCalcGetMeasureValueContext = new DAProfCalcGetMeasureValueContext(profilingDGItem.GroupingValues, aggregationValues);
                    foreach (var daProfCalcCalculationFieldDetail in daProfCalcCalculationFieldDetailDict)
                    {
                        groupingValues.Add(daProfCalcCalculationFieldDetail.Value.Entity.FieldName, daProfCalcCalculationFieldDetail.Value.Evaluator.GetCalculationValue(daProfCalcGetMeasureValueContext));
                    }
                }

                DAProfCalcOutputRecord record = new DAProfCalcOutputRecord() { DAProfCalcExecInput = this.DAProfCalcExecInput, Records = groupingValues, GroupingKey = profilingDGItem.GroupingKey };
                daProfCalcOutputRecordProcessorProcessContext.OutputRecords.Add(record);
            }

            OutputRecordProcessor.ProcessOutputRecords(daProfCalcOutputRecordProcessorProcessContext);
        }

        private dynamic BuildRecord(Dictionary<string, dynamic> groupingValues)
        {
            throw new NotImplementedException();
        }

        private class DARecordAggregateUpdateExistingFromNewContext : IDARecordAggregateUpdateExistingFromNewContext
        {
            public DARecordAggregateState ExistingState { get; set; }

            public DARecordAggregateState NewState { get; set; }

            public DARecordAggregateUpdateExistingFromNewContext(DARecordAggregateState existingState, DARecordAggregateState newState)
            {
                this.ExistingState = existingState;
                this.NewState = newState;
            }
        }

        private class DAProfCalcOutputRecordProcessorProcessContext : IDAProfCalcOutputRecordProcessorProcessContext
        {
            public List<DAProfCalcOutputRecord> OutputRecords { get; set; }
        }

        private class DARecordAggregateGetResultContext : IDARecordAggregateGetResultContext
        {
            public DARecordAggregateState State { get; set; }

            public DARecordAggregateGetResultContext(DARecordAggregateState state)
            {
                this.State = state;
            }
        }
    }
}
