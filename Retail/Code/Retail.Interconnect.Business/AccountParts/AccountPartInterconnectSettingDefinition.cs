using System;
using Retail.BusinessEntity.Entities;

namespace Retail.Interconnect.Business
{
    public class AccountPartInterconnectSettingDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("1ECEEBC8-9EE2-4690-9B10-32351F8F0096"); } }
        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            return true;
        }
    }
}
