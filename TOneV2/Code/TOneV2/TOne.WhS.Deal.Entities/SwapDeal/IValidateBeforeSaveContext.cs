using System;
using System.Collections.Generic;
using System.Linq;

namespace TOne.WhS.Deal.Entities
{
    public interface IValidateBeforeSaveContext
    {
        List<string> ValidateMessages { get; set; }
        bool IsEditMode { get; set; }
        DealDefinition ExistingDeal { get; set; }
    }
}
