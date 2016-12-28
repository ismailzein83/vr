using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.AccountParts
{
    public class AccountPartFinancial : AccountPartSettings, IAccountPayment
    {
        public static Guid _ConfigId = new Guid("82228BE2-E633-4EF8-B383-9894F28C8CB0");
        public override Guid ConfigId { get { return _ConfigId; } }
        public int CreditClassId { get; set; }
        public int CurrencyId { get; set; }
        public int ContractId { get; set; }

        public override dynamic GetFieldValue(IAccountPartGetFieldValueContext context)
        {
            switch (context.FieldName)
            {
                case "Currency": return this.CurrencyId;

                case "CreditClass": return this.CreditClassId;

                default: return null;
            }
        }
    }
}
