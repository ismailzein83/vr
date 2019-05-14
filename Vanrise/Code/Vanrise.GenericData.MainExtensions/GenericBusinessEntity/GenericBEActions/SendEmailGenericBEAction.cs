using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;

namespace Vanrise.GenericData.MainExtensions.GenericBusinessEntity.GenericBEActions
{
    public class SendEmailGenericBEAction : GenericBEActionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("CBAFD46F-676F-4CC1-ADA4-4DF7ED762C95"); }
        }
        public override string ActionTypeName { get { return "SendEmailGenericBEAction"; } }
        public override string ActionKind { get { return "SendEmail"; } }
        public Guid? MailMessageTypeId { get; set; }
        public string EntityObjectName { get; set; }
        public string InfoType { get; set; }
        public List<SendEmailObjectInfo> SendEmailObjectsInfo { get; set; }
        public override bool DoesUserHaveAccess(IGenericBEActionDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
    public class SendEmailObjectInfo
    {
        public Guid SendEmailObjectInfoId { get; set; }
        public string InfoType { get; set; }
        public string ObjectName { get; set; }

    }
}
