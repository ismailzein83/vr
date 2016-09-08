using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class DefaultServicePreview
    {
        public List<ZoneService> CurrentServices { get; set; }
        public bool? IsCurrentServiceInherited { get; set; }
        public List<ZoneService> NewServices { get; set; }
        public DateTime EffectiveOn { get; set; }
        public DateTime? EffectiveUntil { get; set; }
    }
}
