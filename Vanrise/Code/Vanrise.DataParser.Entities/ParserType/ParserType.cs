using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public abstract class ParserType
    {
        public Guid ParserTypeId { get; set; }
        public string Name { get; set; }
        public ParserTypeSettings Settings { get; set; }

    }
}
