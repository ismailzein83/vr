using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Vanrise.GenericData.Notification
{
    public enum NotificationActionMappingField
    {
        [NotificationActionMappingField()]
        AlertRuleTypeId = 0,
        [NotificationActionMappingField()]
        NotificationTypeId = 1,
        [NotificationActionMappingField()]
        AlertRuleId = 2,
        [NotificationActionMappingField()]
        MinNotificationInterval = 3,
        [NotificationActionMappingField()]
        AlertNotificationDescription = 4,
        [NotificationActionMappingField()]
        UserId = 5,
        [NotificationActionMappingField()]
        Notification = 6,
        [NotificationActionMappingField()]
        AlertRuleLevelId = 7,
        [NotificationActionMappingField()]
        RecordTypeField = 99
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

        public static NotificationActionMappingFieldAttribute GetAttribute(NotificationActionMappingField status)
        {
            return _cachedAttributes.GetRecord(status);
        }
    }

}