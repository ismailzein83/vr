using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class FileNameCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("5DE5361E-AE73-4613-A9C6-EBE8E71B9145"); } }
        
        public string FieldName { get; set; }

        public override void Execute(ICompositeFieldsParserContext context)
        {
            context.Record.SetFieldValue(FieldName, context.FileName);
        }
    }
}