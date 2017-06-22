﻿using System;
using System.Collections.Generic;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.GenericData.Business;

namespace Vanrise.Reprocess.BP.Activities
{
    #region Arguments Classes

    public class LoadDataToReprocessInput
    {
        public List<Guid> RecordStorageIds { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public StageManager StageManager { get; set; }

        public List<string> OutputStageNames { get; set; }
    }

    public class LoadDataToReprocessOutput
    {
        public int EventCount { get; set; }
    }

    #endregion

    public sealed class LoadDataToReprocess : BaseAsyncActivity<LoadDataToReprocessInput, LoadDataToReprocessOutput>
    {
        [RequiredArgument]
        public InArgument<List<Guid>> RecordStorageIds { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> FromTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> ToTime { get; set; }

        [RequiredArgument]
        public InArgument<StageManager> StageManager { get; set; }

        [RequiredArgument]
        public InArgument<List<string>> OutputStageNames { get; set; }

        [RequiredArgument]
        public OutArgument<int> EventCount { get; set; }

        protected override LoadDataToReprocessInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new LoadDataToReprocessInput
            {
                RecordStorageIds = this.RecordStorageIds.Get(context),
                FromTime = this.FromTime.Get(context),
                ToTime = this.ToTime.Get(context),
                StageManager = this.StageManager.Get(context),
                OutputStageNames = this.OutputStageNames.Get(context)
            };
        }

        protected override LoadDataToReprocessOutput DoWorkWithResult(LoadDataToReprocessInput inputArgument, AsyncActivityHandle handle)
        {
            LoadDataToReprocessOutput output = new LoadDataToReprocessOutput() { EventCount = 0 };

            if (inputArgument.OutputStageNames == null || inputArgument.OutputStageNames.Count == 0)
                throw new Exception("No output stages!");

            DataRecordStorageManager manager = new DataRecordStorageManager();
            GenericDataRecordBatch batch = new GenericDataRecordBatch() { Records = new List<dynamic>() };

            foreach (var recordStorageId in inputArgument.RecordStorageIds)
            {
                manager.GetDataRecords(recordStorageId, inputArgument.FromTime, inputArgument.ToTime, ((itm) =>
                {
                    output.EventCount++;

                    batch.Records.Add(itm);
                    if (batch.Records.Count >= 10000)
                    {
                        foreach (string stageName in inputArgument.OutputStageNames)
                        {
                            inputArgument.StageManager.EnqueueBatch(stageName, batch);
                        }
                        batch = new GenericDataRecordBatch() { Records = new List<dynamic>() };
                    }
                }));
            }

            if (batch.Records.Count > 0)
            {
                foreach (string stageName in inputArgument.OutputStageNames)
                {
                    inputArgument.StageManager.EnqueueBatch(stageName, batch);
                }
            }
            handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, "Loading Source Records is done. Events Count: {0}", output.EventCount);
            return output;
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, LoadDataToReprocessOutput result)
        {
            this.EventCount.Set(context, result.EventCount);
        }
    }
}