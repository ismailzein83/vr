    using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
    public class AccountManagerAssignmentView : AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("5A58B7C2-3179-42DE-9724-C66F6BEDB61B"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accountmanagerassignment-view";
            }
            set
            {

            }
        }

        public override bool DoesUserHaveAccess(IAccountViewDefinitionCheckAccessContext context)
        {
            return base.DoesUserHaveAccess(context);
        }
    }
}


