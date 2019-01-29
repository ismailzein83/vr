using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.SMSBusinessEntity.Entities
{
    public class UpdateCustomerSMSDraftStatusInput
    {
        public int ProcessDraftID { get; set; }
        public ProcessStatus NewStatus { get; set; }
    }
}
