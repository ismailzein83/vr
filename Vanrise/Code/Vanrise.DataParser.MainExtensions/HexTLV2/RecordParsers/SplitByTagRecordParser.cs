using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV2;
using Vanrise.Common;
using Vanrise.DataParser.Business.HexTLV2;
using System.IO;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.RecordParsers
{
    public class SplitByTagRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("3F2E745A-9AE6-44AA-AC8A-C7D383B4A3B7"); }
        }

        public HexTLVRecordParser DefaultSubRecordParser { get; set; }

        public Dictionary<string, HexTLVRecordParser> SubRecordsParsersByTag { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            HexTLVHelper.ReadTagsFromStream(context.RecordStream,
                (tagValue) =>
                {
                    HexTLVRecordParser subRecordsParser = null;
                    if (this.SubRecordsParsersByTag != null)
                        this.SubRecordsParsersByTag.TryGetValue(tagValue.Tag, out subRecordsParser);

                    if (subRecordsParser == null)
                        subRecordsParser = this.DefaultSubRecordParser;

                    if (subRecordsParser != null)
                        ExecuteRecordParser(subRecordsParser, new MemoryStream(tagValue.Value), context);
                });
        }

        public static void ExecuteRecordParser(HexTLVRecordParser subRecordsParser, Stream recordStream, IHexTLVRecordParserContext parentRecordContext)
        {
            //subRecordsParser.Settings.ThrowIfNull("subRecordParser.Settings");
            //var subRecordContext = new SubRecordHexTLVRecordParserContext(recordStream, parentRecordContext);
            //subRecordsParser.Settings.Execute(subRecordContext);
        }
    }
}
