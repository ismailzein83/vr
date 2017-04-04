using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class ParserTypeSettings
    {
        public ParserTypeExtendedSettings Settings { get; set; }
    }

    public abstract class ParserTypeExtendedSettings
    {
        public abstract IEnumerable<ParsedBatch> Execute(IDataParserInput input);
    }

}
