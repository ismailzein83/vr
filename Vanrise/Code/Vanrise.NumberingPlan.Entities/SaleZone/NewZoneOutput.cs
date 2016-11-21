using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class NewZoneOutput
    {
        public NewZoneOutput()
        {
            ZoneItems = new List<ZoneItem>();
        }
        public string Message { get; set; }
        public List<ZoneItem> ZoneItems { get; set; }
        public ValidationOutput Result { get; set; }
    }
}
