using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.BEActions.AccountBEActionType
{
    public class Open360DegreeAccountActionSettings : AccountActionDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("1819FC7B-B159-49CD-B678-261B3D0F41D5"); }
        }

        public override string ClientActionName
        {
            get { return "Open360Degree"; }
        }
    }
}
