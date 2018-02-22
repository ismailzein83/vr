﻿using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.FieldParsers
{
    public class SkipBlockParser : HexTLVFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("A25FB305-71B0-4288-9196-7C0D3A6D2B8B"); } }

        public override void Execute(IHexTLVFieldParserContext context)
        {
           
        }
    }
}