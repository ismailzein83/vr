using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.BEActions.AccountBEActionType
{
    public class EditAccountActionSettings : AccountBEActionSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }

        public override string ClientActionName
        {
            get { return "Edit"; }
        }
    }
}
