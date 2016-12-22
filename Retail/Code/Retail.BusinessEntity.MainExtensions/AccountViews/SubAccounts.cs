using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class SubAccounts : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("9A5B27E1-4928-4B71-B548-71C2F89444A5"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-account-grid";
            }
            set
            {

            }
        }
    }
}
