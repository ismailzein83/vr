using System;
using System.Linq;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.BinaryParsers.Common.FieldParsers
{
    public class IPv4Parser : BinaryFieldParserSettings
    {
        public override Guid ConfigId { get { return new Guid("593CDE30-2D89-4303-972B-51F86378E30F"); } }
        public string FieldName { get; set; }

        public override void Execute(IBinaryFieldParserContext context)
        {
            string ipAddress = string.Join<int>(".", context.FieldValue.Select(itm => (int)itm));
            context.Record.SetFieldValue(this.FieldName, ipAddress);

        }
    }
}
