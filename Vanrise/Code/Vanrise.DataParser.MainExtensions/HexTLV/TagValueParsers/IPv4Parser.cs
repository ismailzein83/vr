using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class IPv4Parser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { return new Guid("593CDE30-2D89-4303-972B-51F86378E30F"); }
        }
        public string FieldName { get; set; }
        public override void Execute(ITagValueParserExecuteContext context)
        {
           
        }
    }
}
