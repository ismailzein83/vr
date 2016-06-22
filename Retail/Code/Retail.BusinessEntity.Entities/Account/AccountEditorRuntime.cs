using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class AccountEditorRuntime
    {
        public List<AccountPartRuntime> Parts { get; set; }
    }

    public class AccountPartRuntime
    {
        public AccountPartDefinition PartDefinition { get; set; }

        public AccountPartRequiredOptions RequiredSettings { get; set; }
    }
}
