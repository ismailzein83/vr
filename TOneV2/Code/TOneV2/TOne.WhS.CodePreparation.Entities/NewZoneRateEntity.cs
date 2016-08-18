using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.CodePreparation.Entities
{
    public class NewZoneRateEntity
    {
        public int OwnerId { get; set; }

        public SalePriceListOwnerType OwnerType { get; set; }

        public decimal Rate { get; set; }

        public int CurrencyId { get; set; }
    }
}
