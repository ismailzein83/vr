using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities
{
    public class RDBDataRecordFieldAttribute
    {
        public RDBDataType RdbDataType { get; set; }
        public int? Size { get; set; }
        public int? Precision { get; set; }
    }
}
