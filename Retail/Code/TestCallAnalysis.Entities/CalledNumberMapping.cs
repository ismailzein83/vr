using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestCallAnalysis.Entities
{
    public class CalledNumberMapping
    {
        public long ID { get; set; }

        public long OperatorID {get; set;}

        public string Number { get; set; }

        public string MappedNumber { get; set; }
    }
}
