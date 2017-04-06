using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.DataParser.Entities.HexTLV;

namespace Vanrise.DataParser.MainExtensions.HexTLV.RecordReaders
{
    public class TagRecordReader : RecordReader
    {
        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
        public Dictionary<string, TagRecordType> RecordTypesByTag { get; set; }

        public override void Execute(IRecordReaderExecuteContext context)
        {
            //foreach (TagRecordType item in TagRecordTypes)
            //{
            //    context.OnRecordRead(item.RecordType, context.Data, item.TagTypes);
            //}
        }
    }

    public class TagRecordType
    {
        public string RecordType { get; set; }

        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }
    }
}
