using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.GenericData.Business;
using Vanrise.Common;
using Vanrise.Entities;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEOnAfterSaveHandlers
{
    public class SendEmailAfterSaveHandler : GenericBEOnAfterSaveHandler
    {
        public override Guid ConfigId
        {
            get { return new Guid("C21FE60A-1AC2-4424-9B27-2A6EB237AB12"); }
        }
        public override void Execute(IGenericBEOnAfterSaveHandlerContext context)
        {
            if (base.HandlerOperationTypes == null || base.HandlerOperationTypes.Contains(context.OperationType))
            {
                this.EntityObjectName.ThrowIfNull("EntityObjectName");
                this.InfoType.ThrowIfNull("InfoType");

                VRMailManager vrMailManager = new VRMailManager();
                Dictionary<string, dynamic> objects = new Dictionary<string, dynamic>();
                DataRecordTypeManager dataRecordTypeManager = new DataRecordTypeManager();
                var dataRecordRuntimeType = dataRecordTypeManager.GetDataRecordRuntimeType(context.DefinitionSettings.DataRecordTypeId);
                var dataRecord = Activator.CreateInstance(dataRecordRuntimeType, context.NewEntity.FieldValues);
                objects.Add(EntityObjectName, dataRecord);
                GenericBusinessEntityDefinitionManager genericBusinessEntityDefinitionManager = new GenericBusinessEntityDefinitionManager();
                if (SendEmailObjectsInfo != null)
                {
                    foreach(var sendEmailObjectInfo in SendEmailObjectsInfo)
                    {
                        var objectValue = genericBusinessEntityDefinitionManager.GetExtendedSettingsInfoByType(context.DefinitionSettings, sendEmailObjectInfo.InfoType, context.NewEntity);
                        objectValue.ThrowIfNull("objectValue", sendEmailObjectInfo.InfoType);
                        objects.Add(sendEmailObjectInfo.ObjectName, objectValue);
                    }
                }

                var mailTemplateId = genericBusinessEntityDefinitionManager.GetExtendedSettingsInfoByType(context.DefinitionSettings, this.InfoType);
                if (mailTemplateId != null)
                {
                    List<VRMailAttachement> vrMailAttachments = new List<VRMailAttachement>();

                    if (!string.IsNullOrEmpty(WithAttachmentsFieldName) && !string.IsNullOrEmpty(AttachmentsFieldName))
                    {
                        bool withAttachments = (bool)context.NewEntity.FieldValues.GetRecord(WithAttachmentsFieldName);

                        if (withAttachments)
                        {
                            var attachments = (List<AttachmentFieldTypeEntity>)context.NewEntity.FieldValues.GetRecord(AttachmentsFieldName);

                            if (attachments != null && attachments.Count > 0)
                            {
                                vrMailAttachments = attachments.MapRecords(x => vrMailManager.ConvertToGeneralAttachement(x.FileId)).ToList();
                            }
                        }
                    }

                    var emailTemplateEvaluator = vrMailManager.EvaluateMailTemplate((Guid)mailTemplateId, objects);
                    emailTemplateEvaluator.ThrowIfNull("emailTemplateEvaluator");

                    vrMailManager.SendMail(emailTemplateEvaluator.From, emailTemplateEvaluator.To, emailTemplateEvaluator.CC, emailTemplateEvaluator.BCC, emailTemplateEvaluator.Subject, emailTemplateEvaluator.Body, vrMailAttachments);
                    new GenericBusinessEntityManager().LogObjectCustomAction(context.BusinessEntityDefinitionId, context.NewEntity, "Send Email", true, null);
                }
            }
        }
        public string WithAttachmentsFieldName { get; set; }
        public string AttachmentsFieldName { get; set; }
        public string EntityObjectName { get; set; }
        public string InfoType { get; set; }
        public List<SendEmailObjectInfo> SendEmailObjectsInfo { get; set; }
    }
    public class SendEmailObjectInfo
    {
        public Guid SendEmailObjectInfoId { get; set; }
        public string InfoType { get; set; }
        public string ObjectName { get; set; }

    }
}
