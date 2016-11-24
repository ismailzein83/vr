using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.BP.Arguments.Tasks
{
    public class NotificationTaskData : BPTaskData
    {
        public IEnumerable<CarrierAccountInfo> CustomersToBeNotified { get; set; }
    }
}
