using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class HexTLVFieldParser
    {
        public HexTLVFieldParserSettings Settings { get; set; }
    }
    public abstract class HexTLVFieldParserSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IHexTLVFieldParserContext context);
    }

    public interface IHexTLVFieldParserContext
    {
        byte[] FieldValue { get; }

        ParsedRecord Record { get; }
    }
}
