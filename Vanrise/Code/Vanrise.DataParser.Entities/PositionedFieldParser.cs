using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class PositionedFieldParser
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public HexTLVFieldParser FieldParser { get; set; }
    }
}
