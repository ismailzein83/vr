using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.TagValueParsers
{
    public class SequenceParser : TagValueParser
    {
        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }
    }
}
