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
    public class SkipRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("03E65D50-978C-4641-B2FC-08DAD18849EB"); } }
        public HexTLVRecordParser RecordParser { get; set; }
        public int? RecordToSkipBytesLength { get; set; }
        public string RecordStartingTag { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            if (RecordToSkipBytesLength.HasValue)
            {
                HexTLVHelper.ReadBlockFromStream(context.RecordStream, RecordToSkipBytesLength.Value, (block) =>
                {
                    MemoryStream ms = new MemoryStream();
                    context.RecordStream.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    HexTLVHelper.ExecuteRecordParser(RecordParser, ms, context);
                }, true);
            }
            else
            {
                byte[] tagBytes = ParserHelper.StringToByteArray(RecordStartingTag);
                int tagBytesLength = tagBytes.Length;

                HexTLVHelper.ReadBlockFromStream(context.RecordStream, tagBytesLength, (block) =>
                {
                    if (Vanrise.Common.Utilities.AreEquals(tagBytes, block.Value))
                    {
                        context.RecordStream.Seek(context.RecordStream.Position - tagBytesLength, SeekOrigin.Begin);
                        MemoryStream ms = new MemoryStream();
                        context.RecordStream.CopyTo(ms);
                        ms.Seek(0, SeekOrigin.Begin);

                        HexTLVHelper.ExecuteRecordParser(RecordParser, ms, context);
                    }
                    else
                    {
                        context.RecordStream.Seek(context.RecordStream.Position - tagBytesLength + 1, SeekOrigin.Begin);
                    }
                });
            }
        }
    }
}
