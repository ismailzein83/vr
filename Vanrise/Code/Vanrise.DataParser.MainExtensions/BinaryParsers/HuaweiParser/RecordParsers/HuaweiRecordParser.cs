using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using Vanrise.Common;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers
{
    public class HuaweiRecordParser : BinaryRecordParserSettings
    {
        public int RecordLengthPosition { get; set; }
        public int RecordByteLength { get; set; }
        public int RecordTypePosition { get; set; }
        public int RecordTypeByteLength { get; set; }
        public int HeaderLength { get; set; }

        public Dictionary<string, BinaryRecordParser> SubRecordsParsersByRecordType { get; set; }
        public override Guid ConfigId
        {
            get { return new Guid("1BED7CDA-1E98-48C0-8CA0-85EB6C4AB300"); }
        }

        public override void Execute(IBinaryRecordParserContext context)
        {
            while (context.RecordStream.Length - context.RecordStream.Position > 0)
            {
                string recordType = "";
                int recordLength = 0;
                BinaryParserHelper.ReadBlockFromStream(context.RecordStream, HeaderLength, (packageLength) =>
                {
                    byte[] lengthData = new byte[RecordByteLength];
                    Array.Copy(packageLength.Value, RecordLengthPosition, lengthData, 0, RecordByteLength);
                    recordLength = ParserHelper.GetInt(lengthData, 0, RecordByteLength);

                    byte[] recordTypeData = new byte[RecordTypeByteLength];
                    Array.Copy(packageLength.Value, RecordTypePosition, recordTypeData, 0, RecordTypeByteLength);
                    recordType = ParserHelper.ByteArrayToString(recordTypeData, false);

                }, true);


                byte[] data = null;
                int dataLength = recordLength - HeaderLength + RecordTypeByteLength + RecordByteLength;
                data = new byte[dataLength];
                context.RecordStream.Read(data, 0, dataLength);

                BinaryRecordParser subRecordsParser = null;
                if (this.SubRecordsParsersByRecordType != null)
                    this.SubRecordsParsersByRecordType.TryGetValue(recordType, out subRecordsParser);

                if (subRecordsParser != null)
                    BinaryParserHelper.ExecuteRecordParser(subRecordsParser, new MemoryStream(data), context);
            }
        }

    }
}
