using System;
using System.Collections;
using System.Collections.Generic;
using Vanrise.Common;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class PositionedBitsBinaryFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("D71C98B4-4686-44BD-B891-566BA477CF62"); } }

        public List<PositionedBitFieldParser> BitParsers { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            this.BitParsers.ThrowIfNull("positionedBitFieldParser");

            byte[] dataByteArray = context.FieldValue;

            byte[] reversedDataByteArray = new byte[dataByteArray.Length];
            for (int i = 0; i < dataByteArray.Length; i++)
                reversedDataByteArray[i] = ParserHelper.GetReversedByte(dataByteArray[i]);

            BitArray dataBitArray = new BitArray(reversedDataByteArray);

            foreach (var positionedBitFieldParser in this.BitParsers)
            {
                BinaryParserHelper.ReadRecordFieldFromBitArray(dataBitArray, positionedBitFieldParser.Position, positionedBitFieldParser.Length, (recordValue) =>
                {
                    var bitFieldParserContext = new BitFieldParserContext { Record = context.Record, FieldValue = recordValue.Value };
                    positionedBitFieldParser.FieldParser.Settings.Execute(bitFieldParserContext);
                });
            }
        }
    }
}