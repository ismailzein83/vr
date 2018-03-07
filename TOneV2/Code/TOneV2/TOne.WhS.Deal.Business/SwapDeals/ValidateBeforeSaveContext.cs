using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.Business
{
    public class ValidateBeforeSaveContext : IValidateBeforeSaveContext
    {

        public List<string> ValidateMessages { get; set; }

        public bool IsEditMode { get; set; }
        public DealDefinition ExistingDeal { get; set; }
    }
}
