using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class PositionedBitFieldParser
    {
        public int Position { get; set; }
        public int Length { get; set; }
        public BitFieldParser FieldParser { get; set; }
    }
}