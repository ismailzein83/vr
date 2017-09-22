using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class ChangeStatusDefinitionPostAction : AccountProvisionDefinitionPostAction
    {
        public override Guid ConfigId
        {
            get { return new Guid("FB3B7F00-0D58-4A11-9BE8-DCD9A9212C58"); }
        }

        public override string RuntimeDirective
        {
            get { return "retail-be-actionbpdefinition-runtimepostaction-changestatus"; }
        }
        public Guid NewStatusDefinitionId { get; set; }
        public List<Guid> ExistingStatusDefinitionIds { get; set; }
    }
}
