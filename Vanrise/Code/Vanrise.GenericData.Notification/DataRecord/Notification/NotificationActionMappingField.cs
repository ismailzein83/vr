using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.GenericData.Notification
{
    public enum NotificationActionMappingField
    {
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        AlertRuleTypeId = 0,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        NotificationTypeId = 1,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        AlertRuleId = 2,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        MinNotificationInterval = 3,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        AlertNotificationDescription = 4,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        UserId = 5,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        Notification = 6,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.StaticField)]
        AlertRuleLevelId = 7,
        [NotificationActionMappingField(MappingFieldType = NotificationActionMappingFieldType.DataRecordField)]
        RecordTypeField = 99
    }

    public enum NotificationActionMappingFieldType
    {
        StaticField = 0,
        DataRecordField = 1
    }

    public class NotificationActionMappingFieldAttribute : Attribute
    {
        static Dictionary<NotificationActionMappingField, NotificationActionMappingFieldAttribute> _cachedAttributes;
        static NotificationActionMappingFieldAttribute()
        {
            _cachedAttributes = new Dictionary<NotificationActionMappingField, NotificationActionMappingFieldAttribute>();
            foreach (var member in typeof(NotificationActionMappingField).GetFields())
            {
                NotificationActionMappingFieldAttribute mbrAttribute = member.GetCustomAttributes(typeof(NotificationActionMappingFieldAttribute), true).FirstOrDefault() as NotificationActionMappingFieldAttribute;
                if (mbrAttribute != null)
                    _cachedAttributes.Add((NotificationActionMappingField)Enum.Parse(typeof(NotificationActionMappingField), member.Name), mbrAttribute);
            }
        }

        public NotificationActionMappingFieldType MappingFieldType { get; set; }

        public static NotificationActionMappingFieldAttribute GetAttribute(NotificationActionMappingField status)
        {
            return _cachedAttributes.GetRecord(status);
        }
    }
}