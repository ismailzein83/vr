using Retail.BusinessEntity.Business;
using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class EditAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("2504A630-D16B-43DC-8505-F85E3DFD0568"); }
        }

        public override string ClientActionName
        {
            get { return "Edit"; }
        }

        public override bool DoesUserHaveAccess(IAccountActionDefinitionCheckAccessContext context)
        {
            return new AccountBEDefinitionManager().DoesUserHaveEditAccess(context.UserId, context.AccountBEDefinitionId);
        }
    }
}
