using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Business;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions.DataRecordTypeReferences
{
    public class GlobalDataRecordType : DataRecordTypeReference
    {
        public Guid DataRecordTypeId { get; set; }

        public override DataRecordType GetDataRecordType(IDataRecordTypeReferenceContext context)
        {
            DataRecordTypeManager manager = new DataRecordTypeManager();
            return manager.GetDataRecordType(this.DataRecordTypeId);
        }
    }
}
