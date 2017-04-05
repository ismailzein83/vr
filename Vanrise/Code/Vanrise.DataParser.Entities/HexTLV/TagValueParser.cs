using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities.HexTLV
{
    public abstract class TagValueParser
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(ITagValueParserExecuteContext context);
    }

    public interface ITagValueParserExecuteContext
    {
        List<byte> TagValue { get; }

        ParsedRecord Record { get; }
    }
}
