using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace TOne.BusinessEntity.Entities
{
    public class OrgChartAccountManagerInfo : OrgChartLinkedEntityInfo
    {

        public override string GetIdentifier()
        {
            return "TOneCarrierAccountAccountManager";
        }
    }
}
