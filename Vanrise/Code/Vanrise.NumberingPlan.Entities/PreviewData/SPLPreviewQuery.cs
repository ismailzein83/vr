using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SPLPreviewQuery
    {
        public int ProcessInstanceId { get; set; }
        public int? CountryId { get; set; }
        public bool OnlyModified { get; set; }
        public string ZoneName { get; set; }
    }
}
