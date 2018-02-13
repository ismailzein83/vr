using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class PositionedBlockRecordParser : HexTLVRecordParserSettings
    {
        public int BlockSize { get; set; }

        public HexTLVRecordParser RecordParser { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("A7804AF5-8A20-4B15-A768-6DF1BE5E9742"); }
        }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            HexTLVHelper.ReadBlockFromStream(context.RecordStream, BlockSize, (block) =>
            {
                HexTLVHelper.ExecuteRecordParser(RecordParser, new MemoryStream(block.Value), context);

            });
        }
    }
}
