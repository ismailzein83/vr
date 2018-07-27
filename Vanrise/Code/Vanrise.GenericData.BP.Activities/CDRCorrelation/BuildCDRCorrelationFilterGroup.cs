﻿using System;
using System.Activities;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.BP.Activities
{

    public sealed class BuildCDRCorrelationFilterGroup : CodeActivity
    {
        [RequiredArgument]
        public InArgument<CDRCorrelationDefinition> CDRCorrelationDefinition { get; set; }

        [RequiredArgument]
        public InArgument<long?> LastImportedId { get; set; }

        [RequiredArgument]
        public OutArgument<RecordFilterGroup> RecordFilterGroup { get; set; }

        [RequiredArgument]
        public OutArgument<bool> NewDataImported { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            CDRCorrelationDefinition cdrCorrelationDefinition = this.CDRCorrelationDefinition.Get(context);
            long? lastImportedId = this.LastImportedId.Get(context);
            CDRCorrelationDefinitionSettings settings = cdrCorrelationDefinition.Settings;

            bool newDataImported = false;

            RecordFilterGroup recordFilter = new RecordFilterGroup { LogicalOperator = RecordQueryLogicalOperator.And, Filters = new List<RecordFilter>() };
            recordFilter.Filters.Add(new NumberListRecordFilter { FieldName = "RecordType", CompareOperator = ListRecordFilterOperator.In, Values = new List<decimal>() { 0, 1 } });

            if (!lastImportedId.HasValue)
            {
                DataRecordStorageManager manager = new DataRecordStorageManager();
                DateTime? minDate = manager.GetMinDateTimeAfterId(settings.InputDataRecordStorageId, lastImportedId.Value, settings.IdFieldName, settings.DatetimeFieldName);

                if (minDate.HasValue)
                {
                    newDataImported = true;
                    recordFilter.Filters.Add(new DateTimeRecordFilter { FieldName = settings.DatetimeFieldName, CompareOperator = DateTimeRecordFilterOperator.GreaterOrEquals, Value = minDate.Value });
                }
            }
            else
            {
                newDataImported = true;
            }

            this.RecordFilterGroup.Set(context, recordFilter);
            this.NewDataImported.Set(context, newDataImported);
        }
    }
}