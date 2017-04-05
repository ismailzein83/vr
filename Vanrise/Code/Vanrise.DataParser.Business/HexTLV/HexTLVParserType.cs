using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.Business.HexTLV
{
    public class HexTLVParserType : ParserTypeExtendedSettings
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public RecordReader RecordReader { get; set; }

        public override List<ParsedBatch> Execute(IParserTypeExecuteContext context)
        {
            throw new NotImplementedException();
        }

      
    }
}
