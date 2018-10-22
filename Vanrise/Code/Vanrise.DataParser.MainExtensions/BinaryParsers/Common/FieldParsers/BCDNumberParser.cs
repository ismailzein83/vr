﻿using System;
using System.Text;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;
using System.Linq;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class BCDNumberParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("78168E82-0528-4F3A-9E7C-47AEF279CD49"); } }
        public string FieldName { get; set; }
        public bool AIsZero { get; set; }
        public bool RemoveHexa { get; set; }
        public bool Reverse { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            context.Record.SetFieldValue(this.FieldName, ParserHelper.GetBCDNumber(Reverse ? context.FieldValue.Reverse().ToArray() : context.FieldValue, this.RemoveHexa, this.AIsZero));
        }
    }
}
