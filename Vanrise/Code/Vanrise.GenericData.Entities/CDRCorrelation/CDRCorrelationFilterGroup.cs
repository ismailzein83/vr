using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.GenericData.Entities
{
    public class CDRCorrelationFilterGroup
    {
        public RecordFilterGroup RecordFilterGroup { get; set; }

        public DateTime From { get; set; }

        public DateTime? To { get; set; }
    }
}
