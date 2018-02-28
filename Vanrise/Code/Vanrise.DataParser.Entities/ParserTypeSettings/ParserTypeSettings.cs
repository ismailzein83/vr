using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class ParserTypeSettings
    {
        public Boolean UseRecordType { get; set; }
        public ParserTypeExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class ParserTypeExtendedSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void Execute(IParserTypeExecuteContext context);
    }

    public interface IParserTypeExecuteContext
    {
        IDataParserInput Input { get; }

        ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames);

        void OnRecordParsed(ParsedRecord parsedRecord);

        Dictionary<string, dynamic> GetGlobalVariables();

        void SetGlobalVariable(string variableName, dynamic value);
    }
}
