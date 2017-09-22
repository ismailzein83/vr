using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class RevertStatusDefinitionPostAction : AccountProvisionDefinitionPostAction
    {
        public override Guid ConfigId
        {
            get { return new Guid("889C2DA2-5BBA-4316-A245-521E85E3FBE8"); }
        }

        public override string RuntimeDirective
        {
            get { return "retail-be-actionbpdefinition-runtimepostaction-revertstatus"; }
        }
        public Guid RevertToStatusDefinitionId { get; set; }
    }
}
