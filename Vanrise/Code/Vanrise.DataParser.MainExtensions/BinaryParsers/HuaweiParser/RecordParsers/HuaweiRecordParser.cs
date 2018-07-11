﻿using System;
using System.Collections.Generic;
using System.IO;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.HuaweiParser.RecordParsers
{
    public class HuaweiRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("1BED7CDA-1E98-48C0-8CA0-85EB6C4AB300"); } }
        public int HeaderLength { get; set; }
        public int RecordLengthPosition { get; set; }
        public int RecordByteLength { get; set; }
        public int RecordTypePosition { get; set; }
        public int RecordTypeByteLength { get; set; }
        public Dictionary<string, BinaryRecordParser> SubRecordsParsersByRecordType { get; set; }

        public override void Execute(IBinaryRecordParserContext context)
        {
            while (context.RecordStream.Length - context.RecordStream.Position > 0)
            {
                string recordType = "";
                int recordLength = 0;
                BinaryParserHelper.ReadBlockFromStream(context.RecordStream, HeaderLength, (header) =>
                {
                    byte[] lengthData = new byte[RecordByteLength];
                    Array.Copy(header.Value, RecordLengthPosition, lengthData, 0, RecordByteLength);
                    recordLength = ParserHelper.GetInt(lengthData, 0, RecordByteLength);

                    byte[] recordTypeData = new byte[RecordTypeByteLength];
                    Array.Copy(header.Value, RecordTypePosition, recordTypeData, 0, RecordTypeByteLength);
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

    public class SkipTagValueRecordParser : BinaryRecordParserSettings
    {
        public override Guid ConfigId { get { return new Guid("AD4EA28E-46F4-4D0F-9EE8-DA5803ED7DAD"); } }

        public override void Execute(IBinaryRecordParserContext context)
        {

        }
    }
}