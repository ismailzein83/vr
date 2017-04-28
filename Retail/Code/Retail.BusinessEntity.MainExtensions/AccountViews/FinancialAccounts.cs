using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class FinancialAccounts : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("006BA22A-439D-4BF5-B8C2-0254C2F6B40C"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-financialaccount-view";
            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
}
