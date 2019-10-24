using System;
using System.Collections.Generic;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;
using Vanrise.Common;

namespace Vanrise.GenericData.MainExtensions.VRActions
{
    public class OnDataRecordNotificationCreatedSaveAction : OnDataRecordNotificationCreatedAction
    {
        public override Guid ConfigId { get { return new Guid("FD03C559-7901-4A45-9DAD-24135E2CF804"); } }

        public override string ActionName { get { return "Save Record"; } }

        public Guid DataRecordStorageId { get; set; }

        public List<SaveRecordActionGridItemMapping> GridItemsMapping { get; set; }

        public override void Execute(IOnDataRecordNotificationCreatedExecutionContext context)
        {
            GridItemsMapping.ThrowIfNull("GridItemsMapping");

            Dictionary<string, object> fieldValuesByName = new Dictionary<string, object>();

            foreach (var itemMapping in GridItemsMapping)
                fieldValuesByName.Add(itemMapping.RecordFieldName, context.GetFieldValue(itemMapping.MappingFieldType, itemMapping.MappingFieldName));

            var userId = context.GetFieldValue(NotificationActionMappingField.UserId, null);
            new DataRecordStorageManager().AddDataRecord(DataRecordStorageId, fieldValuesByName, userId, out Object insertedId, out bool hasInsertedId);
        }
    }

    public class SaveRecordActionGridItemMapping
    {
        public string RecordFieldName { get; set; }

        public NotificationActionMappingField MappingFieldType { get; set; }

        public string MappingFieldName { get; set; }
    }
}