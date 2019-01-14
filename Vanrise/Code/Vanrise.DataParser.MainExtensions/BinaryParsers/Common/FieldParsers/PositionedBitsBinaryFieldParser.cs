using System;
using System.Collections.Generic;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class PositionedBitsBinaryFieldParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId => throw new NotImplementedException();

        public List<PositionedBitFieldParser> BitParsers { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            throw new NotImplementedException();
        }
    }
}