﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class CallLocationInformationParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("0A6DABA3-CFEF-42E1-B81A-9F88DB7AFFA1"); }
        }

        public string FieldName { get; set; }

        public override void Execute(IHexTLVFieldParserContext context)
        {
            StringBuilder str = new StringBuilder();

            byte[] bytes = new byte[2];
            Array.Copy(context.FieldValue, 0, bytes, 0, 2);
            str.Append(ParserHelper.GetTBCDNumber(bytes, true, false) + "-");

            bytes = new byte[1];
            Array.Copy(context.FieldValue, 2, bytes, 0, 1);
            str.Append(ParserHelper.GetTBCDNumber(bytes, true, false) + "-");

            str.Append(ParserHelper.GetInt(context.FieldValue, 3, 2) + "-");

            str.Append(ParserHelper.GetInt(context.FieldValue, 5, 2));

            context.Record.SetFieldValue(FieldName, str.ToString());
        }
    }
}
