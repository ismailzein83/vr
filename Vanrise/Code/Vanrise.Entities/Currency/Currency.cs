using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace Vanrise.Entities
{
    public class Currency : Vanrise.Entities.EntitySynchronization.IItem
    {
        public int CurrencyId { get; set; }

        public string Name { get; set; }

        public string Symbol { get; set; }

        public string SourceId { get; set; }

        long IItem.ItemId
        {
            get
            {
                return CurrencyId;
            }
            set
            {
                this.CurrencyId = (int)value;
            }
        }
    }
}
