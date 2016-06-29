using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities.EntitySynchronization;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.DBSync.Entities
{
   public class SourceCustomerZone : ISourceItem
    {
        public string SourceId
        {
            get;
            set;
        }

        public int CustomerZonesId { get; set; }
        public int CustomerId { get; set; }
        public List<CustomerCountry> Countries { get; set; }
        public DateTime StartEffectiveTime { get; set; }
    
    }
}
