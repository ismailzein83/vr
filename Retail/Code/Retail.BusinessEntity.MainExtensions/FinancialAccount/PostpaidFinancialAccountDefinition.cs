using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.FinancialAccount
{
    public class PostpaidFinancialAccountDefinition : FinancialAccountDefinitionExtendedSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0953E01C-8F4E-4C01-A714-FE55F62882A8"); }
        }
        public override string RuntimeEditor { get { return "retail-be-financialaccount-postpaid"; } }
    }
}
