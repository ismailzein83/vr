using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class BinaryFieldParser
    {
        public BinaryFieldParserSettings Settings { get; set; }
    }
    public abstract class BinaryFieldParserSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IBinaryFieldParserContext context);
    }

    public interface IBinaryFieldParserContext
    {
        byte[] FieldValue { get; }

        ParsedRecord Record { get; }
    }

    public class BinaryFieldParserContext : IBinaryFieldParserContext
    {
        public byte[] FieldValue { get; set; }

        public ParsedRecord Record { get; set; }
    }
}
