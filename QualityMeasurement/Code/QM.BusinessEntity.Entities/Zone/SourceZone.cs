using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public class SourceZone : Vanrise.Entities.EntitySynchronization.ISourceItem
    {
        public string SourceId { get; set; }

        public string CountryName { get; set; }

        public string SourceCountryId { get; set; }

        public string Name { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }
    }
}
