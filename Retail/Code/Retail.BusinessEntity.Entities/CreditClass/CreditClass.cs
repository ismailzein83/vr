using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Entities
{
    public class CreditClass
    {
        public const string BUSINESSENTITY_DEFINITION_NAME = "Retail_BE_CreditClass";
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("EC73C4A4-96AD-4A3A-9FF3-B5C3EE1031D5");

        public int CreditClassId { get; set; }

        public string Name { get; set; }

        public CreditClassSettings Settings { get; set; }
    }

    public class CreditClassSettings
    {
        public Decimal BalanceLimit { get; set; }

        public int CurrencyId { get; set; }
    }
}
