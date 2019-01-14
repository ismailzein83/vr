using System;
using System.Collections;

namespace Vanrise.DataParser.Entities
{
    public class BitFieldParser
    {
        public BitFieldParserSettings Settings { get; set; }
    }

    public abstract class BitFieldParserSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IBitFieldParserContext context);
    }

    public interface IBitFieldParserContext
    {
        BitArray FieldValue { get; }

        ParsedRecord Record { get; }
    }

    public class BitFieldParserContext : IBitFieldParserContext
    {
        public BitArray FieldValue { get; set; }

        public ParsedRecord Record { get; set; }
    }
}