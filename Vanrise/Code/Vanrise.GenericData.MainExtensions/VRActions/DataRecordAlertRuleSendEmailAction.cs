using System;
using System.Collections.Generic;
using Vanrise.Notification.Entities;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Notification;

namespace Vanrise.GenericData.MainExtensions.VRActions
{
    public class DataRecordAlertRuleSendEmailAction : VRAction
    {
        public Guid MailMessageTemplateId { get; set; }
        const string MAILTEMPLATE_DATARECORDOBJECTNAME = "DataRecord";

        public override void Execute(IVRActionExecutionContext context)
        {
            DataRecordAlertRuleActionEventPayload payload = context.EventPayload as DataRecordAlertRuleActionEventPayload;

            //DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
            //Type dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(payload.DataRecordTypeId);

            //dynamic record = Activator.CreateInstance(dataRecordRuntimeType) as dynamic;
            //record.FillDataRecordTypeFromDictionary(payload.OutputRecords);

            Dictionary<string, dynamic> mailTemplateObjects = new Dictionary<string, dynamic>();
            mailTemplateObjects.Add(MAILTEMPLATE_DATARECORDOBJECTNAME, payload.OutputDataRecord);

            new VRMailManager().SendMail(this.MailMessageTemplateId, mailTemplateObjects);
        }
    }
}