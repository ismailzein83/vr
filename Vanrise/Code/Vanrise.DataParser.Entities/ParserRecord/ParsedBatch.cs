using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class ParsedBatch
    {
        public List<dynamic> Records { get; set; }
        public string RecordType { get; set; }

    }
}
