using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification.DataRecord
{
    public class DataRecordNotificationTypeSettings : VRNotificationTypeExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("E64C51A2-08E0-4B7D-96F0-9FF1848A72FA"); } }

        public override string SearchRuntimeEditor { get { return "vr-genericdata-notificationtypesettings-searcheditor"; } }

        public override string BodyRuntimeEditor { get { return "vr-genericdata-notificationtypesettings-bodyeditor"; } }

        public Guid DataRecordTypeId { get; set; }

        public List<NotificationGridColumnDefinition> GridColumnDefinitions { get; set; }
    }

    public class NotificationGridColumnDefinition
    {
        public string FieldName { get; set; }

        public string Header { get; set; }
    }
}