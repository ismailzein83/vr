using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.DataParser.Entities
{
    public interface IDataParserInput
    {

    }

    public class StreamDataParserInput : IDataParserInput
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public Guid DataSourceId { get; set; }
        public byte[] Data { get; set; }
    }
}