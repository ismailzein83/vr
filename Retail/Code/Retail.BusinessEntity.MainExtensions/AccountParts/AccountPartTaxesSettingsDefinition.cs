using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartTaxesSettingsDefinition : AccountPartDefinitionSettings
    {
        public override Guid ConfigId { get { return new Guid("6388F486-9BB7-47D2-B16C-31C9FDCE9A8D"); } }
        public List<TaxInviceTypeSettingDefinition> InvoiceTypes { get; set; }
        public override bool IsPartValid(IAccountPartDefinitionIsPartValidContext context)
        {
            AccountPartTaxesSettings part = context.AccountPartSettings.CastWithValidate<AccountPartTaxesSettings>("context.AccountPartSettings");
            return true;
        }
    }
    public class TaxInviceTypeSettingDefinition
    {
        public Guid InvoiceTypeId { get; set; }
    }
}
