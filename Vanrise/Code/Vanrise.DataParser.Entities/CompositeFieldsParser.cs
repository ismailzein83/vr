using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public abstract class CompositeFieldsParser
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(ICompositeFieldsParserContext context);
    }

    public interface ICompositeFieldsParserContext
    {
        ParsedRecord Record { get; }

        string FileName { get; }
    }

    public class CompositeFieldsParserContext : ICompositeFieldsParserContext
    {
        public ParsedRecord Record { get; set; }

        public string FileName { get; set; }
    }
}