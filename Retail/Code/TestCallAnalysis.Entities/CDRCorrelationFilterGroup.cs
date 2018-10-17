using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace TestCallAnalysis.Entities
{
    public class CDRCorrelationFilterGroup
    {
        public RecordFilterGroup RecordFilterGroup { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }
}
