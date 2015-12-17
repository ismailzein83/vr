using System;
using System.Collections.Generic;

namespace QM.BusinessEntity.Entities
{
    public class ZoneQuery
    {
        public List<int> Countries { get; set; }
        public string Name { get; set; }
        public DateTime? EffectiveOn { get; set; }
    }
}
