using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Security.Entities
{
    public class MailMessageTemplateSettings
    {
        public Guid NewUserId { get; set; }

        public Guid ResetPasswordId { get; set; }

        public Guid ForgotPasswordId { get; set; } 
    }
}
