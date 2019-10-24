using System;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.Notification
{
    public abstract class OnDataRecordNotificationCreatedAction
    {
        public abstract Guid ConfigId { get; }

        public abstract string ActionName { get; }

        public abstract void Execute(IOnDataRecordNotificationCreatedExecutionContext context);
    }

    public interface IOnDataRecordNotificationCreatedExecutionContext
    {
        dynamic GetFieldValue(NotificationActionMappingField fieldName, string specificFieldName);
    }

    public class OnDataRecordNotificationCreatedExecutionContext : IOnDataRecordNotificationCreatedExecutionContext
    {
        Dictionary<string, dynamic> _fieldValues;
        Dictionary<NotificationActionMappingField, dynamic> _enumValuesByName;
        Guid _dataRecordTypeId;
        dynamic _object;

        public OnDataRecordNotificationCreatedExecutionContext(Dictionary<string, dynamic> fieldValues, Guid dataRecordTypeId, Dictionary<NotificationActionMappingField, dynamic> enumFieldValues)
        {
            _fieldValues = fieldValues;
            _dataRecordTypeId = dataRecordTypeId;
            _enumValuesByName = enumFieldValues;
        }

        public dynamic GetFieldValue(NotificationActionMappingField fieldName, string specificFieldName)
        {
            var mappingFieldType = NotificationActionMappingFieldAttribute.GetAttribute(fieldName).MappingFieldType;
            switch (mappingFieldType)
            {
                case NotificationActionMappingFieldType.StaticField:
                    if (_enumValuesByName.TryGetValue(fieldName, out dynamic value))
                        return value;
                    throw new Exception($"Unable to get value for the field: {fieldName}");

                case NotificationActionMappingFieldType.DataRecordField:
                    specificFieldName.ThrowIfNull("specificFieldName");
                    return GetDataRecordTypeFieldValue(specificFieldName);

                default: throw new NotSupportedException($"Enum NotificationActionMappingFieldType: {mappingFieldType}");
            }
        }

        private dynamic GetDataRecordTypeFieldValue(string fieldName)
        {
            if (_object == null)
                _object = new DataRecordObject(_dataRecordTypeId, _fieldValues);

            return _object.GetFieldValue(fieldName);
        }
    }
}