using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountBEActionTypes
{
    public class AssignAccountManagerAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1504D308-2BC5-48F9-80B9-2AC0BE536288"); }
        }
        public override string ClientActionName
        {
            get { return "AssignAccountManagerAccountAction"; }
        }

    }
}
