using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.CustomerAccess.Entities
{
    public  class AnalyticTileInfo
    {
        public List<AnalyticTileField> Fields { get; set; }
    }
    public class AnalyticTileField
    {
        public string Description { get; set; }
        public Object Value { get; set; }
    }
}
