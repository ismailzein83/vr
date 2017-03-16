using System;
using Vanrise.Notification.Entities;

namespace Vanrise.GenericData.Notification
{
    public class DataRecordSendEmailDefinitionSettings : VRActionDefinitionExtendedSettings
    {
        public override Guid ConfigId { get { return new Guid("3B904E8C-2AC0-43DB-A4EF-425869D40544"); } }

        public override string RuntimeEditor { get { return "vr-genericdata-datarecord-vraction-sendemail"; } }

        public Guid DataRecordTypeId { get; set; }

        public Guid MailMessageTypeId { get; set; }

        public override bool IsApplicable(IVRActionDefinitionIsApplicableContext context)
        {
            DataRecordActionTargetType dataRecordActionTargetType = context.Target as DataRecordActionTargetType;
            if (dataRecordActionTargetType == null)
                return false;

            if (dataRecordActionTargetType.DataRecordTypeId != DataRecordTypeId)
                return false;

            return true;
        }
    }
}