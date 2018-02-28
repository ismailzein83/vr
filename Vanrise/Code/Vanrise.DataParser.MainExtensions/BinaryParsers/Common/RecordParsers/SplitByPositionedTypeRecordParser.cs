using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Common;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.RecordParsers
{
    public enum RecordTypeFieldType { Int, String }
    public class SplitByPositionedTypeRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("2FCEDED6-C422-4FB2-92C7-3ED32D6EEF78"); } }
        public int RecordTypePosition { get; set; }
        public int RecordTypeLength { get; set; }
        public RecordTypeFieldType RecordTypeFieldType { get; set; }
        public Dictionary<string, BinaryRecordParser> SubRecordsParsersByRecordType { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            byte[] typeData = new byte[RecordTypeLength];
            byte[] data = ((MemoryStream)context.RecordStream).ToArray();

            Array.Copy(data, RecordTypePosition, typeData, 0, RecordTypeLength);
            string recordType = "";
            switch (RecordTypeFieldType)
            {
                case RecordTypeFieldType.Int:
                    recordType = ParserHelper.GetInt(typeData, 0, RecordTypeLength).ToString();
                    break;
                case RecordTypeFieldType.String:
                    recordType = typeData.ConvertByteToString();
                    break;
                default:
                    recordType = ParserHelper.GetInt(typeData, 0, RecordTypeLength).ToString();
                    break;
            }

            context.RecordStream.Seek(0, SeekOrigin.Begin);

            BinaryRecordParser subRecordsParser = null;
            if (this.SubRecordsParsersByRecordType != null)
                this.SubRecordsParsersByRecordType.TryGetValue(recordType, out subRecordsParser);

            if (subRecordsParser != null)
                BinaryParserHelper.ExecuteRecordParser(subRecordsParser, context.RecordStream, context);
        }
    }
}
