using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Data;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class ValidationMessageManager
    {
        public void Insert(IEnumerable<ValidationMessage> messages)
        {
            IBPValidationMessageDataManager dataManager = BPDataManagerFactory.GetDataManager<IBPValidationMessageDataManager>();
            dataManager.Insert(messages);
        }
    }
}
