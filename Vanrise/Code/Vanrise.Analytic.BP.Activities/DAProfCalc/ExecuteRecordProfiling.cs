﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Queueing;
using Vanrise.GenericData.Entities;
using Vanrise.Analytic.Entities;
using Vanrise.Analytic.Business;
using Vanrise.Analytic.Entities.DataAnalysis.ProfilingAndCalculation.OutputDefinitions;
using Vanrise.Common.Business;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Analytic.BP.Activities.DAProfCalc
{
    #region Arguments

    public class ExecuteRecordProfilingInput
    {
        public Guid DAProfCalcDefinitionId { get; set; }

        public List<DAProfCalcExecInputDetail> DAProfCalcExecInputDetails { get; set; }

        public BaseQueue<RecordBatch> InputQueue { get; set; }

        public List<BaseQueue<DAProfCalcOutputRecordBatch>> OutputQueues { get; set; }

        public IDAProfCalcOutputRecordProcessor OutputRecordProcessor { get; set; }
    }

    public class ExecuteRecordProfilingOutput
    {

    }

    #endregion

    public sealed class ExecuteRecordProfiling : DependentAsyncActivity<ExecuteRecordProfilingInput, ExecuteRecordProfilingOutput>
    {
        [RequiredArgument]
        public InArgument<Guid> DAProfCalcDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<List<DAProfCalcExecInputDetail>> DAProfCalcExecInputDetails { get; set; }

        [RequiredArgument]
        public InArgument<BaseQueue<RecordBatch>> InputQueue { get; set; }

        [RequiredArgument]
        public InArgument<List<BaseQueue<DAProfCalcOutputRecordBatch>>> OutputQueues { get; set; }

        public InArgument<IDAProfCalcOutputRecordProcessor> OutputRecordProcessor { get; set; }

        protected override ExecuteRecordProfilingOutput DoWorkWithResult(ExecuteRecordProfilingInput inputArgument, AsyncActivityStatus previousActivityStatus, AsyncActivityHandle handle)
        {
            DataAnalysisItemDefinitionManager dataAnalysisItemDefinitionManager = new DataAnalysisItemDefinitionManager();
            Guid daDataRecordTypeId = GetDADataRecordTypeId(inputArgument.DAProfCalcDefinitionId);

            Dictionary<string, DataAnalysisInfo> dataAnalysisInfos = new Dictionary<string, DataAnalysisInfo>();
            Dictionary<Guid, RecordProfilingOutputSettings> daRecordProfilingOutputSettings = new Dictionary<Guid, RecordProfilingOutputSettings>();

            foreach (DAProfCalcExecInputDetail dAProfCalcExecInputItem in inputArgument.DAProfCalcExecInputDetails)
            {
                var outputItemDefinitionId = dAProfCalcExecInputItem.DAProfCalcExecInput.OutputItemDefinitionId;
                var dataAnalysisUniqueName = dAProfCalcExecInputItem.DataAnalysisUniqueName;

                var dataAnalysisItemDefinition = dataAnalysisItemDefinitionManager.GetDataAnalysisItemDefinition(outputItemDefinitionId);
                if (dataAnalysisItemDefinition == null)
                    throw new NullReferenceException(string.Format("dataAnalysisItemDefinition {0}", outputItemDefinitionId));

                var recordProfilingOutputSettings = GetRecordProfilingOutputSettings(dataAnalysisItemDefinition);
                if (!daRecordProfilingOutputSettings.ContainsKey(outputItemDefinitionId))
                    daRecordProfilingOutputSettings.Add(outputItemDefinitionId, recordProfilingOutputSettings);

                dataAnalysisInfos.Add(dataAnalysisUniqueName, new DataAnalysisInfo()
                {
                    DataAnalysisItemDefinition = dataAnalysisItemDefinition,
                    DistributedDataGrouper = new DistributedDataGrouper(dataAnalysisUniqueName, new ProfilingDGHandler { OutputItemDefinitionId = outputItemDefinitionId, OutputRecordProcessor = inputArgument.OutputRecordProcessor })
                });
            }

            ExecuteRecordProfilingOutput output = new ExecuteRecordProfilingOutput();
            bool hasItem = false;
            DoWhilePreviousRunning(previousActivityStatus, handle, () =>
            {
                do
                {
                    hasItem = inputArgument.InputQueue.TryDequeue((recordBatch) =>
                    {
                        foreach (KeyValuePair<string, DataAnalysisInfo> dataAnalysisInfo in dataAnalysisInfos)
                        {
                            Dictionary<string, ProfilingDGItem> profilingDGItems = dataAnalysisInfo.Value.ProfilingDGItems;

                            RecordProfilingOutputSettings settings;
                            daRecordProfilingOutputSettings.TryGetValue(dataAnalysisInfo.Value.DataAnalysisItemDefinition.DataAnalysisItemDefinitionId, out settings);

                            foreach (dynamic cdr in recordBatch.Records)
                            {
                                Dictionary<string, dynamic> groupingValues;
                                string groupingKey = BuildGroupingKey(settings, cdr, out groupingValues);
                                ProfilingDGItem profilingDGItem;
                                if (!profilingDGItems.TryGetValue(groupingKey, out profilingDGItem))
                                {
                                    profilingDGItem = new ProfilingDGItem() { GroupingKey = groupingKey, AggregateStates = new List<DARecordAggregateState>(), GroupingValues = new Dictionary<string, dynamic>(groupingValues) };
                                    if (settings != null && settings.AggregationFields != null)
                                    {
                                        foreach (DAProfCalcAggregationField aggregationField in settings.AggregationFields)
                                        {
                                            DARecordAggregateState state = aggregationField.RecordAggregate.CreateState(null);
                                            profilingDGItem.AggregateStates.Add(state);
                                        }
                                    }
                                    profilingDGItems.Add(groupingKey, profilingDGItem);
                                }

                                if (settings != null && settings.AggregationFields != null)
                                {
                                    for (var index = 0; index < settings.AggregationFields.Count; index++)
                                    {
                                        var aggregationField = settings.AggregationFields[index];
                                        aggregationField.RecordAggregate.Evaluate(new DARecordAggregateEvaluationContext(daDataRecordTypeId, cdr, profilingDGItem.AggregateStates[index]));
                                    }
                                }
                            }
                            if (profilingDGItems.Count >= 10000)
                            {
                                DistributedDataGrouper dataGrouper = dataAnalysisInfo.Value.DistributedDataGrouper;
                                dataGrouper.DistributeGroupingItems(profilingDGItems.Values.Select(itm => itm as IDataGroupingItem).ToList());
                                profilingDGItems = new Dictionary<string, ProfilingDGItem>();
                            }
                        }
                    });
                }
                while (!ShouldStop(handle) && hasItem);
            });

            foreach (KeyValuePair<string, DataAnalysisInfo> dataAnalysisInfo in dataAnalysisInfos)
            {
                Dictionary<string, ProfilingDGItem> profilingDGItems = dataAnalysisInfo.Value.ProfilingDGItems;
                if (profilingDGItems.Count > 0)
                {
                    DistributedDataGrouper dataGrouper = dataAnalysisInfo.Value.DistributedDataGrouper;
                    dataGrouper.DistributeGroupingItems(profilingDGItems.Values.Select(itm => itm as IDataGroupingItem).ToList());
                }
            }

            return output;
        }

        protected override ExecuteRecordProfilingInput GetInputArgument2(AsyncCodeActivityContext context)
        {
            return new ExecuteRecordProfilingInput()
            {
                DAProfCalcDefinitionId = DAProfCalcDefinitionId.Get(context),
                DAProfCalcExecInputDetails = DAProfCalcExecInputDetails.Get(context),
                InputQueue = InputQueue.Get(context),
                OutputQueues = OutputQueues.Get(context),
                OutputRecordProcessor = OutputRecordProcessor.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, ExecuteRecordProfilingOutput result)
        {
            context.GetSharedInstanceData().WriteTrackingMessage(LogEntryType.Information, "Execute Record Profiling is done", null);
        }

        private RecordProfilingOutputSettings GetRecordProfilingOutputSettings(DataAnalysisItemDefinition dataAnalysisItemDefinition)
        {
            if (dataAnalysisItemDefinition.Settings == null)
                throw new NullReferenceException(string.Format("dataAnalysisItemDefinition.Settings {0}", dataAnalysisItemDefinition.DataAnalysisItemDefinitionId));

            RecordProfilingOutputSettings recordProfilingOutputSettings = (dataAnalysisItemDefinition.Settings as RecordProfilingOutputSettings);
            if (recordProfilingOutputSettings == null)
                throw new Exception(String.Format("recordProfilingOutputSettings is not of type RecordProfilingOutputSettings. It is of type '{0}'", dataAnalysisItemDefinition.Settings.GetType()));

            return recordProfilingOutputSettings;
        }
        private Guid GetDADataRecordTypeId(Guid daProfCalcDefinitionId)
        {
            DataAnalysisDefinitionManager dataAnalysisDefinitionManager = new DataAnalysisDefinitionManager();
            DataAnalysisDefinition dataAnalysisDefinition = dataAnalysisDefinitionManager.GetDataAnalysisDefinition(daProfCalcDefinitionId);
            if (dataAnalysisDefinition == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition {0}", daProfCalcDefinitionId));

            if (dataAnalysisDefinition.Settings == null)
                throw new NullReferenceException(string.Format("dataAnalysisDefinition.Settings {0}", daProfCalcDefinitionId));

            DAProfCalcSettings daProfCalcSettings = dataAnalysisDefinition.Settings as DAProfCalcSettings;
            if (daProfCalcSettings == null)
                throw new Exception(String.Format("dataAnalysisDefinition.Settings is not of type DAProfCalcSettings. It is of type '{0}'", dataAnalysisDefinition.Settings.GetType()));

            return daProfCalcSettings.DataRecordTypeId;
        }

        private string BuildGroupingKey(RecordProfilingOutputSettings settings, dynamic cdr, out Dictionary<string, dynamic> groupingValues)
        {
            groupingValues = new Dictionary<string, dynamic>();
            if (settings == null || settings.GroupingFields == null || settings.GroupingFields.Count == 0)
                return string.Empty;

            StringBuilder strBuilder = new StringBuilder();
            foreach (DAProfCalcGroupingField groupingField in settings.GroupingFields)
            {
                var groupingFieldValue = Vanrise.Common.Utilities.GetPropValueReader(groupingField.FieldName).GetPropertyValue(cdr);
                strBuilder.AppendFormat("{0}@", groupingFieldValue != null ? groupingFieldValue : string.Empty);
                groupingValues.Add(groupingField.FieldName, groupingFieldValue);
            }
            return strBuilder.ToString();
        }

        private class DARecordAggregateEvaluationContext : IDARecordAggregateEvaluationContext
        {
            public Guid DataRecordTypeId { get; set; }

            public dynamic Record { get; set; }

            public DARecordAggregateState State { get; set; }

            public DARecordAggregateEvaluationContext(Guid dataRecordTypeId, dynamic record, DARecordAggregateState state)
            {
                this.DataRecordTypeId = dataRecordTypeId;
                this.Record = record;
                this.State = state;
            }
        }

        private class DataAnalysisInfo
        {
            public DataAnalysisInfo()
            {
                ProfilingDGItems = new Dictionary<string, ProfilingDGItem>();
            }
            public DataAnalysisItemDefinition DataAnalysisItemDefinition { get; set; }

            public DistributedDataGrouper DistributedDataGrouper { get; set; }

            public Dictionary<string, ProfilingDGItem> ProfilingDGItems { get; set; }
        }
    }
}
