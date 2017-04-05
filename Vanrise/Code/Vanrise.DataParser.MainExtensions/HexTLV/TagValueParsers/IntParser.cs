using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class IntParser : TagValueParser
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public string FieldName { get; set; }
    }
}
