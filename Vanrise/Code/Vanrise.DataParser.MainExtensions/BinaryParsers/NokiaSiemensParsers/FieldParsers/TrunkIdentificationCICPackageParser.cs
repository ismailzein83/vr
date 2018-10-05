using System;
using Vanrise.DataParser.Business;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.NokiaSiemensParsers.FieldParsers
{
    public class TrunkIdentificationCICPackageParser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("CEEDF4FA-E2F7-47BC-91AB-EC176AB8B519"); } }
        public string TrunkGroupNumberCICFieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            byte[] trunkGroupNumberCICBytes = ParserHelper.ExtractFromByteArray(context.FieldValue, 0, 6, false);
            string trunkGroupNumberCIC = System.Text.Encoding.UTF8.GetString(trunkGroupNumberCICBytes);

            context.Record.SetFieldValue(this.TrunkGroupNumberCICFieldName, trunkGroupNumberCIC);
        }
    }
}