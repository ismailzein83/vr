using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public class CDRQuery
    {
       
        public string CDPN { get; set; }
        public string CGPN { get; set; }
        public Direction DirectionType { get; set; }
        public decimal? MinDuration { get; set; }
        public decimal? MaxDuration { get; set; }
        public int NumberRecords { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<int> DataSourceIds { get; set; }
        public List<int> Directions { get; set; }
        public List<int> ServiceTypes { get; set; }
        public List<int> CDRTypes { get; set; }
    }
}
