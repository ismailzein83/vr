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
        public List<TagRecordType> TagRecordTypes { get; set; }

       
    }

    public class TagRecordType
    {
        public string Tag { get; set; }

        public string RecordType { get; set; }

        public Dictionary<string, HexTLVTagType> TagTypes { get; set; }
    }
}
