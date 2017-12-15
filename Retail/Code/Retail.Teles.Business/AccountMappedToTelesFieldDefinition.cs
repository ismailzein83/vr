using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Teles.Business
{
    public class AccountMappedToTelesFieldDefinition : AccountExtraFieldDefinitionSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("B50FF2AB-E6D2-44A4-9681-F119FEC8ABFC"); }
        }
        public string FieldName { get; set; }
        public string FieldTitle { get; set; }
        public override IEnumerable<AccountGenericField> GetFields(IAccountExtraFieldSettingsContext context)
        {
            List<AccountGenericField> accountGenericFields = new List<AccountGenericField>();
            accountGenericFields.Add(new AccountMappedToTelesGenericField(FieldName, FieldTitle));
            return accountGenericFields;
        }
    }
}
