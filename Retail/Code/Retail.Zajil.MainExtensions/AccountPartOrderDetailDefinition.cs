using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.Zajil.MainExtensions
{
    public class AccountPartOrderDetailDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("2241197C-B5B0-48E5-987A-B3C1949760CB"); } }


        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartOrderDetail part = context.AccountPartSettings.CastWithValidate<AccountPartOrderDetail>("context.AccountPartSettings");
            return true;
        }
    }
}
