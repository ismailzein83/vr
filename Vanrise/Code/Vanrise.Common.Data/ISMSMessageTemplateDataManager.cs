using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common.Data
{
    public interface ISMSMessageTemplateDataManager:IDataManager
    {
        List<SMSMessageTemplate> GetSMSMessageTemplates();

        bool AreSMSMessageTemplateUpdated(ref object updateHandle);

        bool Insert(SMSMessageTemplate smsMessageTemplateItem);

        bool Update(SMSMessageTemplate smsMessageTemplateItem);
    }
}
