    using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountViews
{
   public  class AccountHistoryView: AccountViewDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("FB232763-6AC1-49B5-A410-FA792980055C"); }
        }

        public override string RuntimeEditor
        {
            get
            {
                return "retail-be-accounthistory-view";
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


