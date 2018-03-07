using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class UpdateDealOperationOutput : Vanrise.Entities.UpdateOperationOutput<DealDefinitionDetail>
    {
        public List<string> ValidationMessages { get; set; }
    }
}
