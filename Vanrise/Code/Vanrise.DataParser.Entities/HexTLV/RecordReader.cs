using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities.HexTLV
{
    public abstract class RecordReader
    {
        public abstract Guid ConfigId { get; }

        public abstract void Execute(IRecordReaderExecuteContext context);
    }

    public interface IRecordReaderExecuteContext
    {
        byte[] Data { get; }

        void OnRecordRead(string recordType, byte[] recordData, Dictionary<string, HexTLVTagType> tagTypes);
    }
}
