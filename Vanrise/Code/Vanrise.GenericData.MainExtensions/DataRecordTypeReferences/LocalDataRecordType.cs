using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordTypeReferences
{
    public class LocalDataRecordType : DataRecordTypeReference
    {
        public DataRecordType DataRecordType { get; set; }

        public override DataRecordType GetDataRecordType(IDataRecordTypeReferenceContext context)
        {
            return this.DataRecordType;
        }
    }
}
