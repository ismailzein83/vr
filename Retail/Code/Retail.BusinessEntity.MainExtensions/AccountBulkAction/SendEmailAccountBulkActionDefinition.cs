using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;

namespace Retail.BusinessEntity.MainExtensions.AccountBulkAction
{
    public class SendEmailAccountBulkActionDefinition : AccountBulkActionSettings
    {
        public Guid MailMessageTypeId { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("81A1FFFA-8AE6-41F0-A3E2-ED6457F72FDB"); }
        }

        public override string RuntimeEditor
        {
            get { return "retail-be-accountbulkactionsettings-sendemail-runtimeeditor"; }
        }

    }
}
