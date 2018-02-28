using System;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers
{
    public class PositionedBlockRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("A7804AF5-8A20-4B15-A768-6DF1BE5E9742"); } }

        public int BlockSize { get; set; }

        public BinaryRecordParser RecordParser { get; set; }


        public override void Execute(IBinaryRecordParserContext context)
        {
            BinaryParserHelper.ReadBlockFromStream(context.RecordStream, BlockSize, (block) =>
            {
                BinaryParserHelper.ExecuteRecordParser(RecordParser, new MemoryStream(block.Value), context);

            });
        }
    }
}
