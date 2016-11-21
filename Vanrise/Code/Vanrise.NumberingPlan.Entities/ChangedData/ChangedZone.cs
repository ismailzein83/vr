using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.NumberingPlan.Entities
{
    public class ChangedZone : IChangedEntity
    {
        public long EntityId { get; set; }

        public DateTime EED { get; set; }

    }
}
