using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class UpdateSwitchOperationOutput : Vanrise.Entities.UpdateOperationOutput<SwitchDetail>
    {
        public List<string> ValidationMessages { get; set; }
    }
}
