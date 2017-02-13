using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class TelesEnterpriseExtraFieldDefinition : AccountExtraFieldDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("DFFBF1F9-EF68-43FA-BE34-AD181031CDDA"); }
        }
        public Guid EnterpriseBEDefinitionId { get; set; }
        public override IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context)
        {
            List<AccountGenericField> accountGenericFields = new List<AccountGenericField>();
            accountGenericFields.Add(new TelesEnterpriseGenericField(this.EnterpriseBEDefinitionId));
            return accountGenericFields;
        }
    }
}
