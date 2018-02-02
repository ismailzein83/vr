using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class Changes
    {
        public int? CurrencyId { get; set; }

        public DefaultChanges DefaultChanges { get; set; }

        public List<ZoneChanges> ZoneChanges { get; set; }

		public CountryChanges CountryChanges { get; set; }

        public List<SubscriberOwnerEntity> SubscriberOwnerEntities { get; set; }
    }

    public class SubscriberOwnerEntity
    {
        public int Id { get; set; }
        public int EntityId { get; set; }
        public string EntityName { get; set; }
    }
}
