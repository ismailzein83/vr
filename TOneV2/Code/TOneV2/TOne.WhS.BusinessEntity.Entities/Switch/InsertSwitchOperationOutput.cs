using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class InsertSwitchOperationOutput : Vanrise.Entities.InsertOperationOutput<SwitchDetail>
    {
        public List<string> ValidationMessages { get; set; }
    }
}
