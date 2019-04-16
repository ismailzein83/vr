using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.GenericData.MainExtensions
{
    public class ParentDataRecordTypeExtraFields : DataRecordTypeExtraField
    {
        public override Guid ConfigId { get { return new Guid("043E058A-0E0E-40E2-82AB-FE04C896E615"); } }

        public Guid DataRecordTypeId { get; set; }

        public override List<DataRecordField> GetFields(IDataRecordExtraFieldContext context)
        {
            return null;
        }
    }
}
