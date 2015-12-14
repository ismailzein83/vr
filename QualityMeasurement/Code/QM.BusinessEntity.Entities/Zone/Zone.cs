using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.BusinessEntity.Entities
{
    public class Zone : Vanrise.Entities.EntitySynchronization.IItem
    {
        public long ZoneId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        long IItem.ItemId
        {
            get
            {
                return this.ZoneId;
            }
            set
            {
                this.ZoneId = value;
            }
        }
    }
}
