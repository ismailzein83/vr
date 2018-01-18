using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions
{
    public class PortalAccountExtraFieldDefinition : AccountExtraFieldDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("95183B89-056A-410A-B920-91EC6A134F82"); }
        }
        public override IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context)
        {
            List<AccountGenericField> accountGenericFields = new List<AccountGenericField>();
            accountGenericFields.Add(new PrimaryPortalAccountGenericField());
            accountGenericFields.Add(new SecondaryPortalAccountGenericField());
            return accountGenericFields;
        }
    }
}
