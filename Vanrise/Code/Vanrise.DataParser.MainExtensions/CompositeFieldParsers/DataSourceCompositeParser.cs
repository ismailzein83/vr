using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.CompositeFieldParsers
{
    public class DataSourceCompositeParser : CompositeFieldsParser
    {
        public override Guid ConfigId { get { return new Guid("9081E437-0664-48C2-AD44-55BB6E542829"); } }

        public string DataSourceFieldName { get; set; } 

        public override void Execute(ICompositeFieldsParserContext context)
        {
            context.Record.SetFieldValue(this.DataSourceFieldName, context.DataSourceId);
        }
    }
}