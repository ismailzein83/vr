using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Vanrise.NumberingPlan.Entities 
{
    public class ChangedCode : IChangedEntity
    {
        public long EntityId { get; set; }

        public DateTime EED { get; set; }
    }
}
