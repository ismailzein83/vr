using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public abstract class CompositeFieldsParser
    {
        public abstract void Execute(ICompositeFieldsParserContext context);
    }

    public interface ICompositeFieldsParserContext
    {
        ParsedRecord Record { get; }
    }
}
