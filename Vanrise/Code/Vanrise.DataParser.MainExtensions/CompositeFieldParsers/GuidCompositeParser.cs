using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class GuidCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("43720AC7-DCB3-4994-B82A-3749950E9E15"); }
        }
        public string FieldName { get; set; }
        public override void Execute(ICompositeFieldsParserContext context)
        {
            context.Record.SetFieldValue(FieldName, Guid.NewGuid().ToString());
        }
    }
}
