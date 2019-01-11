using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.GenericData.Entities.DataRecordTypes
{
    public class RDBDataRecordFieldAttribute
    {
        public RDBDataType RDBDataType { get; set; }
        public int? Size { get; set; }
        public int? Precision { get; set; }
    }
}
