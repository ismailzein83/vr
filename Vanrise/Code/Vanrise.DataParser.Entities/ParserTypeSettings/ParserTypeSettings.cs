using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class ParserTypeSettings
    {
        public ParserTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class ParserTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract List<ParsedBatch> Execute(IParserTypeExecuteContext context);
    }

    public interface IParserTypeExecuteContext
    {
        IDataParserInput Input { get; }
    }
}
