using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class HexTLVRecordParser
    {
        public HexTLVRecordParserSettings Settings { get; set; }
    }

    public abstract class HexTLVRecordParserSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IHexTLVRecordParserContext context);
    }

    public interface IHexTLVRecordParserContext
    {
        string FileName { get; }

        Stream RecordStream { get; }

        ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames);

        void OnRecordParsed(ParsedRecord parsedRecord);

        HexTLVRecordParser GetParserTemplate(Guid templateId);

        Dictionary<string, dynamic> GetGlobalVariables();

        void SetGlobalVariable(string variableName, dynamic value);
    }
}
