using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class BinaryFieldParserCollection
    {
        public Dictionary<string, BinaryFieldParser> FieldParsersByTag { get; set; }
    }
}
