using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class DealSettings
    {
        public abstract Guid ConfigId { get; }

        public DateTime BeginDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
