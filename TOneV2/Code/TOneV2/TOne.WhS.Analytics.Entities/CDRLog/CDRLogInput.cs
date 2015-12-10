using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public enum CDRType { All = 0, Successful = 1, Failed = 2, Invalid = 3, }
    public class CDRLogInput
    {
        public CDRLogFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int NRecords { get; set; }
        public CDRType CDRType { get; set; }
    }
}
