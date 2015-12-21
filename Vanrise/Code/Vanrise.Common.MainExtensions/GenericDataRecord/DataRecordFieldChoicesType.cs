using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.Entities.GenericDataRecord;

namespace Vanrise.Common.MainExtensions
{
    public class DataRecordFieldChoicesType : DataRecordFieldType
    {
        public List<Choices> Choices { get; set; }
    }
}
