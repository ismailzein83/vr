using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class InternationalCallsBlockedFieldDefinition : AccountExtraFieldDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("E440C268-19B9-4D43-820B-C407604C7EF3"); }
        }
        public override IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context)
        {
            List<AccountGenericField> accountGenericFields = new List<AccountGenericField>();
            accountGenericFields.Add(new InternationalCallsBlockedGenericField());
            return accountGenericFields;
        }
    }
}
