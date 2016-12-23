using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class AccountPackages : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("BB2CBAE6-05A1-4132-A2E0-F6C761B273DA"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountpackages-view";
            }
            set
            {

            }
        }
    }
}
