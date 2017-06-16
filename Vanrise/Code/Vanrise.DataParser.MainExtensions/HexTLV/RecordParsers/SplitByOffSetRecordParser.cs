using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordParsers
{
    public class SplitByOffSetRecordParser : HexTLVRecordParserSettings
    {
        public override Guid ConfigId
        {
            get { return new Guid("58E5FB1C-974E-4D5D-BF93-6C2D598FA466"); }
        }

        public int LengthNbOfBytes { get; set; }

        public HexTLVRecordParser RecordParser { get; set; }

        public override void Execute(IHexTLVRecordParserContext context)
        {
            throw new NotImplementedException();
        }
    }
}
