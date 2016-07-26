using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Business
{
    public class VRMailManager
    {
        public void SendMail(VRMailMessageTemplate mailMessageTemplate, SendMailContext context)
        {

        }

        public void SendMail(VRMailMessage mailMessage)
        {
            
        }
    }

    public class SendMailContext
    {
        Dictionary<int, object> _businessEntitiesByDefinitionId;
        public SendMailContext(Dictionary<int, object> businessEntitiesByDefinitionId)
        {
            _businessEntitiesByDefinitionId = businessEntitiesByDefinitionId;
        }
    }
}
