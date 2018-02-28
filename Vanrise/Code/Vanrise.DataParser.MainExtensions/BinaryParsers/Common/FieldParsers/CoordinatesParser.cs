using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV2.FieldParsers
{
    public class CoordinatesParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("8A5B5D5F-68FE-4ED5-8A45-09F62EF3B817"); } }

        public string LongitudeFieldName { get; set; }

        public string LatitudeFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            string longitude = string.Format("{0} {1} {2}", ParserHelper.GetHexFromByte(context.FieldValue[1])
                                                            ,ParserHelper.GetHexFromByte(context.FieldValue[2])
                                                            ,ParserHelper.GetHexFromByte(context.FieldValue[3]));

            string latitude = string.Format("{0} {1} {2}", ParserHelper.GetHexFromByte(context.FieldValue[4])
                                                ,ParserHelper.GetHexFromByte(context.FieldValue[5])
                                                ,ParserHelper.GetHexFromByte(context.FieldValue[6]));

            context.Record.SetFieldValue(this.LongitudeFieldName, longitude);
            context.Record.SetFieldValue(this.LatitudeFieldName, latitude);
        }
    }
}
