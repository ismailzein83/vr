using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBulkAction
{
    public class SendEmailAccountBulkAction : AccountBulkActionRuntimeSettings
    {
        public Guid MailMessageTemplateId { get; set; }
    }
}
