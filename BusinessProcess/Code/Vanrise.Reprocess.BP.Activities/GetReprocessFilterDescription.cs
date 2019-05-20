using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BusinessProcess;
using Vanrise.Reprocess.Entities;
using Vanrise.Entities;
using Vanrise.Reprocess.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;
using Vanrise.Common;

namespace Vanrise.Reprocess.BP.Activities
{
    public sealed class GetReprocessFilterDescription : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> ReprocessDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<ReprocessFilter> ReprocessFilter { get; set; }

        [RequiredArgument]
        public OutArgument<string> FilterDescription { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            Guid reprocessDefinitionId = this.ReprocessDefinitionId.Get(context);
            ReprocessFilter reprocessFilter = this.ReprocessFilter.Get(context);

            ReprocessDefinition reprocessDefinition = new ReprocessDefinitionManager().GetReprocessDefinition(reprocessDefinitionId);
            reprocessDefinition.ThrowIfNull("reprocessDefinition", reprocessDefinitionId);
            reprocessDefinition.Settings.ThrowIfNull("reprocessDefinition.Settings", reprocessDefinitionId);

            if (reprocessFilter == null || reprocessDefinition.Settings.FilterDefinition == null)
                return;

            ReprocessDefinitionSettings reprocessDefinitionSettings = reprocessDefinition.Settings;
            ReprocessFilterDefinition filterDefinition = reprocessDefinitionSettings.FilterDefinition;

            List<Guid> sourceRecordStorageIds = reprocessDefinitionSettings.SourceRecordStorageIds;
            sourceRecordStorageIds.ThrowIfNull("sourceRecordStorageIds", reprocessDefinitionId);

            Guid firstRecordStorageId = sourceRecordStorageIds.First();
            DataRecordStorage dataRecordStorage = new DataRecordStorageManager().GetDataRecordStorage(firstRecordStorageId);
            dataRecordStorage.ThrowIfNull("dataRecordStorage", firstRecordStorageId);

            Guid dataRecordTypeId = dataRecordStorage.DataRecordTypeId;
            DataRecordType dataRecordType = new DataRecordTypeManager().GetDataRecordType(dataRecordTypeId);
            dataRecordStorage.ThrowIfNull("dataRecordType", dataRecordTypeId);

            Vanrise.GenericData.Entities.RecordFilterGroup recordFilterGroup = filterDefinition.GetFilterGroup(new ReprocessFilterGetFilterGroupContext
            {
                ReprocessFilter = reprocessFilter,
                TargetDataRecordTypeId = dataRecordTypeId
            });

            Dictionary<string, RecordFilterFieldInfo> recordFilterFieldInfosByFieldName = BuildRecordFilterFieldInfosByFieldName(dataRecordType.Fields);

            string description = new RecordFilterManager().BuildRecordFilterGroupExpression(recordFilterGroup, recordFilterFieldInfosByFieldName);

            this.FilterDescription.Set(context, !string.IsNullOrEmpty(description)? $"Filter Query: {description}" : string.Empty);
        }

        private Dictionary<string, RecordFilterFieldInfo> BuildRecordFilterFieldInfosByFieldName(List<DataRecordField> dataRecordFields)
        {
            Func<DataRecordField, RecordFilterFieldInfo> recordFilterFieldInfoMapperFunc = (dataRecordField) =>
            {
                return new RecordFilterFieldInfo()
                {
                    Name = dataRecordField.Name,
                    Title = dataRecordField.Title,
                    Type = dataRecordField.Type
                };
            };

            return dataRecordFields.ToDictionary(item => item.Name, item => recordFilterFieldInfoMapperFunc(item));
        }
    }
}