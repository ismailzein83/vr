using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public class BinaryRecordParser
    {
        public BinaryRecordParserSettings Settings { get; set; }
    }

    public abstract class BinaryRecordParserSettings
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IBinaryRecordParserContext context);
    }

    public interface IBinaryRecordParserContext
    {
        string FileName { get; }

        Stream RecordStream { get; }

        ParsedRecord CreateRecord(string recordType, HashSet<string> tempFieldNames);

        void OnRecordParsed(ParsedRecord parsedRecord);

        BinaryRecordParser GetParserTemplate(Guid templateId);

        Dictionary<string, dynamic> GetGlobalVariables();

        void SetGlobalVariable(string variableName, dynamic value);
    }
}
